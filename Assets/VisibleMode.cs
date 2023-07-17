using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleMode : MonoBehaviour
{
    private AndroidJavaObject bluetoothAdapter;

    void Start()
    {
        // Get the default Bluetooth adapter
        AndroidJavaClass androidClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        bluetoothAdapter = androidClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");

        // Check if Bluetooth is supported on the device
        if (bluetoothAdapter == null)
        {
            Debug.Log("Bluetooth is not supported on this device.");
            return;
        }

        // Enable device discoverability
        //EnableDiscoverability();
    }

    public void EnableDiscoverability()
    {
        // Enable discoverability for a fixed duration (e.g., 300 seconds)
        int discoverableDuration = 300;

        // Create an intent to enable discoverability
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", "android.bluetooth.adapter.action.REQUEST_DISCOVERABLE");
        intent.Call<AndroidJavaObject>("putExtra", "android.bluetooth.adapter.extra.DISCOVERABLE_DURATION", discoverableDuration);

        // Start the activity to enable discoverability
        AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intent);
    }
}

