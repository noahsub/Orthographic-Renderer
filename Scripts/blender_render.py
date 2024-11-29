import json

import bpy
import sys

import os
import argparse

import datetime
from typing import List

def get_formatted_date():
    """
    Get the current date and time in a formatted string
    :return: A formatted string of the current date and time
    """
    return datetime.datetime.now().strftime("%Y-%m-%d-%H-%M-%S-%f")

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
# RENDER SETTINGS
########################################################################################################################


def degrees_to_radians(degrees: float) -> float:
    """
    Convert degrees to radians
    :param degrees: The degrees to convert to radians
    :return: The converted degrees in radians value
    """
    return degrees * (3.14159 / 180)


class Position:
    x: float
    y: float
    z: float
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


class Camera:
    """
    Camera class to store camera position and rotation
    """
    position: Position

    def __init__(self, position: Position):
        self.position = position


class Light:
    position: Position
    colour: str
    power: float
    size: float
    distance: float

    def __init__(
        self,
        position: Position,
        colour: str,
        power: float,
        size: float,
        distance: float,
    ):
        self.position = position
        self.colour = colour
        self.power = power
        self.size = size
        self.distance = distance


class Resolution:
    width: int
    height: int
    scale: int

    def __init__(self, width: int, height: int, scale: int = 100):
        self.width = width
        self.height = height
        self.scale = scale


class RenderSettings:
    name: str
    model: str
    unit: float
    output_directory: str
    resolution: Resolution
    camera: Camera
    lights: List[Light]
    save_blender_file: bool

    def __init__(
        self,
        name: str,
        model: str,
        unit: float,
        output_directory: str,
        resolution: Resolution,
        camera: Camera,
        lights: List[Light],
        save_blender_file: bool,
    ):
        self.name = name
        self.model = model
        self.unit = unit
        self.output_directory = output_directory
        self.resolution = resolution
        self.camera = camera
        self.lights = lights
        self.save_blender_file = save_blender_file

    def import_model(self):
        directory = os.path.dirname(self.model)
        file_name = os.path.basename(self.model)
        name = os.path.splitext(file_name)[0]

        if self.model.endswith(".obj"):
            bpy.ops.wm.obj_import(
                filepath=self.model,
                directory=directory,
                global_scale=self.unit,
                forward_axis="Y",
                up_axis="Z",
            )
        elif self.model.endswith(".stl"):
            bpy.ops.wm.stl_import(
                filepath=self.model,
                directory=directory,
                global_scale=self.unit,
                forward_axis="Y",
                up_axis="Z",
            )

        try:
            bpy.data.objects.remove(bpy.data.objects["Cube"], do_unlink=True)
        except:
            pass

        bpy.ops.object.select_all(action="DESELECT")
        bpy.data.objects[name].select_set(True)
        bpy.ops.object.origin_set(type="ORIGIN_CENTER_OF_VOLUME", center="MEDIAN")
        bpy.data.objects[name].location = (0, 0, 0)

    def set_render_resolution(self):
        bpy.context.scene.render.resolution_x = self.resolution.width
        bpy.context.scene.render.resolution_y = self.resolution.height
        bpy.context.scene.render.resolution_percentage = self.resolution.scale

    def create_camera(self):
        exists = False

        for obj in bpy.data.objects:
            if obj.type == "CAMERA" and obj.name == "Camera":
                exists = True

        if not exists:
            bpy.ops.object.camera_add(
                enter_editmode=False,
                align="VIEW",
                location=(self.camera.position.x, self.camera.position.y, self.camera.position.z),
                rotation=(self.camera.position.rx, self.camera.position.ry, self.camera.position.rz),
                scale=(1, 1, 1),
            )

    def set_render_preferences(self):
        preferences = bpy.context.preferences
        cycles_preferences = preferences.addons["cycles"].preferences
        cycles_preferences.refresh_devices()
        devices = list(cycles_preferences.devices)

        optix_devices = [x for x in devices if str(x.type) == "OPTIX"]
        cuda_devices = [x for x in devices if str(x.type) == "CUDA"]
        cpu_devices = [x for x in devices if str(x.type) == "CPU"]

        if optix_devices:
            bpy.context.scene.render.engine = "CYCLES"
            bpy.context.scene.cycles.device = "GPU"
            for device in optix_devices:
                device.use = True
        elif cuda_devices:
            bpy.context.scene.render.engine = "CYCLES"
            bpy.context.scene.cycles.device = "GPU"
            for device in cuda_devices:
                device.use = True
        elif cpu_devices:
            bpy.context.scene.render.engine = "BLENDER_EEVEE_NEXT"
            for device in cpu_devices:
                device.use = True

        else:
            pass

        bpy.context.scene.cycles.preview_samples = 500
        bpy.context.scene.cycles.use_denoising = True
        bpy.context.scene.render.use_persistent_data = True

        if optix_devices:
            bpy.context.scene.cycles.denoiser = "OPTIX"

        bpy.context.scene.render.use_simplify = True
        bpy.context.scene.render.simplify_subdivision = 2
        bpy.context.scene.cycles.texture_limit_render = "2048"

        bpy.context.scene.cycles.use_camera_cull = True
        bpy.context.scene.cycles.use_distance_cull = True

        bpy.context.scene.cycles.use_fast_gi = True
        bpy.context.scene.cycles.use_animated_seed = True

        # might need to turn this off
        bpy.context.scene.cycles.auto_scrambling_distance = True

        bpy.context.scene.cycles.max_bounces = 5

        # bpy.context.scene.render.image_settings.file_format = 'OPEN_EXR'
        bpy.context.scene.render.image_settings.file_format = "PNG"
        bpy.context.scene.render.image_settings.color_mode = "RGBA"
        bpy.context.scene.render.image_settings.color_depth = "16"
        bpy.context.scene.render.image_settings.exr_codec = "DWAA"

        # Enable transparent background
        bpy.context.scene.render.film_transparent = True

    def create_lights(self):
        bpy.ops.object.select_all(action="DESELECT")
        bpy.ops.object.select_by_type(type="LIGHT")
        bpy.ops.object.delete()

        for area_light in self.lights:
            bpy.ops.object.light_add(
                type="AREA", align="WORLD", location=(0, 0, 0), scale=(1, 1, 1)
            )
            light = bpy.context.object
            light.data.energy = area_light.power
            light.data.size = area_light.size
            light.location[0] = area_light.position.x
            light.location[1] = area_light.position.y
            light.location[2] = area_light.position.z

            light.rotation_euler[0] = degrees_to_radians(area_light.position.rx)
            light.rotation_euler[1] = degrees_to_radians(area_light.position.ry)
            light.rotation_euler[2] = degrees_to_radians(area_light.position.rz)


