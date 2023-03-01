import geojson
import pandas as pd
import argparse
import geopandas as gpd
from utils import read_ublox_pos, read_interaction, add_time_col, gpst_leapseconds, LaneSegment, LaneDetail
from typing import Dict, List
from geojson import FeatureCollection
from itertools import chain

parser = argparse.ArgumentParser(description='Read and Process interaction und ublox data.')
parser.add_argument("--pos", type=str, nargs=1)
parser.add_argument("--ilog", type=str, nargs=1)
parser.add_argument("--filename", type=str, nargs=1)

_merge_rcol = "UTC_corrected"
_merge_lcol = "UTC_Seconds"
_position_time = "GPST_Seconds"
_lane_config = {
    1: 5,
    2: 10,
    3: 15,
    4: -5,
    5: -10,
    6: -15,
}


def lanes_from_df(df: gpd.GeoDataFrame, lane_config: dict) -> dict:
    all_lanes: Dict[int, List[LaneSegment]] = dict()
    for k, v in lane_config.items():
        interactions_filtered: pd.DataFrame = df.loc[df["LaneIndex"] == k]
        segment = None
        lanes: [LaneSegment] = []
        for interaction in interactions_filtered.itertuples(False):
            detail = LaneDetail.from_pd(interaction)
            if detail.ltype == 'open':
                segment = LaneSegment(lane_index=interaction.LaneIndex, start=detail.utc_time)
                segment.actions = [detail]
                lanes.append(segment)
            elif detail.ltype == 'close':
                segment.end = detail.utc_time
                segment.actions.append(detail)
            else:
                segment.actions.append(detail)
        all_lanes[k] = lanes
    return all_lanes


def main(positions_df: pd.DataFrame, interactions_df: pd.DataFrame, file_name: str):
    # add time columns, atm. both ways of calc.
    add_time_col(time_func=gpst_leapseconds, df=positions_df, col_name=_merge_rcol, base_col=_position_time)
    interactions_df = interactions_df.sort_values(by=_merge_lcol)  # only needed for the test data set
    # merge interactions_df and position
    interactions_df = pd.merge_asof(interactions_df, positions_df, left_on=_merge_lcol, right_on=_merge_rcol)
    # is the geopandas df really nes. here?
    geometry = gpd.points_from_xy(interactions_df["lat_deg"], interactions_df['long_deg'], crs="EPSG:4326")
    interactions_df_gdf = gpd.GeoDataFrame(data=interactions_df, geometry=geometry)
    # collect all lanes
    result_lanes = lanes_from_df(interactions_df_gdf, _lane_config)
    # lanes to geojson
    features = [[e.to_geojson_feature(_lane_config) for e in v] for k, v in result_lanes.items()]
    feature_list = list(chain.from_iterable(features))
    f_list = [e for l in feature_list for e in l]  # unpack tuples
    collection = FeatureCollection(features=f_list)

    # write to file
    out = geojson.dumps(collection, default=lambda x: x.__dict__)
    with open(file_name, 'w') as f:
        f.write(out)
    print("done")


if __name__ == "__main__":
    args = parser.parse_args()
    positions = read_ublox_pos(args.pos[0])
    interactions = read_interaction(args.ilog[0])
    file_name = args.filename[0]
    main(positions, interactions, file_name)

