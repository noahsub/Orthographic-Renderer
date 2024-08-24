########################################################################################################################
# render.py
#
# This script is used to render a view of the scene with the specified parameters. It supports rendering with both
# Cycles and Eevee render engines. It takes GPU preference over CPU if both are available.
#
# Author: https://github.com/noahsub
########################################################################################################################

########################################################################################################################
# IMPORTS
########################################################################################################################
import datetime
import math
import argparse
import os
from enum import Enum
import sys
import bpy
from mathutils import Vector


########################################################################################################################
# ARGUMENT PARSING
########################################################################################################################
class BlenderArgparse(argparse.ArgumentParser):
    """
    A modified version of the argparse.ArgumentParser class that allows for parsing of arguments from the command line
    whilst ignoring blender specific arguments.
    """

    def parse_args(self):
        arguments = []
        if "--" in sys.argv:
            arguments = sys.argv[sys.argv.index("--") + 1:]
        return super().parse_args(args=arguments)


########################################################################################################################
# RENDER DEVICE ENUM
########################################################################################################################
class RenderDevice(Enum):
    """
    An enum of the different types of rendering devices that can be used.
    """
    OPTIX = "OPTIX"
    CUDA = "CUDA"
    EEVEE = "EEVEE"
    UNSUPPORTED = "UNSUPPORTED"


########################################################################################################################
# SCALE ENUM
########################################################################################################################
class Unit(Enum):
    """
    An enum of the different types of units that can be used and their corresponding scales in relation to meters.
    """
    MILLIMETERS = 0.001
    CENTIMETERS = 0.01
    METERS = 1
    INCHES = 0.0254
    FEET = 0.3048


########################################################################################################################
# POSITION CLASS
########################################################################################################################
class Position:
    """
    A class to represent the position and rotation of an object in the scene.
    """
    # The x, y, and z coordinates of the object
    x: float
    y: float
    z: float
    # The x, y, and z rotations of the object
    rx: float
    ry: float
    rz: float

    def __init__(self, x: float, y: float, z: float, rx: float, ry: float, rz: float):
        self.x = x
        self.y = y
        self.z = z
        self.rx = rx
        self.ry = ry
        self.rz = rz

    def __str__(self):
        return f"X.{self.x}-Y.{self.y}-Z.{self.z}-RX.{self.rx}-RY.{self.ry}-RZ.{self.rz}"


########################################################################################################################
# MODEL FUNCTIONS
########################################################################################################################

def import_model(model_path: str, unit: Unit) -> None:
    directory = os.path.dirname(model_path)
    file_name = os.path.basename(model_path)
    name = os.path.splitext(file_name)[0]
    print(model_path)
    print(directory)
    print(file_name)
    print(name)

    bpy.ops.wm.obj_import(filepath=model_path, directory=directory, files=[{"name": file_name, "name": file_name}],
                          global_scale=unit.value, forward_axis='Y', up_axis='Z')

    try:
        bpy.data.objects.remove(bpy.data.objects["Cube"], do_unlink=True)
    except:
        pass

    bpy.ops.object.select_all(action='DESELECT')
    bpy.data.objects[name].select_set(True)
    bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_VOLUME', center='MEDIAN')
    bpy.data.objects[name].location = (0, 0, 0)

    # save the blend file
    bpy.ops.wm.save_as_mainfile(filepath="C:/Users/noahs/Downloads/model.blend")


########################################################################################################################
# RENDERING FUNCTIONS
########################################################################################################################

