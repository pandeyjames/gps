using UnityEngine;

[UnityEngine.Scripting.Preserve]
[UnityEngine.AddComponentMenu("")]
[UnityEngine.RequireComponent(typeof(AndroidJavaProxy))]
public class BluetoothReceiver : AndroidJavaProxy
{
    private AndroidJavaObject bluetoothManager;

    public BluetoothReceiver(AndroidJavaObject bluetoothManager)
        : base("android.content.BroadcastReceiver")
    {
        this.bluetoothManager = bluetoothManager;
    }

    public void onReceive(AndroidJavaObject context, AndroidJavaObject intent)
    {
        string action = intent.Call<string>("getAction");
        if (action.Equals("android.bluetooth.device.action.FOUND"))
        {
            string deviceAddress = intent.Call<string>("getStringExtra", "android.bluetooth.device.extra.DEVICE");
            string deviceName = intent.Call<string>("getStringExtra", "android.bluetooth.device.extra.NAME");
            bluetoothManager.Call("OnDeviceDiscovered", deviceAddress, deviceName);
        }
    }
}
