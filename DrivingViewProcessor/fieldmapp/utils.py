import pandas as pd
from astropy import time
import math
import shapely.geometry as shp


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


def add_time_col(time_func, df: pd.DataFrame, col_name: str, base_col: str):
    if base_col not in df.columns:
        raise NameError(f"Column name {base_col} does not exist in the provided df!")
    df[col_name] = time_func(df[base_col])


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
