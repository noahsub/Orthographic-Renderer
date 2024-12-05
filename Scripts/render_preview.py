########################################################################################################################
# render.py
#
# This script is used to render a view of the scene with the specified parameters. It supports rendering with both
# Cycles and Eevee render engines. It takes GPU preference over CPU if both are available.
#
# Copyright (C) 2024 noahsub
########################################################################################################################

########################################################################################################################
# IMPORTS
########################################################################################################################
import datetime
import json
import math
import argparse
import os
from enum import Enum
import sys
import bpy


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

def import_model(model_path: str, unit: float) -> None:
    directory = os.path.dirname(model_path)
    file_name = os.path.basename(model_path)
    name = os.path.splitext(file_name)[0]

    if model_path.endswith(".obj"):
        bpy.ops.wm.obj_import(filepath=model_path, directory=directory, global_scale=unit, forward_axis='Y',
                              up_axis='Z')
    elif model_path.endswith(".stl"):
        bpy.ops.wm.stl_import(filepath=model_path, directory=directory, global_scale=unit, forward_axis='Y',
                              up_axis='Z')

    try:
        bpy.data.objects.remove(bpy.data.objects["Cube"], do_unlink=True)
    except:
        pass

    bpy.ops.object.select_all(action='DESELECT')
    bpy.data.objects[name].select_set(True)
    bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_VOLUME', center='MEDIAN')
    bpy.data.objects[name].location = (0, 0, 0)


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
        bpy.context.preferences.addons['cycles'].preferences.compute_device_type = 'OPTIX'
        bpy.context.scene.render.engine = 'CYCLES'
        bpy.context.scene.cycles.device = 'GPU'
        for device in optix_devices:
            device.use = True
        render_device = RenderDevice.OPTIX
    elif cuda_devices:
        bpy.context.preferences.addons['cycles'].preferences.compute_device_type = 'CUDA'
        bpy.context.scene.render.engine = 'CYCLES'
        bpy.context.scene.cycles.device = 'GPU'
        for device in cuda_devices:
            device.use = True
        render_device = RenderDevice.CUDA
    elif cpu_devices:
        bpy.context.scene.render.engine = 'BLENDER_EEVEE_NEXT'
        for device in cpu_devices:
            device.use = True
        render_device = RenderDevice.EEVEE
    else:
        render_device = RenderDevice.UNSUPPORTED

    bpy.context.scene.cycles.preview_samples = 200
    bpy.context.scene.cycles.use_denoising = True
    bpy.context.scene.render.use_persistent_data = True

    if optix_devices:
        bpy.context.scene.cycles.denoiser = 'OPTIX'

    bpy.context.scene.render.use_simplify = True
    bpy.context.scene.render.simplify_subdivision = 2
    bpy.context.scene.cycles.texture_limit_render = '512'

    bpy.context.scene.cycles.use_camera_cull = True
    bpy.context.scene.cycles.use_distance_cull = True

    bpy.context.scene.cycles.use_fast_gi = True
    bpy.context.scene.cycles.use_animated_seed = True

    # might need to turn this off
    bpy.context.scene.cycles.auto_scrambling_distance = True

    bpy.context.scene.cycles.max_bounces = 3

    # bpy.context.scene.render.image_settings.file_format = 'OPEN_EXR'
    bpy.context.scene.render.image_settings.file_format = 'PNG'
    bpy.context.scene.render.image_settings.color_mode = 'RGBA'
    bpy.context.scene.render.image_settings.color_depth = '8'
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
    bpy.context.scene.render.filepath = (output_folder + f"{name}")
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
# LIGHTING FUNCTIONS
########################################################################################################################

def create_area_light(power, size, position):
    # Create a new area light
    bpy.ops.object.light_add(type='AREA', align='WORLD', location=(0, 0, 0), scale=(1, 1, 1))

    # Get the newly created light
    light = bpy.context.object

    # Set the light's power
    light.data.energy = power

    # Set the light's size
    light.data.size = size

    # Set the light's position
    light.location[0] = position.x
    light.location[1] = position.y
    light.location[2] = position.z

    # Set the light's rotation
    light.rotation_euler[0] = degrees_to_radians(position.rx)
    light.rotation_euler[1] = degrees_to_radians(position.ry)
    light.rotation_euler[2] = degrees_to_radians(position.rz)


def setup_lighting(distance: float):
    # delete all existing lights
    bpy.ops.object.select_all(action='DESELECT')
    bpy.ops.object.select_by_type(type='LIGHT')
    bpy.ops.object.delete()

    leg = compute_triangular_leg(distance)

    # Create top front left light
    create_area_light(1000, 3, Position(-leg, -leg, distance, 45, 0, 315))
    # Create top front right light
    create_area_light(800, 3, Position(leg, -leg, distance, 45, 0, 45))
    # Create top back left light
    create_area_light(200, 3, Position(-leg, leg, distance, 45, 0, 225))


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
# FILE FUNCTIONS
########################################################################################################################

def save_file(file_path: str) -> None:
    """
    Save the file as a .blend file.
    :param file_path: The path to save the file
    :return: None
    """
    bpy.ops.wm.save_as_mainfile(filepath=file_path.replace(".obj", ".blend").replace(".stl", ".blend"))


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
    parser.add_argument("--options", type=str, help="Options for rendering")
    args = parser.parse_args()

    data = json.loads(args.options)

    # Import the model if it is not a .blend file
    if not data["Model"].endswith(".blend"):
        import_model(data["Model"], data["Unit"])

    # Otherwise, open the .blend file
    else:
        bpy.ops.wm.open_mainfile(filepath=data["Model"])

    # If the output path does not end with a "/", add one
    output_path = data["OutputDirectory"]
    if not output_path.endswith("/"):
        output_path += "/"

    # Set the default unit settings
    bpy.context.scene.unit_settings.system = 'METRIC'
    bpy.context.scene.unit_settings.scale_length = 1
    bpy.context.scene.unit_settings.system_rotation = 'DEGREES'

    # Create the camera
    create_camera()

    # # Setup lighting
    # setup_lighting(args.light_distance)

    # delete all existing lights
    bpy.ops.object.select_all(action='DESELECT')
    bpy.ops.object.select_by_type(type='LIGHT')
    bpy.ops.object.delete()

    for light in data["Lights"]:
        create_area_light(light["Power"], light["Size"], Position(light["Position"]["X"], light["Position"]["Y"], light["Position"]["Z"], light["Position"]["Rx"], light["Position"]["Ry"], light["Position"]["Rz"]))

    # Detect the rendering device and set the rendering preferences
    set_render_preferences()

    # Set the rendering resolution
    set_render_resolution(width=data["Resolution"]["Width"], height=data["Resolution"]["Height"], scale=data["Resolution"]["Scale"])

    # Set the camera position
    dist = data["Camera"]["Distance"]
    leg = compute_triangular_leg(dist)
    position = Position(x=data["Camera"]["Position"]["X"], y=data["Camera"]["Position"]["Y"], z=data["Camera"]["Position"]["Z"],
                        rx=data["Camera"]["Position"]["Rx"], ry=data["Camera"]["Position"]["Ry"], rz=data["Camera"]["Position"]["Rz"])

    set_camera_start_pos(dist)

    # Render the view
    render_generic_view(name=data["Name"], output_folder=output_path, position=position)

    save = data["SaveBlenderFile"]

    if save:
        save_file(output_path + data["Name"] + ".blend")
