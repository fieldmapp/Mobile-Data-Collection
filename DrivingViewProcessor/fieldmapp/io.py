import pandas as pd
import geopandas as gpd
from .utils import add_time_col, gpst_leapseconds


def write_f(feature: str,
            file_prefix: str,
            output_name: str,
            out_folder: str):
    with open(f"{out_folder}/{file_prefix}_{output_name}", 'w') as f:
        f.write(feature)
        return f"{out_folder}/{file_prefix}_{output_name}"


def prepare_and_merge(pos_df: pd.DataFrame,
                      interaction_df: pd.DataFrame,
                      pos_tim_col: str,
                      merge_col: str,
                      base_epsg: int,
                      calc_epsg: int
                      ) -> gpd.GeoDataFrame:
    add_time_col(time_func=gpst_leapseconds,
                 df=pos_df,
                 col_name=merge_col,
                 base_col=pos_tim_col)
    interactions_df = interaction_df.sort_values(by=merge_col)  # only needed for the test data set
    # merge interactions_df and position
    interactions_df = pd.merge_asof(interactions_df, pos_df, on=merge_col)
    geometry = gpd.points_from_xy(interactions_df["long_deg"], interactions_df['lat_deg'], crs=f"EPSG:{base_epsg}")
    geometry = geometry.to_crs(crs=f"EPSG:{calc_epsg}")
    return gpd.GeoDataFrame(data=interactions_df, geometry=geometry)


def read_interaction(path: str) -> pd.DataFrame:
    interactions = pd.read_csv(path, parse_dates=['UtcDateTime'])
    interactions['UTC_Seconds'] = interactions['UtcDateTime'].astype('int64') // 1e9
    return interactions


def read_ublox_pos(path: str) -> pd.DataFrame:
    header = ['GPST', 'lat_deg', 'long_deg', 'height_m', 'Q', 'ns', 'sdn_m', 'sde_m', 'sdu_m', 'sdne_m', 'sdeu_m',
              'sdun_m', 'age_s', 'ratio']

    positions = pd.read_csv(filepath_or_buffer=path, delimiter=r"\s\s+", comment='%', header=None, names=header,
                            parse_dates=['GPST'],
                            date_parser=lambda x: pd.to_datetime(x, format="%Y/%m/%d %H:%M:%S.%f"),
                            engine='python')
    positions['GPST_Seconds'] = positions['GPST'].astype('int64') // 1e9
    return positions