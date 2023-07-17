using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class BluetoothManager : MonoBehaviour
{
    private AndroidJavaObject bluetoothAdapter;
    private AndroidJavaObject bluetoothSocket;
    private AndroidJavaObject outputStream;
    private AndroidJavaObject inputStream;
    public TMP_Text dubugText;
    public TMP_Text actionDebug;
    public GameObject deviceListUI;

    public string jsonData;
    DeviceData currentDeviceData;
    private class DeviceData
    {
        public string address;
        public string name;

        public DeviceData(string address, string name)
        {
            this.address = address;
            this.name = name;
        }
    }
    private int state;
    private int newState;
    private const string UUID_UNIQUE = "00001101-0000-1000-8000-00805F9B34FB"; // Replace with your desired UUID
    static string MY_UUID_SECURE = "fa87c0d0-afac-11de-8a39-0800200c9a66";
    static string MY_UUID_INSECURE = "8ce255c0-200a-11e0-ac64-0800200c9a66";
    private AndroidJavaObject activityContext;
    private AndroidJavaObject receiverObject;
    private bool isReceiverRegistered = false;

    private bool isBluetoothEnabled = false;
    private bool isDiscoveryStarted = false;
    private bool isConnecting = false;
    private bool isConnected = false;
    void Start()
    {
        // Get the current Android activity context
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // Get the default Bluetooth adapter
        AndroidJavaClass bluetoothClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        bluetoothAdapter = bluetoothClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");

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
            if (name.Contains("QCOM-BTD"))
            {
                currentDeviceData = new DeviceData(address, name);
                //currentDeviceData.address = address;
                //currentDeviceData.name = name;
            }
        }

        if (currentDeviceData != null)
        {
            Debug.Log("Current Name: " + currentDeviceData.name + ", Current Address: " + currentDeviceData.address);
            Debug.Log("Connecting to: " + currentDeviceData.address);
            ConnectToServer(currentDeviceData.address.ToString());
        }
    }

    public void EnableBluetooth()
    {
        if (bluetoothAdapter != null)
        {
            bluetoothAdapter.Call<bool>("enable");
            Debug.Log(bluetoothAdapter.Call<string>("getAddress"));
            isBluetoothEnabled = true;
            actionDebug.text = "Bluetooth Enabled " + bluetoothAdapter.Call<string>("getName") + " " + bluetoothAdapter.Call<string>("getAddress");
        }
        else
        {
            actionDebug.text = "Bluetooth Not Enabled";
        }
    }

    public void DisableBluetooth()
    {
        if (bluetoothAdapter != null)
        {
            bluetoothAdapter.Call<bool>("disable");
            actionDebug.text = "Bluetooth disabled";
            isBluetoothEnabled = false;
            isDiscoveryStarted = false;
            isConnecting = false;
            isConnected = false;
        }
    }

    public void DiscoverDevices()
    {
        if (bluetoothAdapter != null)
        {
            // Start device discovery
            bool startedDiscovery = bluetoothAdapter.Call<bool>("startDiscovery");
            if (startedDiscovery)
            {
                actionDebug.text = "Discovery Started";
            }
            else
            {
                actionDebug.text = "Failed to start discovery";
            }
            //MakeDiscoverable(120);
            // Register a BroadcastReceiver to receive device discovery events
            if (!isReceiverRegistered)
            {
                receiverObject = new AndroidJavaObject("com.unity3d.player.UnityPlayer$BroadcastReceiver", this.gameObject.name, "OnDeviceDiscovered");
                activityContext.Call("registerReceiver", receiverObject, new AndroidJavaObject("android.content.IntentFilter", "android.bluetooth.device.action.FOUND"));
                isReceiverRegistered = true;
            }

        }
    }

    public void OnDeviceDiscovered(string deviceAddress, string deviceName)
    {
        AddDeviceToList(deviceName, deviceAddress);
        Debug.Log("Device Discovered: " + deviceName + " (" + deviceAddress + ")");
        actionDebug.text = "Device Discovered: " + deviceName + " (" + deviceAddress + ")";
        // Add your logic here to display the discovered devices in your UI or perform actions based on the discovered devices.
        deviceListUI.SendMessage("OnDeviceDiscovered", new DeviceData(deviceAddress, deviceName));
    }
    public void AddDeviceToList(string deviceName, string deviceAddress)
    {
        // Create a new UI element for the device
        GameObject deviceUI = Instantiate(deviceListUI, deviceListUI.transform.parent);

        // Get the text components of the device UI
        TMP_Text deviceNameText = deviceUI.GetComponentInChildren<TMP_Text>();
        TMP_Text deviceAddressText = deviceUI.GetComponentInChildren<TMP_Text>();

        // Set the name and address text
        deviceNameText.text = deviceName;
        deviceAddressText.text = deviceAddress;

        // Set the device UI as active
        deviceUI.SetActive(true);
    }

    private void OnDestroy()
    {
        // Unregister the BroadcastReceiver when the script is destroyed
        if (isReceiverRegistered)
        {
            activityContext.Call("unregisterReceiver", receiverObject);
            isReceiverRegistered = false;
        }
    }

    public void ConnectGlass()
    {
        if (bluetoothAdapter != null)
        {
            AndroidJavaClass bluetoothAdapterClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
            int connectionState = bluetoothAdapterClass.GetStatic<int>("STATE_CONNECTED");
            if (connectionState == bluetoothAdapter.Call<int>("getConnectionState"))
            {
                Debug.Log("Bluetooth is already connected.");
                return;
            }
            else
            {
                actionDebug.text = "Making Discoverable ... connecting to 22:22:BD:CD:3F:6E";
                ConnectToDevice("22:22:BD:CD:3F:6E");
            }
            
        }
        else
        {
            actionDebug.text = "Bluetooth is not ready.";
           
        }


    }
    public void ConnectApp()
    {
        if (bluetoothAdapter != null)
        {
            AndroidJavaClass bluetoothAdapterClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
            int connectionState = bluetoothAdapterClass.GetStatic<int>("STATE_CONNECTED");
            if (connectionState == bluetoothAdapter.Call<int>("getConnectionState"))
            {
                Debug.Log("Bluetooth is already connected.");
                return;
            }
            else
            {
                actionDebug.text = "Making Discoverable ... connecting to F8:AB:82:11:2D:BD";
                ConnectToDevice("F8:AB:82:11:2D:BD");
            }
            
        }
        else
        {
            actionDebug.text = "Bluetooth is not ready.";
            //ConnectToServer("F8:AB:82:11:2D:BD");
        }
        

    }
    public void ConnectToServer(string serverAddress)
    {

        AndroidJavaObject bluetoothAdapter = GetBluetoothAdapter();
        if (bluetoothAdapter != null)
        {

            AndroidJavaObject device = bluetoothAdapter.Call<AndroidJavaObject>("getRemoteDevice", serverAddress);
            AndroidJavaClass javaUtilUUID = new AndroidJavaClass("java.util.UUID");
            //AndroidJavaObject uuid = new AndroidJavaObject("java.util.UUID", UUID_UNIQUE);
            AndroidJavaObject uuid = javaUtilUUID.CallStatic<AndroidJavaObject>("fromString", UUID_UNIQUE);

            try
            {
                bluetoothSocket = device.Call<AndroidJavaObject>("createRfcommSocketToServiceRecord", uuid);
                bluetoothSocket.Call("connect");

                outputStream = bluetoothSocket.Call<AndroidJavaObject>("getOutputStream");
                inputStream = bluetoothSocket.Call<AndroidJavaObject>("getInputStream");
            }
            catch (AndroidJavaException ex)
            {
                Debug.LogError("Failed to connect to server: " + ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                Debug.LogError("IO Exception during connection: " + ex.Message);
            }
        }
    }

    public void ConnectToDevice(string address)
    {
        if (bluetoothAdapter != null && isBluetoothEnabled && !isConnecting && !isConnected)
        {
            AndroidJavaObject device = bluetoothAdapter.Call<AndroidJavaObject>("getRemoteDevice", address);
            AndroidJavaClass javaUtilUUID = new AndroidJavaClass("java.util.UUID");
            AndroidJavaObject uuid = javaUtilUUID.CallStatic<AndroidJavaObject>("fromString", UUID_UNIQUE);

            try
            {
                bluetoothSocket = device.Call<AndroidJavaObject>("createRfcommSocketToServiceRecord", uuid);
                bluetoothSocket.Call("connect");

                outputStream = bluetoothSocket.Call<AndroidJavaObject>("getOutputStream");
                inputStream = bluetoothSocket.Call<AndroidJavaObject>("getInputStream");

                isConnecting = true;
                actionDebug.text = "Connecting to device...";
            }
            catch (AndroidJavaException ex)
            {
                Debug.LogError("Failed to connect to server: " + ex.Message);
                actionDebug.text = "Failed to connect to device.";
                isConnecting = false;
            }
            catch (System.IO.IOException ex)
            {
                Debug.LogError("IO Exception during connection: " + ex.Message);
                actionDebug.text = "Failed to connect to device.";
                isConnecting = false;
            }
        }
    }

    public void SendData(string jsonData)
    {
        if (outputStream != null)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            outputStream.Call("write", dataBytes);
            actionDebug.text = "Sending Data";
        }
    }

    public string ReceiveData()
    {
        if (inputStream != null)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = inputStream.Call<int>("read", buffer);
            if (bytesRead > 0)
            {
                string receivedData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                actionDebug.text = "Data Received";
                return receivedData;
            }
        }

        return null;
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
    void Update()
    {
        if (inputStream != null)
        {
            dubugText.text = ReceiveData();
        }
    }
}

