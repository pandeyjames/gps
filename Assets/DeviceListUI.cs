using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DeviceListUI : MonoBehaviour
{
    public TMP_Text deviceListText;

    public class DeviceData
    {
        public string address;
        public string name;

        public DeviceData(string address, string name)
        {
            this.address = address;
            this.name = name;
        }
    }

    private List<string> deviceList = new List<string>();

    public void AddDeviceToList(string deviceAddress, string deviceName)
    {
        DeviceData deviceData = new DeviceData(deviceAddress, deviceName);
        string deviceInfo = deviceData.name + " (" + deviceData.address + ")";

        if (!deviceList.Contains(deviceInfo))
        {
            deviceList.Add(deviceInfo);
            UpdateDeviceListText();
        }
    }

    private void UpdateDeviceListText()
    {
        deviceListText.text = string.Join("\n", deviceList);
    }

    public void OnDeviceDiscovered(DeviceData deviceData)
    {
        // Add your logic here to display the discovered devices in your UI or perform actions based on the discovered devices.
        Debug.Log("Device Discovered: " + deviceData.name + " (" + deviceData.address + ")");
    }
}
