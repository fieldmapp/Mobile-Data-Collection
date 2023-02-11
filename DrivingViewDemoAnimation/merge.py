import pyglet
from time import time, sleep
from typing import Union, Tuple, List, Dict
import ctypes
import os
from matplotlib.image import imread
import numpy as np
import numpy.typing
import shutil
import os.path

abspath = os.path.abspath(__file__)
dname = os.path.dirname(abspath)
os.chdir(dname)

if os.path.isdir('merged'):
    shutil.rmtree('merged')
os.makedirs('merged')

WINDOW_HEIGHT = 1036

IN0_WIDTH = 1036
IN1_WIDTH = 1728

WINDOW_WIDTH = IN0_WIDTH + IN1_WIDTH

FRAME_COUNT = 583


window = pyglet.window.Window(style=pyglet.window.Window.WINDOW_STYLE_BORDERLESS)
window.width = WINDOW_WIDTH
window.height = WINDOW_HEIGHT

part0 : pyglet.image.Texture = pyglet.resource.image('in0/0001.png')
part1 : pyglet.image.Texture = pyglet.resource.image('in1/0001.png')

frame_count = 0
@window.event
def on_draw():
    global frame_count, part0, part1
    frame_count += 1

    window.clear()
    part0.blit(0, 0)
    part1.blit(IN0_WIDTH, 0)
    pyglet.image.get_buffer_manager().get_color_buffer().save(f'merged/{str(frame_count).zfill(4)}.png')

    part0 = pyglet.resource.image(f'in0/{str(frame_count).zfill(4)}.png')
    if os.path.isfile(f'in1/{str(frame_count).zfill(4)}.png'):
        part1 = pyglet.resource.image(f'in1/{str(frame_count).zfill(4)}.png')



pyglet.app.run(1/10)
