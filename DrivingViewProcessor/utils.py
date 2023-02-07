import pandas as pd
from datetime import datetime
from astropy import time

def gpst_to_utc(gpst_seconds) -> int:
    # GPST = TAI - 19s (constant)
    t = time.Time(gpst_seconds + 19, format='unix_tai', scale='tai')
    t = time.Time(t, format='unix', scale='utc')
    
    return t.to_value('unix')


def read_ublox_pos(path : str) -> pd.DataFrame:
    header = ['GPST', 'lat_deg', 'long_deg', 'height_m', 'Q', 'ns', 'sdn_m', 'sde_m', 'sdu_m', 'sdne_m', 'sdeu_m', 'sdun_m', 'age_s', 'ratio']
    custom_date_parser = lambda x: datetime.strptime(x, "%Y/%m/%d %H:%M:%S.%f")
    positions = pd.read_csv(path, delimiter=r"\s\s+", comment='%', header=None, names=header, parse_dates=['GPST'], date_parser=custom_date_parser, engine = 'python')
    positions['GPST Seconds'] = positions['GPST'].astype('int64')//1e9
    positions['UTC Seconds'] =  gpst_to_utc(positions['GPST Seconds'])

    return positions

def read_interaction(path : str) -> pd.DataFrame:
    interactions = pd.read_csv(path, parse_dates=['UtcDateTime'])
    interactions['UTC Seconds'] = interactions['UtcDateTime'].astype('int64')//1e9

    return interactions



