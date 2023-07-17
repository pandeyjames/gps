using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothDeviceList : MonoBehaviour
{
    private AndroidJavaObject bluetoothAdapter;
    private AndroidJavaObject activityContext;

    void Start()
    {
        // Get the current Android activity context
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // Get the default Bluetooth adapter
        AndroidJavaClass bluetoothClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        //bluetoothAdapter = bluetoothClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
        AndroidJavaObject bluetoothAdapter = GetBluetoothAdapter();
        // Check if Bluetooth is supported on the device
        if (bluetoothAdapter == null)
        {
            Debug.Log("Bluetooth is not supported on this device.");
            return;
        }

        // Check if Bluetooth is enabled
        if (!bluetoothAdapter.Call<bool>("isEnabled"))
        {
            Debug.Log("Bluetooth is not enabled.");
            return;
        }

        // Get the list of paired devices
        AndroidJavaObject pairedDevices = bluetoothAdapter.Call<AndroidJavaObject>("getBondedDevices");

        // Iterate through the paired devices
        foreach (AndroidJavaObject device in pairedDevices.Call<AndroidJavaObject[]>("toArray"))
        {
            // Get the name and address of each device
            string name = device.Call<string>("getName");
            string address = device.Call<string>("getAddress");

            Debug.Log("Name: " + name + ", Address: " + address);
        }
    }
    private AndroidJavaObject GetBluetoothAdapter()
    {
        if (bluetoothAdapter == null)
        {
            AndroidJavaClass bluetoothAdapterClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
            bluetoothAdapter = bluetoothAdapterClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
        }

        return bluetoothAdapter;
    }
}

