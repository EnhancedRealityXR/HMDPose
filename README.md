# HMDPose for Meta Quest 3

HMDPose is a Unity-based application designed for the Meta Quest 3. It runs as a flatscreen (2D) background application and forwards the Head Mounted Display (HMD) pose (position and rotation) to [Opentrack](https://github.com/opentrack/opentrack) over the local network via UDP.

This allows you to use your Quest 3 as a head tracker for PC games or other applications that support Opentrack, even while the Quest is running other 2D Android apps or just sitting in the background.

## Features

- **Real-time Pose Forwarding**: Sends HMD position and rotation data to Opentrack.
- **Background Operation**: Runs as a 2D Android app with multitasking support, allowing it to function in the background.
- **Configurable**: IP address and port can be customized via a configuration file.
- **Lightweight**: Minimal overhead.

## Requirements

- Meta Quest 3 (or other compatible Meta Quest headset).
- PC running [Opentrack](https://github.com/opentrack/opentrack).
- Both devices connected to the same local network (Wi-Fi).

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
    - Click **Start** in Opentrack.

2.  **Launch HMDPose** on your Quest 3.
    - Go to your App Library -> (Search/Filter for Unknown Sources if necessary).
    - Launch **HMDPose**.

3.  The app will immediately start sending UDP packets to the configured IP address.

## Configuration

By default, the app sends data to:
- **IP**: `192.168.178.35`
- **Port**: `4242`

To change this, you need to edit the `opentrack.cfg` file on the Quest.

1.  Run the app once to generate the default configuration file.
2.  Connect your Quest to your PC via USB.
3.  Navigate to the app's persistent data path. This is typically located at:
    `/sdcard/Android/data/com.yourcompany.HMDPose/files/opentrack.cfg`
    *(Note: The package name `com.yourcompany.HMDPose` depends on your Unity Player Settings)*
4.  Open `opentrack.cfg` and update the IP and Port to match your PC's local IP address and Opentrack port.

    ```ini
    # Opentrack UDP config
    opentrack_ip=192.168.1.100
    opentrack_port=4242
    ```
5.  Restart the app on your Quest.

## Data Format

The app sends a UDP packet containing 6 `double` values (little-endian):
1.  **X** Position (cm)
2.  **Y** Position (cm)
3.  **Z** Position (cm)
4.  **Yaw** (degrees)
5.  **Pitch** (degrees)
6.  **Roll** (degrees)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
