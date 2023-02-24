import datetime

import pandas as pd
from astropy import time
def gpst_to_utc(gpst_seconds: pd.Series) -> int:
    # GPST = TAI - 19s (constant)
    t_tai = time.Time(gpst_seconds + 19, format='unix_tai', scale='tai')
    t_utc = time.Time(t_tai, format='unix', scale='utc')
    return t_utc.to_value('unix')


def gpst_leapseconds(gpst_sec: pd.Series) -> int:
    """
    The GPS time scale began on January 6, 1980.  At that time, the UTC timescale had undergone 19 leap second events (TAI-UTC).
    so we need to 19s substract 19s from the GPST time to get the correct UTC-Timestamp.

    https://raw.githubusercontent.com/tomojitakasu/RTKLIB/rtklib_2.4.3/doc/manual_2.4.2.pdf page 131, 31
    astro py epoch https://docs.astropy.org/en/stable/time/index.html#time-from-epoch-formats
    TAI = UTC + LS
    GPS_LS = LS + 19 , GPS since 1980, 19 UTC had 19 LS since then
    UTC = GPS - GPS_LS
    GPST = UTC + GPS_LS
    """
    t = time.Time(gpst_sec.iloc[0], format="unix", scale="tai")
    gpst_utc_ls = (t.unix_tai - t.unix) - 19
    t_corrected = time.Time(t.unix - gpst_utc_ls, format='unix')
    print(f'GPST to UTC offset { gpst_utc_ls }')
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
