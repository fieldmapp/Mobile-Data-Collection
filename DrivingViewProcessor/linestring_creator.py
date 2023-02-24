from utils import read_ublox_pos, read_interaction, add_time_col, gpst_leapseconds, gpst_to_utc
import pandas as pd
import argparse
from typing import Dict
import geojson

parser = argparse.ArgumentParser(description='Read and Process interaction und ublox data.')
parser.add_argument("--pos", type=str, nargs=1)
parser.add_argument("--ilog", type=str, nargs=1)

_merge_rcol = "UTC_corrected"
_merge_lcol = "UTC_Seconds"
_position_time = "GPST_Seconds"

if __name__ == "__main__":
    args = parser.parse_args()
    print(args)

    positions = read_ublox_pos(args.pos[0])
    interactions = read_interaction(args.ilog[0])
    # add time columns, atm. both ways of calc.
    add_time_col(time_func=gpst_leapseconds, df=positions, col_name=_merge_rcol, base_col=_position_time)
    add_time_col(time_func=gpst_to_utc, df=positions, col_name=_merge_lcol, base_col=_position_time)
    # merge interactions and position
    interactions_new = pd.merge_asof(interactions, positions, on=_merge_lcol)
    interactions_alt = pd.merge_asof(interactions, positions, left_on=_merge_lcol, right_on=_merge_rcol)
    assert(interactions.shape[0] == interactions.shape[0])
    """


    class LaneData():
        start: geojson.Point
        end: geojson.Point
        cause: str
        type: str


    lanes: Dict[int, LaneData] = dict()

    for interaction in interactions.itertuples(False):
        if interaction.Action.startswith('Miss'):
            continue
        print(interaction)
        if interaction.Action == 'canceled':
            lanes = dict()
        elif interaction.Action == 'open':
            if interaction.LaneIndex in lanes:
                # finalize lane
                pass

            lanes[interaction.LaneIndex] = LaneData()
            lanes[interaction.LaneIndex].start = interaction.
        elif interaction.Action == 'close':
            pass
        elif interaction.Action.startswith('damage='):
            pass
        elif interaction.Action.startswith('cause='):
            pass
        else:
            raise NotImplementedError(
                f'Unexpected interaction action encountered: {interaction.Action} at lane {interaction.LaneIndex}')
        """