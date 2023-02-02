import pyglet
from time import time, sleep
from typing import Union, Tuple, List, Dict
import ctypes
import os
from matplotlib.image import imread
import numpy as np
import numpy.typing
import shutil

abspath = os.path.abspath(__file__)
dname = os.path.dirname(abspath)
os.chdir(dname)

if os.path.isdir('frames'):
    shutil.rmtree('frames')
os.makedirs('frames')


#ctypes.windll.user32.SetProcessDPIAware()

TARGET_FRAMERATE = 100
WINDOW_WIDTH = 800
WINDOW_HEIGHT = 800

INITIAL_TRACTOR_Y = 160
TRACTOR_WIDTH = 560
SAMPLE_OFFSET = 1

window = pyglet.window.Window(style=pyglet.window.Window.WINDOW_STYLE_BORDERLESS)
window.set_size(window.width, window.height)
window.width = WINDOW_WIDTH
window.height = WINDOW_HEIGHT

def get_texture(original : Union[pyglet.sprite.Sprite, pyglet.image.Texture]) -> pyglet.image.Texture:
    if type(original) is pyglet.image.Texture:
        return original
    elif type(original) is pyglet.sprite.Sprite:
        if type(original.image) is pyglet.image.Texture:
            return original.image
        else:
            raise Exception
    else:
        raise Exception

def get_box(original : Union[pyglet.sprite.Sprite, pyglet.image.Texture], width : Union[int, None] = None, height : Union[int, None] = None) -> Tuple[int, int]:
    texture = get_texture(original)

    if width is None and height is None:
        width = int(texture.width)
        height = int(texture.height)
    if width is None:
        width = int(texture.width / texture.height * height)
    if height is None:
        height = int(texture.height / texture.width * width)
    
    return (width, height)

def get_scale(original : Union[pyglet.sprite.Sprite, pyglet.image.Texture], target_width : Union[int, None] = None, target_height : Union[int, None] = None) -> float:
    texture = get_texture(original)

    scale = float('inf')
    if target_width is not None:
        scale = min(scale, target_width/texture.width)
    if target_height is not None:
        scale = min(scale, target_height/texture.height)
    if scale == float('inf'):
        scale = 1
    return scale

def find_runs(vals : np.ndarray, val):
    # altered from https://stackoverflow.com/a/24892274
    # Create an array that is 1 where a is 0, and pad each end with an extra 0.
    iszero = np.concatenate(([0], np.equal(vals, val).view(np.int8), [0]))
    absdiff = np.abs(np.diff(iszero))
    # Runs start and end where absdiff is 1.
    ranges = np.where(absdiff == 1)[0].reshape(-1, 2)
    return ranges

def lerp(v1 : float, v2 : float, x : numpy.typing._ArrayLikeFloat_co):
    return np.interp(x, [0, 1], [v1, v2])

tractor : pyglet.sprite.Sprite = pyglet.sprite.Sprite(pyglet.resource.image('img/tractor.png'))
ground : pyglet.sprite.Sprite = pyglet.sprite.Sprite(pyglet.resource.image('img/ground.png'))
ground.scale = get_scale(ground, window.width)
tractor.scale = get_scale(tractor, TRACTOR_WIDTH)
tractor.position = (window.width / 2 - tractor.width / 2, INITIAL_TRACTOR_Y, 0)

frame_count : int = 0
cam_pos = pyglet.math.Vec3(0, 0, 0)
lane_batch = pyglet.graphics.Batch()
lane_history : List[pyglet.shapes.BorderedRectangle] = list()
active_lanes : Dict[int, pyglet.shapes.BorderedRectangle] = dict()
lane_bounds : np.ndarray = np.array((), ndmin=2)
lane_count : int = -1
@window.event
def on_draw():
    global frame_count, lane_bounds, lane_count
    cam_pos.y -= 1
    tractor.position = (tractor.position[0], tractor.position[1] + 1, 0)
    
    window.clear()
    window.view = window.view.from_translation(cam_pos)
    ground.draw()
    tractor.draw()
    lane_batch.draw()


    pyglet.image.get_buffer_manager().get_color_buffer().save(f'frames/frame{str(frame_count).zfill(4)}.png')
    frame = imread(f'frames/frame{str(frame_count).zfill(4)}.png')

    if frame_count == 0:
        bottom_row_red = frame[-1, :, 0]
        lane_bounds = find_runs(bottom_row_red, 1)
        lane_count = lane_bounds.shape[0]

    
    for i in range(lane_count):
        sample_y = INITIAL_TRACTOR_Y - SAMPLE_OFFSET + 3
        
        sample_x = lerp(lane_bounds[i][0], lane_bounds[i][1], [0, 0.2, 0.4, 0.6, 0.8, 1]).astype(int)
        sample_values = frame[-sample_y, sample_x][:, 1]
        
        area_detected = np.any(sample_values > 0.7)
        if area_detected:
            lane = active_lanes.get(i, None)
            if lane is None:
                lane = pyglet.shapes.BorderedRectangle(lane_bounds[i][0], 
                    tractor.position[1] - SAMPLE_OFFSET, lane_bounds[i][1] - lane_bounds[i][0], 1, border = 2, 
                    batch=lane_batch, color = (144, 238, 144, 180), border_color = (144, 238, 144))
                lane_history.append(lane)
                active_lanes[i] = lane
            lane.height += 1
        elif i in active_lanes:
            active_lanes.pop(i)
            
    
    frame_count += 1

pyglet.app.run(1/TARGET_FRAMERATE)

# keep frame 0153 - 0735