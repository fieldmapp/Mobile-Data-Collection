import pandas as pd
from astropy import time
from geojson import Feature, MultiPoint, MultiLineString, LineString
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
    start: int
    end: int
    actions: [LaneDetail]
    lane: int

    def __init__(self, lane_index: int, start: int):
        self.lane = lane_index
        self.start = start
        self.actions = []

    def to_props(self):
        res = self.__dict__
        res = self.get_cause(res)
        return res

    def get_cause(self, res: dict):
        for i in self.actions:
            if "=" in i.ltype:
                split = i.ltype.split("=")
                res[split[0]] = split[1]
        return res

    def to_geojson_feature(self, lane_config: dict) -> Feature:
        """
        :param lane_config: Configuration of the field lanes: number and width
        :return: GeoJson Feature with the calculated parallel Lines representing the FieldLanes
                as well as a Multipoint feature representing the GPS-Track
        """
        open_node = next(x for x in self.actions if x.ltype == "open")
        close_node = next(x for x in self.actions if x.ltype == "close")

        if open_node is None:
            raise ValueError(f"No open node found for {self}")
        if close_node is None:
            raise ValueError(f"No close node found for {self}")

        a, b = calc_parallel_c((open_node.lon, open_node.lat), (close_node.lon, close_node.lat),
                               meters_to_degrees(lane_config[self.lane], (open_node.lat+close_node.lat)/2)[1])
        # constructed parallel shifted line
        line_shifted = LineString([a, b], precision=15)
        raw_points = MultiPoint([(open_node.lon, open_node.lat), (close_node.lon, close_node.lat)], precision=15)

        multi_lines = MultiLineString([line_shifted], precision=15)
        line_prop = {
            "start": self.start,
            "lane": self.lane
        }

        line_feature = Feature(geometry=multi_lines, properties=self.get_cause(line_prop))
        raw_point_feature = Feature(geometry=raw_points, properties=self.get_cause(line_prop))

        return line_feature, raw_point_feature


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
    #print(lat_change, lon_change)
    return lat_change, lon_change


def calc_parallel_c(a, b, distance):
    ab = shp.LineString([a, b])
    parallel = ab.offset_curve(distance=distance,quad_segs=32)
    return parallel.coords[0], parallel.coords[1]
