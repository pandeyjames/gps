package com.DefaultCompany.GPS;

import com.unity3d.player.UnityPlayer;

public class UnityBridge {
    public static void sendDataToUnity(String data) {
        UnityPlayer.UnitySendMessage("BluetoothBLE", "OnDataReceived", data);
    }
}
