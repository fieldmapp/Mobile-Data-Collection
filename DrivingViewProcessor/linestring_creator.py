import pandas as pd
import argparse
import shapely as shp
from driving_view_processor.core import Field
from driving_view_processor.utils import read_ublox_pos, read_interaction
from driving_view_processor.io import prepare_and_merge, write_f


parser = argparse.ArgumentParser(description='Read and Process interaction und ublox data.')
parser.add_argument("--pos", type=str, nargs=1)
parser.add_argument("--ilog", type=str, nargs=1)
parser.add_argument("--filename", type=str, nargs=1)
parser.add_argument("--outdir", type=str, nargs=1)

merge_col = "UTC_Seconds"
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

_exec_config = {
    "drive_line": lambda field, f_name, out_f: write_f(field=field, file_prefix="drive_line", no_shift=True, geom_type=shp.LineString, output_name=f_name, out_folder=out_f),
    "drive_points": lambda field, f_name, out_f: write_f(field=field, file_prefix="drive_points", no_shift=True, geom_type=shp.Point, output_name=f_name, out_folder=out_f),
    "drive_polygon": lambda field, f_name, out_f: write_f(field=field, file_prefix="drive_polygon", no_shift=True, geom_type=shp.Polygon, output_name=f_name, out_folder=out_f),
    "lane_polygon": lambda field, f_name, out_f: write_f(field=field, file_prefix="polygon", no_shift=False, geom_type=shp.Polygon, output_name=f_name, out_folder=out_f),
    "lane_line": lambda field, f_name, out_f: write_f(field=field, file_prefix="line", no_shift=False, geom_type=shp.LineString, output_name=f_name, out_folder=out_f),
    "lane_points": lambda field, f_name, out_f: write_f(field=field, file_prefix="points", no_shift=False, geom_type=shp.Point, output_name=f_name, out_folder=out_f),
}


def main(positions_df: pd.DataFrame, interactions_df: pd.DataFrame, f_name: str, output_dir: str, exec_list: [str]):
    merged = prepare_and_merge(pos_df=positions_df, interaction_df=interactions_df,
                               merge_col=merge_col, pos_tim_col=_position_time,
                               base_epsg=_base_epsg, calc_epsg=_calc_epsg)
    # collect all lanes
    field = Field.from_gpd(df=merged,
                           target_epsg=_base_epsg,
                           lane_config=_lane_config,
                           lane_width=_lane_width)
    for e in exec_list:
        if e in _exec_config:
            _exec_config[e](field, f_name, output_dir)


if __name__ == "__main__":
    args = parser.parse_args()
    positions = read_ublox_pos(args.pos[0])
    interactions = read_interaction(args.ilog[0])
    file_name = args.filename[0]
    output_folder = args.outdir[0]
    main(positions, interactions, file_name, output_folder, ["lane_polygon"])
