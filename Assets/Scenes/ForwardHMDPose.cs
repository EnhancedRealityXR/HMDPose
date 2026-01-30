using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR;

public class QuestHMDToOpentrackUDP : MonoBehaviour
{
    [Header("Defaults (used if config missing)")]
    public string defaultIpAddress = "192.168.178.1";
    public int defaultPort = 4242;

    [Header("Translation")]
    public float positionScale = 100f; // meters -> centimeters

    private UdpClient udp;
    private IPEndPoint endPoint;

    private string configPath;

    // opentrack UDP layout:
    // [0]=TX [1]=TY [2]=TZ [3]=Yaw [4]=Pitch [5]=Roll
    private readonly double[] pose = new double[6];

    void Start()
    {
        configPath = Path.Combine(Application.persistentDataPath, "opentrack.cfg");

        string ip;
        int port;

        LoadOrCreateConfig(out ip, out port);

        udp = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Debug.Log($"[Opentrack] Sending UDP to {ip}:{port}");
        Debug.Log($"[Opentrack] Config path: {configPath}");
    }

    void Update()
    {
        InputDevice hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (!hmd.isValid)
            return;

        if (!hmd.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion q))
            return;

        if (!hmd.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            return;

        /* ---------------- ROTATION ---------------- */

        double w = q.w;
        double x = q.x;
        double y = q.y;
        double z = q.z;

        double yaw = Math.Atan2(
            2.0 * (w * y + x * z),
            1.0 - 2.0 * (y * y + z * z)
        );

        double pitch = Math.Asin(
            Math.Clamp(2.0 * (w * x - y * z), -1.0, 1.0)
        );

        double roll = Math.Atan2(
            2.0 * (w * z + x * y),
            1.0 - 2.0 * (x * x + z * z)
        );

        const double rad2deg = 180.0 / Math.PI;

        pose[3] =  yaw   * rad2deg;   // Yaw
        pose[4] = -pitch * rad2deg;   // Pitch
        pose[5] = -roll  * rad2deg;   // Roll

        /* ---------------- TRANSLATION ---------------- */

        pose[0] =  pos.x * positionScale;   // TX
        pose[1] =  pos.y * positionScale;   // TY
        pose[2] = -pos.z * positionScale;   // TZ

        SendPose();
    }

    private void SendPose()
    {
        byte[] buffer = new byte[6 * sizeof(double)];

        for (int i = 0; i < 6; i++)
        {
            byte[] b = BitConverter.GetBytes(pose[i]);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Buffer.BlockCopy(b, 0, buffer, i * 8, 8);
        }

        udp.Send(buffer, buffer.Length, endPoint);
    }

    private void LoadOrCreateConfig(out string ip, out int port)
    {
        ip = defaultIpAddress;
        port = defaultPort;

        if (!File.Exists(configPath))
        {
            CreateDefaultConfig();
            return;
        }

        try
        {
            foreach (string line in File.ReadAllLines(configPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key.Equals("opentrack_ip", StringComparison.OrdinalIgnoreCase))
                    ip = value;

                else if (key.Equals("opentrack_port", StringComparison.OrdinalIgnoreCase))
                    int.TryParse(value, out port);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Opentrack] Failed to read config, using defaults: {e.Message}");
        }
    }

    private void CreateDefaultConfig()
    {
        try
        {
            File.WriteAllText(
                configPath,
                "# Opentrack UDP config\n" +
                $"opentrack_ip={defaultIpAddress}\n" +
                $"opentrack_port={defaultPort}\n"
            );

            Debug.Log("[Opentrack] Default config created");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Opentrack] Failed to create config: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        udp?.Close();
    }
}
