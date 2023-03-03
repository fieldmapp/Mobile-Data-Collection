import pandas as pd
from astropy import time
from typing import Dict
import geopandas as gpd
import math
import shapely.geometry as shp


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
        shift = kwargs.get("shift_by", None)
        geom_type = kwargs.get("geom_type", shp.Point)
        geom = self.get_open_close_as_points()

        if shift is not None:
            geom = self.get_shifted_segment(shift_by=shift)

        if geom_type is shp.Point or geom_type is shp.MultiPoint:
            geom = geom.boundary if type(geom) is shp.LineString else shp.MultiPoint(geom)
        if geom_type is shp.LineString:
            geom = shp.LineString(geom)
        if geom_type is shp.Polygon:
            geom = geom if type(geom) is shp.LineString else shp.LineString(geom)
            geom = construct_parallel_polygon((geom.xy[0][0], geom.xy[1][0]),
                                              (geom.xy[0][1], geom.xy[1][1]), self.width)
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
        rows = [i.to_dict(shift_by=None if no_shift else self.lane_shift,
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


def gpst_to_utc(gpst_seconds: pd.Series) -> int:
    # GPST = TAI - 19s (constant)
    t_tai = time.Time(gpst_seconds + 19, format='unix_tai', scale='tai')
    t_utc = time.Time(t_tai, format='unix', scale='utc')
    return t_utc.to_value('unix')


def gpst_leapseconds(gpst_sec: pd.Series) -> int:
    """
    The GPS time scale began on January 6, 1980.  At that time, the UTC timescale had undergone 19 leap second events (TAI-UTC).
    so we need to substract 19s from the current TAI-UTC LS to get the correct UTC-Timestamp from the provided GPST.

    https://raw.githubusercontent.com/tomojitakasu/RTKLIB/rtklib_2.4.3/doc/manual_2.4.2.pdf page 131, 31
    astro py epoch https://docs.astropy.org/en/stable/time/index.html#time-from-epoch-formats
    TAI = UTC + LS
    GPS_LS = LS - 19 , GPS since 1980, UTC had 19 LS since then
    UTC = GPS - GPS_LS
    GPST = UTC + GPS_LS
    """
    t = time.Time(gpst_sec.iloc[0], format="unix", scale="tai")
    gpst_utc_ls = (t.unix_tai - t.unix) - 19
    t_corrected = time.Time(t.unix - gpst_utc_ls, format='unix')
    print(f'GPST to UTC offset {gpst_utc_ls}')
    print(f'TAI from GPST {t.iso}')
    print(f'UTC corrected from GPST {t_corrected.iso}')
    t_final = time.Time(gpst_sec - gpst_utc_ls, format='unix')
    return t_final.unix


def read_ublox_pos(path: str) -> pd.DataFrame:
    header = ['GPST', 'lat_deg', 'long_deg', 'height_m', 'Q', 'ns', 'sdn_m', 'sde_m', 'sdu_m', 'sdne_m', 'sdeu_m',
              'sdun_m', 'age_s', 'ratio']

    positions = pd.read_csv(filepath_or_buffer=path, delimiter=r"\s\s+", comment='%', header=None, names=header,
                            parse_dates=['GPST'],
                            date_parser=lambda x: pd.to_datetime(x, format="%Y/%m/%d %H:%M:%S.%f"),
                            engine='python')
    positions['GPST_Seconds'] = positions['GPST'].astype('int64') // 1e9
    return positions


def add_time_col(time_func, df: pd.DataFrame, col_name: str, base_col: str):
    if base_col not in df.columns:
        raise NameError(f"Column name {base_col} does not exist in the provided df!")
    df[col_name] = time_func(df[base_col])


def read_interaction(path: str) -> pd.DataFrame:
    interactions = pd.read_csv(path, parse_dates=['UtcDateTime'])
    interactions['UTC_Seconds'] = interactions['UtcDateTime'].astype('int64') // 1e9
    return interactions


def meters_to_degrees(meters, latitude):
    """
    Approximation of meters based on positions latitude
    :param meters:
    :param latitude: current pos latitude
    :return: Meters in degree
    """
    # Radius of the Earth in meters
    radius = 6371000
    # Calculate change in latitude
    lat_change = meters / (radius * math.pi / 180)
    # Calculate change in longitude
    lon_change = meters / (radius * math.pi / 180 * math.cos(latitude * math.pi / 180))
    print(lat_change, lon_change)
    return lat_change, lon_change


def calc_parallel_c(a: tuple, b: tuple, distance: float):
    ab = shp.LineString([a, b])
    parallel = ab.offset_curve(distance=distance, quad_segs=32)
    return parallel.coords[0], parallel.coords[1]


def construct_parallel_polygon(a: tuple, b: tuple, height: float) -> shp.Polygon:
    c, d = calc_parallel_c(a, b, -1 * height)
    polygon = shp.Polygon([a, c, d, b])
    return polygon
