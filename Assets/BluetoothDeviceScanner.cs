using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BluetoothDeviceScanner : MonoBehaviour
{
    public TMP_Text deviceListText;
    private List<string> deviceList = new List<string>();

    public void ScanForDevices()
    {
        using (AndroidJavaObject bluetoothAdapter = GetBluetoothAdapter())
        {
            if (bluetoothAdapter == null)
            {
                Debug.LogError("Bluetooth is not supported on this device.");
                return;
            }

            if (!bluetoothAdapter.Call<bool>("isEnabled"))
            {
                // Request enabling Bluetooth if it's not already enabled
                using (AndroidJavaObject activity = GetActivity())
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        bluetoothAdapter.Call("enable");
                    }));
                }

                return;
            }
            Debug.Log("Scanning for devices ...");
            // Clear the existing device list
            deviceList.Clear();
            UpdateDeviceListText();

            // Start device discovery
            using (AndroidJavaObject bluetoothAdapterWrapper = new AndroidJavaObject("com.example.unitybluetooth.BluetoothAdapterWrapper", this.gameObject.name))
            {
                bluetoothAdapterWrapper.Call("startDiscovery");
            }
        }
    }

    private void UpdateDeviceListText()
    {
        if (deviceListText != null)
        {
            deviceListText.text = string.Join("\n", deviceList);
        }
    }

    // This method will be called when a Bluetooth device is discovered
    public void OnDeviceDiscovered(string deviceName)
    {
        Debug.Log("Device Discovered"+ deviceName);
        if (!deviceList.Contains(deviceName))
        {
            deviceList.Add(deviceName);
            UpdateDeviceListText();
        }
    }

    private AndroidJavaObject GetBluetoothAdapter()
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return activity.Call<AndroidJavaObject>("getSystemService", "bluetooth");
            }
        }
    }

    private AndroidJavaObject GetActivity()
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            Debug.Log("GetActivity");
            return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
}
