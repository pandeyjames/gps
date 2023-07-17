package com.DefaultCompany.GPS;
import com.unity3d.player.UnityPlayer;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;

import java.io.UnsupportedEncodingException;

public class MyBluetoothGattCallback extends BluetoothGattCallback {

    private static MyBluetoothGattCallback instance = null;

    private MyBluetoothGattCallback() {
        // Private constructor to enforce singleton pattern
    }

    public static synchronized MyBluetoothGattCallback getInstance() {
        if (instance == null) {
            instance = new MyBluetoothGattCallback();
        }
        return instance;
    }

    @Override
    public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {
        // Handle connection state changes here
        if (newState == BluetoothGatt.STATE_CONNECTED) {
            // Device connected
        } else if (newState == BluetoothGatt.STATE_DISCONNECTED) {
            // Device disconnected
        }
    }

    @Override
	public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
		super.onCharacteristicRead(gatt, characteristic, status);

		if (status == BluetoothGatt.GATT_SUCCESS) {
			byte[] data = characteristic.getValue();
			String dataString;
			try {
				dataString = new String(data, "UTF-8");
			} catch (UnsupportedEncodingException e) {
				e.printStackTrace();
				return;
			}

			//UnityPlayer.UnitySendMessage("BluetoothManagerBLE", "OnDataReceived", dataString);
			//UnityPlayer.UnitySendMessage("BluetoothManagerBLE", "OnCharacteristicRead", characteristic.getUuid().toString() + "|" + dataString);

		}
	}

    @Override
	public void onCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
		super.onCharacteristicWrite(gatt, characteristic, status);

		if (status == BluetoothGatt.GATT_SUCCESS) {
			// Characteristic write successful
			byte[] data = characteristic.getValue();
			// Process the written data or perform any necessary actions
		} else {
			// Characteristic write failed
		}
	}
	@Override
	public void onServicesDiscovered(BluetoothGatt gatt, int status) {
		if (status == BluetoothGatt.GATT_SUCCESS) {
			// Services discovered successfully
			// Access services and characteristics here
		} else {
			// Error occurred while discovering services
		}
	}

	@Override
	public void onDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, int status) {
		if (status == BluetoothGatt.GATT_SUCCESS) {
			// Descriptor write successful
			// Perform any necessary actions
		} else {
			// Descriptor write failed
		}
	}


}
