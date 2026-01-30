# HMDPose - HMD Pose forwarding for standalone OpenXR Headsets with Opentrack support

HMDPose is an Android OpenXR Unity application. It runs as a flatscreen (2D) background application and forwards the Head Mounted Display (HMD) pose (position and rotation) to [Opentrack](https://github.com/opentrack/opentrack) over the local network via UDP.

This allows you to use your Headset as a head tracker for PC games or other applications that support Opentrack, even while simultaneously running other 2D apps or any immersive app/game on your headset. For example, HMDPose adds 3-DOF and 6-DOF capabilities to non VR games that can be played through existing PCVR solutions such as ALVR/VirtualDesktop/SteamLink/MetaLink. 

## Features

- **Real-time Pose Forwarding**: Sends HMD position and rotation data to Opentrack.
- **Background Operation**: Runs as a 2D Android app with seamless multitasking support, allowing it to function in the background.
- **Configurable**: IP address and port can be customized via a configuration file.
- **Lightweight**: Minimal overhead.

## Requirements

- Meta Quest Headset (or other compatible headset).
- PC running [Opentrack](https://github.com/opentrack/opentrack).
- Both devices connected to the same local network (Wi-Fi) or a internet bridge/vpn solution.

## Installation

1.  **Build the APK**: Open this project in Unity and build the Android APK.
    - Ensure your Build Settings are switched to **Android**.
    - Build the project.
2.  **Install on Quest**: Use ADB or SideQuest to install the generated `.apk` file onto your Quest 3.
    ```bash
    adb install HMDPose.apk
    ```

## Usage

1.  **Start Opentrack** on your PC.
    - Set **Input** to **UDP over network**.
    - Click the settings icon next to Input (hammer icon).
    - Note the **Port** (default is usually 4242).
    - Set **Output** to Mouse Emulation (or anything else you like).
    - Click **Start** in Opentrack.

2.  **Get local IP** on your PC.
    - Open **Command Prompt** on your PC by searching for **CMD** in Windows Search.
    - Type "ipconfig" in CMD and hit enter.
    - Note the displayed (local) IPV4 Address.

3.  **Launch HMDPose** on your Quest 3.
    - Go to your App Library -> (Unknown Sources).
    - Launch **HMDPose**.
    - Exit **HMDPose**
    - Connect your headset to the PC and allow file access (in the Headset).
    - Use SideQuest or ADB to insert your previously noted IP and Port in the following file **Android/data/com.EnhancedReality.HMDPose/opentrack.cfg** (Save to Desktop -> Edit IP/Port -> Overwrite on Headset)
    - Launch **HMDPose** again. 

4.  The app will immediately start sending UDP packets to the configured IP address. **IMPORTANT**: When using HMDpose simultaneously with another app on the headset, make sure the HMDpose app is in the foreground while minimized (invisible). Otherwise HMDPose cannot track your head accurately which will result in a choppy/laggy experience. 

## Supported Devices

- Tested on Meta Quest 3, but should work on any standalone Headset that supports OpenXR. May require adjustments to the Unity Android/OpenXR Build settings.

## Implementing HMD Pose in your own OpenXR Unity Project

- Copy **Assets/Scenes/ForwardHMDPose.cs** to the matching directory of your OpenXR Unity Project and attach the script to an empty GameObject in the Scene.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
