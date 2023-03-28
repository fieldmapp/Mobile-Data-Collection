from typing import Dict
from .utils import calc_parallel_c, construct_parallel_polygon
import pandas as pd
import geopandas as gpd
import shapely.geometry as shp
from shapely import Geometry


class LaneDetail:
    utc_time: int
    lat: int
    lon: int
    ltype: str
    geom: shp.Point

    def __init__(self, utc_time, lat, lon, ltype):
        self.utc_time = utc_time
        self.lat = lat
        self.lon = lon
        self.ltype = ltype

    @classmethod
    def from_pd(cls, pandas):
        return LaneDetail(pandas.UTC_Seconds, pandas.geometry.x, pandas.geometry.y, pandas.Action)


class LaneSegment:
    start: LaneDetail
    end: LaneDetail
    lane: int
    cause: LaneDetail = None
    damage: LaneDetail = None
    width: float

    def __init__(self, lane_index: int, start: LaneDetail, width: float):
        self.lane = lane_index
        self.start = start
        self.width = width

    def add_lane_detail(self, detail: LaneDetail):
        if detail.ltype == "close":
            self.end = detail
        elif detail.ltype.startswith("damage="):
            self.damage = detail
        elif detail.ltype.startswith("cause="):
            self.damage = detail

    def is_valid(self) -> bool:
        # at least damage or cause have to be filled so that a lane detail is valid
        return self.damage is not None or self.cause is not None

    def to_dict(self, **kwargs) -> dict:
        shift = kwargs.get("shift_by", 0)
        geom_type = kwargs.get("geom_type", shp.Point)
        geom: Geometry

        if shift is not None:
            geom = self.get_shifted_segment(shift_by=shift)
        else:
            geom = self.get_open_close_as_points()

        if geom_type is shp.Point or geom_type is shp.MultiPoint:
            geom = geom.boundary if type(geom) is shp.LineString else shp.MultiPoint(geom)
        if geom_type is shp.LineString:
            geom = shp.LineString(geom)
        if geom_type is shp.Polygon:
            geom = geom if type(geom) is shp.LineString else shp.LineString(geom)
            factor = 1 if shift < 0 else -1
            factor = 0.5 if shift == 0 else factor
            geom = construct_parallel_polygon((geom.xy[0][0], geom.xy[1][0]),
                                              (geom.xy[0][1], geom.xy[1][1]), factor*self.width)
        res = {
            "start": self.start.utc_time,
            "lane": self.lane,
            "geometry": geom
        }
        res = self.get_cause(res)
        return res

    def get_cause(self, res: dict):
        if self.cause is not None:
            cause = self.cause.ltype.split("=")
            res[cause[0]] = cause[1]
        if self.damage is not None:
            dmg = self.damage.ltype.split("=")
            res[dmg[0]] = dmg[1]
        return res

    def get_open_close(self):
        if self.start is None:
            raise ValueError(f"No open node found for {self}")
        if self.end is None:
            raise ValueError(f"No close node found for {self}")
        return self.start, self.end

    def get_open_close_as_points(self) -> tuple:
        o, c = self.get_open_close()
        return shp.Point(o.lat, o.lon), shp.Point(c.lat, c.lon)

    def get_line_prop(self) -> dict:
        return {
            "start": self.start,
            "lane": self.lane
        }

    def get_shifted_segment(self, shift_by: float) -> shp.LineString:
        open_node, close_node = self.get_open_close()
        a, b = calc_parallel_c((open_node.lat, open_node.lon), (close_node.lat, close_node.lon), shift_by)
        return shp.LineString([a, b])


class Lane:
    lane_nr: int
    segments: [LaneSegment]
    lane_shift: float
    lane_width: float
    crs: str

    def __init__(self, lane_nr: int, segments: [LaneSegment], shift: float, width: float ,crs: str):
        # filter before adding segements to lane
        segs = [i for i in segments if i.is_valid()]
        self.lane_nr = lane_nr
        self.segments = segs
        self.lane_shift = shift
        self.crs = crs
        self.lane_width = width

    def to_gpd(self, no_shift: bool, **kwargs) -> gpd.GeoDataFrame:
        geom_type = kwargs.get("geom_type", shp.Point)
        i: LaneSegment
        rows = [i.to_dict(shift_by=0 if no_shift else self.lane_shift,
                          geom_type=geom_type) for i in self.segments]
        df = gpd.GeoDataFrame(rows, crs=self.crs)
        return df


class Field:
    lanes: dict
    input_epsg: int
    target_epsg: int
    lane_config: dict
    lane_width: float

    def __init__(self, all_lanes, input_epsg: int, target_epsg: int, lane_config: dict, lane_width: float):
        self.lanes = all_lanes
        self.lane_config = lane_config
        self.input_epsg = input_epsg
        self.target_epsg = target_epsg
        self.lane_width = lane_width

    def to_gpd(self, **kwargs) -> gpd.GeoDataFrame:
        # should the output geom be the shifted representation of the recorded lane
        # if true output geom is the start and end point of a recorded lane
        # if false output geom is the linestring of the recorded lane shifted by
        # its defined distance from the driving lane
        no_shift = kwargs.get("no_shift", True)
        geom_type = kwargs.get("geom_type", shp.Point)
        v: Lane
        res: gpd.GeoDataFrame
        res = pd.concat([v.to_gpd(no_shift=no_shift, geom_type=geom_type) for k, v in self.lanes.items()])
        if self.input_epsg != self.target_epsg:
            res = res.to_crs(epsg=self.target_epsg)
        return res

    @classmethod
    def from_gpd(cls, df: gpd.GeoDataFrame, target_epsg: int, lane_config: dict, lane_width: float):
        all_l: Dict[int, Lane] = dict()
        for k, v in lane_config.items():
            interactions_filtered: pd.DataFrame = df.loc[df["LaneIndex"] == k]
            segment = None
            lanes: [LaneSegment] = []
            for interaction in interactions_filtered.itertuples(False):
                detail = LaneDetail.from_pd(interaction)
                if detail.ltype == 'open':
                    segment = LaneSegment(lane_index=interaction.LaneIndex,
                                          start=detail,
                                          width=lane_width)
                    lanes.append(segment)
                else:
                    try:
                        segment.add_lane_detail(detail)
                    except Exception as e:
                        # if segment was never opened
                        print(f"Error creating segment {e}")
            all_l[k] = Lane(lane_nr=k,
                            segments=lanes,
                            shift=lane_config[k],
                            width=lane_width,
                            crs=df.crs)
        field = Field(all_lanes=all_l,
                      lane_config=lane_config,
                      target_epsg=target_epsg,
                      input_epsg=df.crs,
                      lane_width=lane_width)
        return field
