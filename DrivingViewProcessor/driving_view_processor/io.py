import pandas as pd
import geopandas as gpd
import shapely as shp
from .core import Field
from .utils import add_time_col, gpst_leapseconds


def write_f(field: Field,
            file_prefix: str,
            output_name: str,
            out_folder: str,
            no_shift: bool,
            geom_type: shp.Geometry):
    with open(f"{out_folder}/{file_prefix}_{output_name}", 'w') as f:
        out = field.to_gpd(no_shift=no_shift, geom_type=geom_type)
        f.write(out.to_json())


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