if __name__ == "__main__":
    parser = BlenderArgparse(description="Render an orthographic image of a 3D model")
    # Take a json string as input, synonomous with the RenderSettings class string representation
    parser.add_argument("--options", type=str, help="Render settings as a json string")
    args = parser.parse_args()

    # Load the render settings from the json string
    data = json.loads(args.options)


    def convert_keys_to_lowercase(data):
        if isinstance(data, dict):
            return {k.lower(): convert_keys_to_lowercase(v) for k, v in data.items()}
        elif isinstance(data, list):
            return [convert_keys_to_lowercase(item) for item in data]
        else:
            return data


    # Example usage
    data = json.loads(args.options)
    data = convert_keys_to_lowercase(data)

    lights = []
    for light in data["lights"]:
        lights.append(
            Light(
                Position(
                    light["position"]["x"],
                    light["position"]["y"],
                    light["position"]["z"],
                    light["position"]["rx"],
                    light["position"]["ry"],
                    light["position"]["rz"],
                ),
                light["colour"],
                light["power"],
                light["size"],
                light["distance"],
            )
        )

    resolution = Resolution(
        data["resolution"]["width"],
        data["resolution"]["height"],
        data["resolution"]["scale"],
    )

    render_settings = RenderSettings(
        name=data['name'],
        model=data['model'],
        unit=data['unit'],
        output_directory=data['outputdirectory'],
        resolution=resolution,
        camera=data['camera'],
        lights=lights,
        save_blender_file=data['saveblenderfile'],
    )

    # print(json.dumps(render_settings.__dict__, indent=4))

    print(render_settings.lights[0].power)

    # Set the default unit settings
    bpy.context.scene.unit_settings.system = 'METRIC'
    bpy.context.scene.unit_settings.scale_length = 1
    bpy.context.scene.unit_settings.system_rotation = 'DEGREES'

    # Create the camera
    render_settings.create_camera()

    # Setup lighting
    render_settings.create_lights()

    # Import the model
    render_settings.import_model()

    # Set render preferences
    render_settings.set_render_preferences()

    # Set the render resolution
    render_settings.set_render_resolution()

    if render_settings.save_blender_file:
        save_file(render_settings.output_directory + f"{render_settings.name}_{get_formatted_date()}.blend")

    # Render the image
    bpy.context.scene.render.filepath = (render_settings.output_directory + f"{render_settings.name}_{get_formatted_date()}")
    try:
        bpy.ops.render.render(write_still=True)
    except Exception as e:
        sys.exit(1)



