using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class BluetoothCommunication : MonoBehaviour
{
    private AndroidJavaObject bluetoothHelper;

    void Start()
    {
        // Create an instance of the BluetoothHelper class
        // Create an instance of the BluetoothHelper class
        // Create an instance of the BluetoothHelper class
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        bluetoothHelper = new AndroidJavaObject("com.DefaultCompany.GPS.BluetoothHelper", activity);

        // Start the Bluetooth server
        bluetoothHelper.Call("startServer");
        ConnectToDevice("F8:AB:82:11:2D:BD");
    }

    public void ConnectToDevice(string deviceAddress)
    {
        // Get the BluetoothDevice object based on the device address
        // Get the BluetoothDevice object from the address
        AndroidJavaObject bluetoothAdapter = bluetoothHelper.Get<AndroidJavaObject>("bluetoothAdapter");
        AndroidJavaObject device = bluetoothAdapter.Call<AndroidJavaObject>("getRemoteDevice", deviceAddress);

        // Call the connectToDevice method of BluetoothHelper
        bluetoothHelper.Call("connectToDevice", device);
    }

    public void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        bluetoothHelper.Call("write", data);
    }

    public void Write(byte[] data)
    {
        // Call the write method of BluetoothHelper
        bluetoothHelper.Call("write", data);
    }

    public void ReceiveMessage(byte[] data)
    {
        string message = Encoding.UTF8.GetString(data);
        Debug.Log("Received message: " + message);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 50), "Send Message"))
        {
            SendMessage("Hello, Bluetooth!");
        }
    }
}
