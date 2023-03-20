import shapely as shp
from enum import Enum
from typing import List
from .core import Field
from .io import write_f, prepare_and_merge, read_interaction, read_ublox_pos


class DvpExecTypes(Enum):
    DRIVE_LINE = "drive_line"
    DRIVE_POINT = "drive_point"
    DRIVE_POLY = "drive_polygon"
    LANE_LINE = "lane_line"
    LANE_POINT = "lane_point"
    LANE_POLY = "lane_polygon"


_exec_config = {
    DvpExecTypes.DRIVE_LINE: lambda field: field.to_gpd(no_shift=True, geom_type=shp.LineString),
    DvpExecTypes.DRIVE_POINT: lambda field: field.to_gpd(no_shift=True, geom_type=shp.Point),
    DvpExecTypes.DRIVE_POLY: lambda field: field.to_gpd(no_shift=True, geom_type=shp.Polygon),
    DvpExecTypes.LANE_POLY: lambda field: field.to_gpd(no_shift=False, geom_type=shp.Polygon),
    DvpExecTypes.LANE_LINE: lambda field: field.to_gpd(no_shift=False, geom_type=shp.LineString),
    DvpExecTypes.LANE_POINT: lambda field: field.to_gpd(no_shift=False, geom_type=shp.Point)
}


def run_dvp(positions_file: str,
            interactions_file: str,
            merge_col: str,
            pos_time: str,
            base_epsg: int,
            calc_epsg: int,
            lane_config: dict,
            lane_width: float,
            exec_list: List[DvpExecTypes]) -> dict:
    # read files
    positions = read_ublox_pos(positions_file)
    interactions = read_interaction(interactions_file)
    # merge dfs
    merged = prepare_and_merge(pos_df=positions, interaction_df=interactions,
                               merge_col=merge_col, pos_tim_col=pos_time,
                               base_epsg=base_epsg, calc_epsg=calc_epsg)
    # collect all lanes
    field = Field.from_gpd(df=merged,
                           target_epsg=base_epsg,
                           lane_config=lane_config,
                           lane_width=lane_width)
    # exec write to geojson
    finished = {}
    for e in exec_list:
        if e not in _exec_config.keys():
            raise NameError(f"Config name {e} not available as execution config!")
        finished[e] = _exec_config[e](field).to_json()
    return finished


def run_dvp_to_file(
            positions_file: str,
            interactions_file: str,
            merge_col: str,
            pos_time: str,
            base_epsg: int,
            calc_epsg: int,
            lane_config: dict,
            lane_width: float,
            f_name: str,
            output_dir: str,
            exec_list: List[DvpExecTypes]) -> List[str]:

    features = run_dvp(
            positions_file=positions_file,
            interactions_file=interactions_file,
            lane_width=lane_width,
            lane_config=lane_config,
            base_epsg=base_epsg,
            calc_epsg=calc_epsg,
            pos_time=pos_time,
            merge_col=merge_col,
            exec_list=exec_list)
    # exec write to geojson
    finished_list = []
    for k, v in features.items():
        finished_list.append(write_f(feature=v,
                                     output_name=f_name,
                                     out_folder=output_dir,
                                     file_prefix=k.value))
    for i in finished_list:
        print(f"wrote file: {i}")
    return finished_list
