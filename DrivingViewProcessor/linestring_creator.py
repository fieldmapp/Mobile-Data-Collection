import argparse
from fieldmapp.run import run_dvp_to_file, DvpExecTypes


parser = argparse.ArgumentParser(description='Read and Process interaction und ublox data.')
parser.add_argument("--pos", type=str, nargs=1)
parser.add_argument("--ilog", type=str, nargs=1)
parser.add_argument("--filename", type=str, nargs=1)
parser.add_argument("--outdir", type=str, nargs=1)

_merge_col = "UTC_Seconds"
_position_time = "GPST_Seconds"
# https://epsg.io/5653
_base_epsg = 4326
_calc_epsg = 5653

_lane_config = {
    1: 6,
    2: 12,
    3: 18,
    4: -6,
    5: -12,
    6: -18
}

_lane_width = 6

if __name__ == "__main__":
    args = parser.parse_args()
    positions = args.pos[0]
    interactions = args.ilog[0]
    file_name = args.filename[0]
    output_folder = args.outdir[0]
    run_dvp_to_file(
            positions_file=positions,
            interactions_file=interactions,
            merge_col=_merge_col,
            pos_time=_position_time,
            calc_epsg=_calc_epsg,
            base_epsg=_base_epsg,
            lane_config=_lane_config,
            lane_width=_lane_width,
            f_name=file_name,
            output_dir=output_folder,
            exec_list=[DvpExecTypes.LANE_POLY, DvpExecTypes.LANE_POINT, DvpExecTypes.LANE_LINE, DvpExecTypes.DRIVE_LINE,
                       DvpExecTypes.DRIVE_POINT, DvpExecTypes.DRIVE_POLY])