def set_render_preferences() -> RenderDevice:
    render_device = None
    preferences = bpy.context.preferences
    cycles_preferences = preferences.addons["cycles"].preferences
    cycles_preferences.refresh_devices()
    devices = list(cycles_preferences.devices)

    optix_devices = [x for x in devices if str(x.type) == "OPTIX"]
    cuda_devices = [x for x in devices if str(x.type) == "CUDA"]
    cpu_devices = [x for x in devices if str(x.type) == "CPU"]

    if optix_devices:
        bpy.context.scene.render.engine = 'CYCLES'
        bpy.context.scene.cycles.device = 'GPU'
        for device in optix_devices:
            device.use = True
        render_device = RenderDevice.OPTIX
    elif cuda_devices:
        bpy.context.scene.render.engine = 'CYCLES'
        bpy.context.scene.cycles.device = 'GPU'
        for device in cuda_devices:
            device.use = True
        render_device = RenderDevice.CUDA
    elif cpu_devices:
        bpy.context.scene.render.engine = 'BLENDER_EEVEE'
        for device in cpu_devices:
            device.use = True
        render_device = RenderDevice.EEVEE
    else:
        render_device = RenderDevice.UNSUPPORTED

    bpy.context.scene.cycles.preview_samples = 500
    bpy.context.scene.cycles.use_denoising = True
    bpy.context.scene.render.use_persistent_data = True

    if optix_devices:
        bpy.context.scene.cycles.denoiser = 'OPTIX'

    bpy.context.scene.render.use_simplify = True
    bpy.context.scene.render.simplify_subdivision = 2
    bpy.context.scene.cycles.texture_limit_render = '2048'

    bpy.context.scene.cycles.use_camera_cull = True
    bpy.context.scene.cycles.use_distance_cull = True

    bpy.context.scene.cycles.use_fast_gi = True
    bpy.context.scene.cycles.use_animated_seed = True

    # might need to turn this off
    bpy.context.scene.cycles.auto_scrambling_distance = True

    bpy.context.scene.cycles.max_bounces = 5

    # bpy.context.scene.render.image_settings.file_format = 'OPEN_EXR'
    bpy.context.scene.render.image_settings.file_format = 'PNG'
    bpy.context.scene.render.image_settings.color_mode = 'RGBA'
    bpy.context.scene.render.image_settings.color_depth = '16'
    bpy.context.scene.render.image_settings.exr_codec = 'DWAA'

    # Enable transparent background
    bpy.context.scene.render.film_transparent = True

    return render_device


def set_render_resolution(width: int, height: int, scale: int) -> None:
    """
    Set the resolution of the rendered image
    :param width: The width of the rendered image
    :param height: The height of the rendered image
    :param scale: The scale of the rendered image
    :return: None
    """
    bpy.context.scene.render.resolution_x = width
    bpy.context.scene.render.resolution_y = height
    bpy.context.scene.render.resolution_percentage = scale


def render_generic_view(name: str, output_folder: str, position: Position):
    """
    Render a generic view of the scene with the specified parameters
    :param name: The name of the rendered image
    :param output_folder: The output folder for the rendered image
    :param position: The position and rotation of the camera
    :return: None
    """
    set_camera_pos_and_rot(position)
    bpy.context.scene.render.filepath = (output_folder + f"{name}_{str(position)}_{get_formatted_date()}")
    try:
        bpy.ops.render.render(write_still=True)
    except Exception as e:
        sys.exit(1)


########################################################################################################################
# CAMERA FUNCTIONS
########################################################################################################################
def create_camera() -> None:
    exists = False
    # Iterate through all the objects in the scene
    for obj in bpy.data.objects:
        if obj.type == "CAMERA" and obj.name == "Camera":
            exists = True

    # If a camera with the name "Camera" does not exist, create one
    if not exists:
        bpy.ops.object.camera_add(enter_editmode=False, align='VIEW', location=(0, 0, 0), rotation=(0, 0, 0),
                                  scale=(1, 1, 1))


def set_camera_start_pos(distance: float) -> None:
    """
    Set the camera to a birds eye view of the origin
    :param distance: The distance from the origin to the camera in meters along the z-axis
    :return: None
    """
    # set the initial camera position and rotation to look at the origin from a birds eye view
    bpy.data.objects["Camera"].location[0] = 0
    bpy.data.objects["Camera"].location[1] = 0
    bpy.data.objects["Camera"].location[2] = distance

    bpy.data.objects["Camera"].rotation_euler[0] = 0
    bpy.data.objects["Camera"].rotation_euler[1] = 0
    bpy.data.objects["Camera"].rotation_euler[2] = 0


