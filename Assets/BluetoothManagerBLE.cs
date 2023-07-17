using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BluetoothManagerBLE : MonoBehaviour
{
    private AndroidJavaObject bluetoothAdapter;
    private AndroidJavaObject bluetoothGatt;
    private AndroidJavaObject bluetoothGattCallback;
    public TMP_Text debugText;
    private bool isConnected = false;

    // Constants for device address and UUID
    private const string DEVICE_ADDRESS = "00:00:00:00:00:00"; // Replace with your device's address
    //private const string SERVICE_UUID = "00001101-0000-1000-8000-00805F9B34FB"; // Replace with your desired service UUID
    private const string SERVICE_UUID = "0000aaa0-0000-1000-8000-aabbccddeeff";
    private const string CHARACTERISTIC_UUID = "00001101-0000-1000-8000-00805F9B34FB"; // Replace with your desired characteristic UUID
    private AndroidJavaObject myBluetoothGattServer;
    void Start()
    {
        // Check if Bluetooth is supported on the device
        if (!IsBluetoothSupported())
        {
            Debug.Log("Bluetooth is not supported on this device.");
            return;
        }

        // Check if Bluetooth is enabled
        if (!IsBluetoothEnabled())
        {
            Debug.Log("Bluetooth is not enabled.");
            return;
        }
        startGattServer();
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

        // Connect to the Bluetooth device
        //ConnectToDevice(DEVICE_ADDRESS);
    }
    void startGattServer()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject bluetoothManager = new AndroidJavaObject("com.DefaultCompany.GPS.MyBluetoothGattServer", activity);
        bluetoothManager.Call("startAdvertising");
    }

    void OnDestroy()
    {
        myBluetoothGattServer.Call("stopAdvertising");
        myBluetoothGattServer.Call("close");
    }

    private AndroidJavaObject GetContext()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
    }


    public void ConnectToDevice(string address)
    {
        if (bluetoothAdapter != null)
        {
            AndroidJavaObject device = bluetoothAdapter.Call<AndroidJavaObject>("getRemoteDevice", address);
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityPlayer = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass gattCallbackClass = new AndroidJavaClass("com.DefaultCompany.GPS.MyBluetoothGattCallback"); // Replace with your own GattCallback class

            bluetoothGattCallback = gattCallbackClass.CallStatic<AndroidJavaObject>("getInstance");
            bluetoothGatt = device.Call<AndroidJavaObject>("connectGatt", unityPlayer, false, bluetoothGattCallback);

            isConnected = true;
            Debug.Log("ConnectToDevice called");
            Debug.Log("Device address: " + address);
            debugText.text = "ConnectToDevice called for this " + address;
        }
        else
        {
            Debug.Log("BluetoothAdapter is null");
            AndroidJavaClass bluetoothClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
            bluetoothAdapter = bluetoothClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
            debugText.text = "Error: bluetooth adapter is null, try again not connected to" + address;
        }
    }

    public void ConnectToGlass()
    {
        //ConnectToDevice("22:22:BD:CD:3F:6E");
        ConnectToDevice("22:22:AF:50:C3:70"); //Working GPS SOM
    }
    public void ConnectToApp()
    {
        ConnectToDevice("F8:AB:82:11:2D:BD"); //Redmi Note 11
    }
    public void DisconnectDevice()
    {
        if (bluetoothGatt != null)
        {
            bluetoothGatt.Call("disconnect");
            isConnected = false;
        }
    }
    public void SendDemoData()
    {
        AndroidJavaObject servicesList = bluetoothGatt.Call<AndroidJavaObject>("getServices");
        int serviceCount = servicesList.Call<int>("size");

        for (int i = 0; i < serviceCount; i++)
        {
            AndroidJavaObject service = servicesList.Call<AndroidJavaObject>("get", i);
            AndroidJavaObject serviceUUID = service.Call<AndroidJavaObject>("getUuid");
            string uuidString = serviceUUID.Call<string>("toString");

            Debug.Log("Service UUID: " + uuidString);
        }

        debugText.text = "Data being sent";
        SendDataToDevice("Hello World");
        
    }
    public void SendData(string data)
    {
        if (bluetoothGatt != null && isConnected)
        {
            AndroidJavaObject characteristic = bluetoothGatt
                .Call<AndroidJavaObject>("getService", SERVICE_UUID)
                .Call<AndroidJavaObject>("getCharacteristic", CHARACTERISTIC_UUID);

            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            characteristic.Call("setValue", dataBytes);
            bluetoothGatt.Call("writeCharacteristic", characteristic);
        }
    }

    public void SendDataToDevice(string data)
    {
        Debug.Log(data + " is being sent to the connected device.");
        if (bluetoothGatt != null)
        {
            AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

            // Convert the UUID strings to Java UUID objects
            AndroidJavaObject serviceUUID = ConvertToJavaUUID(SERVICE_UUID);
            AndroidJavaObject characteristicUUID = ConvertToJavaUUID(CHARACTERISTIC_UUID);

            // Get the BluetoothGattService with the UUID you want
            AndroidJavaObject bluetoothGattService = bluetoothGatt.Call<AndroidJavaObject>("getService", serviceUUID);

            if (bluetoothGattService != null)
            {
                // Get the BluetoothGattCharacteristic with the UUID you want
                AndroidJavaObject characteristic = bluetoothGattService.Call<AndroidJavaObject>("getCharacteristic", characteristicUUID);

                if (characteristic != null)
                {
                    // Set the value of the characteristic
                    byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
                    characteristic.Call("setValue", dataBytes);

                    // Write the characteristic to the device
                    bluetoothGatt.Call("writeCharacteristic", characteristic);
                }
                else
                {
                    Debug.Log("Error occurred: Characteristic is null.");
                }
            }
            else
            {
                Debug.Log("Error occurred: BluetoothGattService is null.");
            }
        }
        else
        {
            Debug.Log("Error occurred: BluetoothGatt is null.");
        }
    }


    public void ReadData()
    {
        if (bluetoothGatt != null && isConnected)
        {
            AndroidJavaObject characteristic = bluetoothGatt
                .Call<AndroidJavaObject>("getService", SERVICE_UUID)
                .Call<AndroidJavaObject>("getCharacteristic", CHARACTERISTIC_UUID);

            bluetoothGatt.Call("readCharacteristic", characteristic);
        }
    }

    public void OnCharacteristicRead(AndroidJavaObject characteristic, int status, byte[] data)
    {
        AndroidJavaClass bluetoothGattClass = new AndroidJavaClass("android.bluetooth.BluetoothGatt");
        int gattSuccess = bluetoothGattClass.GetStatic<int>("GATT_SUCCESS");

        if (status == gattSuccess)
        {
            string dataString = System.Text.Encoding.UTF8.GetString(data);
            OnDataReceived(dataString);
        }
    }


    public void OnDataReceived(string data)
    {
        debugText.text = "Received data: " + data;
    }
    private AndroidJavaObject UUIDFromString(string uuidString)
    {
        AndroidJavaObject uuid = new AndroidJavaObject("java.util.UUID", new AndroidJavaObject("java.lang.String", uuidString));
        return uuid;
    }
    private AndroidJavaObject ConvertToJavaUUID(string uuidString)
    {
        AndroidJavaClass javaUUIDClass = new AndroidJavaClass("java.util.UUID");
        return javaUUIDClass.CallStatic<AndroidJavaObject>("fromString", uuidString);
    }


    private bool IsBluetoothSupported()
    {
        AndroidJavaClass bluetoothClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        bluetoothAdapter = bluetoothClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
        return bluetoothAdapter != null;
    }

    private bool IsBluetoothEnabled()
    {
        return bluetoothAdapter.Call<bool>("isEnabled");
    }

}
