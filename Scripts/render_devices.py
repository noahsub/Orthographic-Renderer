########################################################################################################################
# render_devices.py
#
# This script is used to find and set the appropriate render device for the current system.
#
# Copyright (C) 2024 noahsub
########################################################################################################################


########################################################################################################################
# IMPORTS
########################################################################################################################
import json
import bpy

########################################################################################################################
# GLOBALS
########################################################################################################################
RENDER_DEVICES = None


########################################################################################################################
# RENDER DEVICE FUNCTIONS
########################################################################################################################
def find_render_devices() -> dict:
    """
    Find all available render devices on the current system, and organize them by type.
    """

    # Use the global variable to store the render devices
    global RENDER_DEVICES

    # Get the preferences and devices
    preferences = bpy.context.preferences
    cycles_preferences = preferences.addons["cycles"].preferences
    cycles_preferences.refresh_devices()
    devices = list(cycles_preferences.devices)

    # Organize the devices by type
    RENDER_DEVICES = {
        "OPTIX": [x for x in devices if str(x.type) == "OPTIX"],
        "CUDA": [x for x in devices if str(x.type) == "CUDA"],
        "CPU": [x for x in devices if str(x.type) == "CPU"],
    }

    # Return the render devices
    return RENDER_DEVICES


def set_render_device():
    """
    Sets the render device(s) to used, taking priority of OptiX, then CUDA, then CPU.
    """

    # Check if the render devices have been found
    if RENDER_DEVICES is None:
        # Find the render devices if they have not been found
        find_render_devices()

    # Ensure the render devices have been found
    assert RENDER_DEVICES is not None

    # Get the devices
    optix_devices = RENDER_DEVICES["OPTIX"]
    cuda_devices = RENDER_DEVICES["CUDA"]
    cpu_devices = RENDER_DEVICES["CPU"]

    # If there are OptiX devices present, use them
    if len(optix_devices) != 0:
        # Set the cycles compute device type to OptiX
        bpy.context.preferences.addons["cycles"].preferences.compute_device_type = (
            "OPTIX"
        )
        # Set the render engine to Cycles and the device to GPU
        bpy.context.scene.render.engine = "CYCLES"
        bpy.context.scene.cycles.device = "GPU"
        # Use the OptiX devices
        for gpu in optix_devices:
            gpu.use = True
        # Return the render engine and device
        return "CYCLES", "OPTIX"

    # Otherwise, if there are CUDA devices present, use them
    elif len(cuda_devices) != 0:
        # Set the cycles compute device type to CUDA
        bpy.context.preferences.addons["cycles"].preferences.compute_device_type = (
            "CUDA"
        )
        # Set the render engine to Cycles and the device to GPU
        bpy.context.scene.render.engine = "CYCLES"
        bpy.context.scene.cycles.device = "GPU"
        # Use the CUDA devices
        for gpu in cuda_devices:
            gpu.use = True
        # Return the render engine and device
        return "CYCLES", "CUDA"

    # Otherwise, use the CPU
    elif len(cpu_devices) != 0:
        # Set the render engine to Eevee and the device to CPU
        bpy.context.scene.render.engine = "BLENDER_EEVEE_NEXT"
        # Use the CPU devices
        for cpu in cpu_devices:
            cpu.use = True
        # Return the render engine and device
        return "BLENDER_EEVEE_NEXT", "CPU"


def print_found_render_devices():
    """
    Print the found render devices in a JSON format.
    """

    # Check if the render devices have been found
    if RENDER_DEVICES is None:
        # Find the render devices if they have not been found
        find_render_devices()

    # Ensure the render devices have been found
    assert RENDER_DEVICES is not None
    # Ensure the render devices are a dictionary
    assert type(RENDER_DEVICES) == dict

    # Create a dictionary of the render device names
    names = dict()
    # Get the names of the render devices
    for key, value in RENDER_DEVICES.items():
        names[key] = [x.name for x in value]

    # Convert the dictionary to a JSON string
    devices_json = json.dumps(names, indent=4)
    # Print the JSON string
    print(devices_json)


########################################################################################################################
# MAIN
########################################################################################################################
if __name__ == "__main__":
    # Find and print the render devices
    print_found_render_devices()
