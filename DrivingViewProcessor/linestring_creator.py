import pandas as pd
import argparse
import geopandas as gpd
import shapely as shp
from utils import read_ublox_pos, read_interaction, add_time_col, gpst_leapseconds, Field

parser = argparse.ArgumentParser(description='Read and Process interaction und ublox data.')
parser.add_argument("--pos", type=str, nargs=1)
parser.add_argument("--ilog", type=str, nargs=1)
parser.add_argument("--filename", type=str, nargs=1)

_merge_rcol = "UTC_corrected"
_merge_lcol = "UTC_Seconds"
_position_time = "GPST_Seconds"
# https://epsg.io/3043
_base_epsg = 4326
_calc_epsg = 3043

_lane_config = {
    1: 6,
    2: 12,
    3: 18,
    4: -6,
    5: -12,
    6: -18,
}
_lane_width = 6


def write_f(field: Field,
            file_prefix: str,
            output_name: str,
            no_shift: bool,
            geom_type: shp.Geometry):
    with open(f"{file_prefix}_{output_name}", 'w') as f:
        out = field.to_gpd(no_shift=no_shift, geom_type=geom_type)
        f.write(out.to_json())


def prepare_and_merge(pos_df: pd.DataFrame, int_df: pd.DataFrame, ) -> gpd.GeoDataFrame:
    add_time_col(time_func=gpst_leapseconds,
                 df=pos_df,
                 col_name=_merge_rcol,
                 base_col=_position_time)
    interactions_df = int_df.sort_values(by=_merge_lcol)  # only needed for the test data set
    # merge interactions_df and position
    interactions_df = pd.merge_asof(interactions_df, pos_df, left_on=_merge_lcol, right_on=_merge_rcol)
    geometry = gpd.points_from_xy(interactions_df["long_deg"], interactions_df['lat_deg'], crs=f"EPSG:{_base_epsg}")
    geometry = geometry.to_crs(crs=f"EPSG:{_calc_epsg}")
    return gpd.GeoDataFrame(data=interactions_df, geometry=geometry)


_exec_config = {
    "drive_line": lambda field, f_name: write_f(field=field, file_prefix="drive_line", no_shift=True, geom_type=shp.LineString, output_name=f_name),
    "drive_points": lambda field, f_name: write_f(field=field, file_prefix="drive_points", no_shift=True, geom_type=shp.Point, output_name=f_name),
    "drive_polygon": lambda field, f_name: write_f(field=field, file_prefix="drive_polygon", no_shift=True, geom_type=shp.Polygon, output_name=f_name),
    "lane_polygon": lambda field, f_name: write_f(field=field, file_prefix="polygon", no_shift=False, geom_type=shp.Polygon, output_name=f_name),
    "lane_line": lambda field, f_name: write_f(field=field, file_prefix="line", no_shift=False, geom_type=shp.LineString, output_name=f_name),
    "lane_points": lambda field, f_name: write_f(field=field, file_prefix="points", no_shift=False, geom_type=shp.Point, output_name=f_name),
}


def main(positions_df: pd.DataFrame, interactions_df: pd.DataFrame, f_name: str, exec_list: [str]):
    merged = prepare_and_merge(positions_df, interactions_df)
    # collect all lanes
    field = Field.from_gpd(df=merged,
                           target_epsg=_base_epsg,
                           lane_config=_lane_config,
                           lane_width=_lane_width)
    for e in exec_list:
        _exec_config[e](field, f_name)


if __name__ == "__main__":
    args = parser.parse_args()
    positions = read_ublox_pos(args.pos[0])
    interactions = read_interaction(args.ilog[0])
    file_name = args.filename[0]
    main(positions, interactions, file_name, ["lane_polygon"])
