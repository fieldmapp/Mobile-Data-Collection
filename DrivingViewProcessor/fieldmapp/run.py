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
    DvpExecTypes.DRIVE_LINE: lambda field, f_name, out_f: write_f(field=field,
                                                                  file_prefix=DvpExecTypes.DRIVE_LINE.value,
                                                                  no_shift=True,
                                                                  geom_type=shp.LineString, output_name=f_name,
                                                                  out_folder=out_f),
    DvpExecTypes.DRIVE_POINT: lambda field, f_name, out_f: write_f(field=field,
                                                                   file_prefix=DvpExecTypes.DRIVE_POINT.value,
                                                                   no_shift=True,
                                                                   geom_type=shp.Point, output_name=f_name,
                                                                   out_folder=out_f),
    DvpExecTypes.DRIVE_POLY: lambda field, f_name, out_f: write_f(field=field,
                                                                  file_prefix=DvpExecTypes.DRIVE_POLY.value,
                                                                  no_shift=True,
                                                                  geom_type=shp.Polygon, output_name=f_name,
                                                                  out_folder=out_f),
    DvpExecTypes.LANE_POLY: lambda field, f_name, out_f: write_f(field=field, file_prefix=DvpExecTypes.LANE_POLY.value,
                                                                 no_shift=False,
                                                                 geom_type=shp.Polygon, output_name=f_name,
                                                                 out_folder=out_f),
    DvpExecTypes.LANE_LINE: lambda field, f_name, out_f: write_f(field=field, file_prefix=DvpExecTypes.LANE_LINE.value,
                                                                 no_shift=False,
                                                                 geom_type=shp.LineString, output_name=f_name,
                                                                 out_folder=out_f),
    DvpExecTypes.LANE_POINT: lambda field, f_name, out_f: write_f(field=field,
                                                                  file_prefix=DvpExecTypes.LANE_POINT.value,
                                                                  no_shift=False,
                                                                  geom_type=shp.Point, output_name=f_name,
                                                                  out_folder=out_f),
}


def run_dvp(positions_file: str,
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
    finished_list = []
    for e in exec_list:
        if e not in _exec_config.keys():
            raise NameError(f"Config name {e} not available as execution config!")
        finished_list.append(_exec_config[e](field, f_name, output_dir))

    for i in finished_list:
        print(f"wrote file: {i}")
    return finished_list