def set_camera_pos_and_rot(pos: Position) -> None:
    """
    Set the camera to a specific position and rotation
    :param pos: The position and rotation of the camera
    """
    bpy.data.objects["Camera"].location[0] = pos.x
    bpy.data.objects["Camera"].location[1] = pos.y
    bpy.data.objects["Camera"].location[2] = pos.z

    bpy.data.objects["Camera"].rotation_euler[0] = degrees_to_radians(pos.rx)
    bpy.data.objects["Camera"].rotation_euler[1] = degrees_to_radians(pos.ry)
    bpy.data.objects["Camera"].rotation_euler[2] = degrees_to_radians(pos.rz)


########################################################################################################################
# COMPUTATION FUNCTIONS
########################################################################################################################
def degrees_to_radians(degrees: float) -> float:
    """
    Convert degrees to radians
    :param degrees: The degrees to convert to radians
    :return: The converted degrees in radians value
    """
    return degrees * (3.14159 / 180)


def compute_triangular_leg(distance: float):
    """
    Compute the length of the leg of a right-angled triangle given the hypotenuse
    :param distance: The length of the hypotenuse
    :return: The length of the leg of the triangle
    """
    return math.sqrt(math.pow(distance, 2) / 2)


########################################################################################################################
# HELPER FUNCTIONS
########################################################################################################################
def get_formatted_date():
    """
    Get the current date and time in a formatted string
    :return: A formatted string of the current date and time
    """
    return datetime.datetime.now().strftime("%Y-%m-%d-%H-%M-%S-%f")


########################################################################################################################
# MAIN
########################################################################################################################
if __name__ == "__main__":
    # Parse the command line arguments
    parser = BlenderArgparse(description="Script to render a view of the scene with the specified parameters")
    parser.add_argument("--model", type=str, help="The path to the model")
    parser.add_argument("--name", type=str, help="The name of the rendered image")
    parser.add_argument("--output_path", type=str, help="The output path for the rendered images")
    parser.add_argument("--resolution", type=int, nargs=2, help="The resolution of the rendered image")
    parser.add_argument("--scale", type=int, help="The scale of the rendered image")
    parser.add_argument("--distance", type=float, help="The distance from the origin to the camera")
    parser.add_argument("--x", type=float, help="The x position of the camera")
    parser.add_argument("--y", type=float, help="The y position of the camera")
    parser.add_argument("--z", type=float, help="The z position of the camera")
    parser.add_argument("--rx", type=float, help="The x rotation of the camera")
    parser.add_argument("--ry", type=float, help="The y rotation of the camera")
    parser.add_argument("--rz", type=float, help="The z rotation of the camera")
    args = parser.parse_args()

    # Check that each argument is present, if any are not, set them to default values
    defaults = {
        "model": None,
        "name": "render",
        "output_path": None,
        "resolution": (1920, 1080),
        "scale": 100,
        "distance": 2,
        "x": 0,
        "y": 0,
        "z": 2,
        "rx": 0,
        "ry": 0,
        "rz": 0,
    }

    for key, value in defaults.items():
        if getattr(args, key) is None:
            if key == "output_path":
                raise Exception("Output path not specified")
            if key == "model":
                raise Exception("Model path not specified")
            setattr(args, key, value)

    # If the output path does not end with a "/", add one
    output_path = args.output_path
    if not output_path.endswith("/"):
        output_path += "/"

    # Create the camera
    create_camera()

    # Import the model if it is not a .blend file
    if not args.model.endswith(".blend"):
        import_model(args.model, Unit.MILLIMETERS)

    # Detect the rendering device and set the rendering preferences
    set_render_preferences()

    # Set the rendering resolution
    set_render_resolution(width=args.resolution[0], height=args.resolution[1], scale=args.scale)

    # Set the camera position
    dist = args.distance
    leg = compute_triangular_leg(dist)
    position = Position(x=args.x, y=args.y, z=args.z, rx=args.rx, ry=args.ry, rz=args.rz)
    set_camera_start_pos(dist)

    # Render the view
    render_generic_view(name=args.name, output_folder=output_path, position=position)
