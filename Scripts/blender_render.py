from typing import List

import jsonpickle


class Position:
    x: float
    y: float
    z: float

    def __init__(self, x: float, y: float, z: float):
        self.x = x
        self.y = y
        self.z = z

class Rotation:
    rx: float
    ry: float
    rz: float

    def __init__(self, rx: float, ry: float, rz: float):
        self.rx = rx
        self.ry = ry
        self.rz = rz

class Camera:
    """
    Camera class to store camera position and rotation
    """
    position: Position
    rotation: Rotation

    def __init__(self, position: Position, rotation: Rotation):
        self.position = position
        self.rotation = rotation

class Light:
    position: Position
    rotation: Rotation
    colour: str
    power: float
    size: float
    distance: float

    def __init__(self, position: Position, rotation: Rotation, colour: str, power: float, size: float, distance: float):
        self.position = position
        self.rotation = rotation
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


class Render:
    name: str
    model: str
    unit: float
    output_directory: str
    resolution: Resolution
    camera: Camera
    lights: List[Light]
    save_blender_file: bool

    def __init__(self, name: str, model: str, unit: float, output_directory: str, resolution: Resolution, camera: Camera, lights: List[Light], save_blender_file: bool):
        self.name = name
        self.model = model
        self.unit = unit
        self.output_directory = output_directory
        self.resolution = resolution
        self.camera = camera
        self.lights = lights
        self.save_blender_file = save_blender_file

    def __str__(self):
        return jsonpickle.encode(self, unpicklable=False)

    def create_camera(self):
        exists = False

        for obj in bpy.data.objects:
            if obj.type == 'CAMERA' and obj.name == 'Camera':
                exists = True

        if not exists:
            bpy.ops.object.camera_add(enter_editmode=False, align='VIEW', location=(camera.position.x, camera.position.y, camera.position.z), rotation=(camera.rotation.rx, camera.rotation.ry, camera.rotation.rz), scale=(1, 1, 1))


    def create_lights(self):
        for light in self.lights:
            pass


if __name__ == '__main__':
    position = Position(0, 0, 0)
    rotation = Rotation(0, 0, 0)
    camera = Camera(position, rotation)
    light = Light(position, rotation, "white", 1000, 10, 100)
    resolution = Resolution(1920, 1080)
    settings = Render("Test", "model.obj", 1.0, "output", resolution, camera, [light], True)
    print(settings)