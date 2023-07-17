// MyBluetoothGattServer.java

package com.DefaultCompany.GPS;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattServer;
import android.bluetooth.BluetoothGattServerCallback;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothProfile;
import android.content.Context;
import android.bluetooth.BluetoothGatt;

import android.bluetooth.le.AdvertiseCallback; // Add this import
import android.bluetooth.le.AdvertiseData;
import android.bluetooth.le.AdvertiseSettings;
import android.os.ParcelUuid;
import android.util.Log; // Add this import

import java.util.UUID;
import android.content.pm.PackageManager;


public class MyBluetoothGattServer {

    private Context context;
    private BluetoothManager bluetoothManager;
    private BluetoothAdapter bluetoothAdapter;
    private BluetoothGattServer gattServer;
    private BluetoothGattService service;
    private BluetoothGattCharacteristic characteristic;


    //private static final UUID SERVICE_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	
	private static final UUID SERVICE_UUID = UUID.fromString("0000aaa0-0000-1000-8000-aabbccddeeff");
    private static final UUID CHARACTERISTIC_UUID = UUID.fromString("00001102-0000-1000-8000-00805F9B34FB");

    public MyBluetoothGattServer(Context context) {
        this.context = context;
        bluetoothManager = (BluetoothManager) context.getSystemService(Context.BLUETOOTH_SERVICE);
        bluetoothAdapter = bluetoothManager.getAdapter();
        gattServer = bluetoothManager.openGattServer(context, gattServerCallback);
        setupService();
		if (!isBleSupported()) {
            Log.e("MyBluetoothGattServer", "BLE is not supported on this device");
            return;
        }
    }

    private void setupService() {
        service = new BluetoothGattService(SERVICE_UUID, BluetoothGattService.SERVICE_TYPE_PRIMARY);

        characteristic = new BluetoothGattCharacteristic(
                CHARACTERISTIC_UUID,
                BluetoothGattCharacteristic.PROPERTY_READ | BluetoothGattCharacteristic.PROPERTY_WRITE,
                BluetoothGattCharacteristic.PERMISSION_READ | BluetoothGattCharacteristic.PERMISSION_WRITE
        );

        characteristic.setValue("Hello, GATT Server!".getBytes());

        service.addCharacteristic(characteristic);

        gattServer.addService(service);
    }

    private final BluetoothGattServerCallback gattServerCallback = new BluetoothGattServerCallback() {
        @Override
        public void onConnectionStateChange(BluetoothDevice device, int status, int newState) {
            if (newState == BluetoothProfile.STATE_CONNECTED) {
                // Device connected
            } else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
                // Device disconnected
            }
        }

        @Override
		public void onCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic) {
			gattServer.sendResponse(device, requestId, BluetoothGatt.GATT_SUCCESS, offset, characteristic.getValue());
		}

		@Override
		public void onCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, boolean preparedWrite, boolean responseNeeded, int offset, byte[] value) {
			characteristic.setValue(value);
			gattServer.sendResponse(device, requestId, BluetoothGatt.GATT_SUCCESS, offset, value);
		}

    };

		public void startAdvertising() {
		bluetoothAdapter.setName("My GATT Server");

		AdvertiseSettings settings = new AdvertiseSettings.Builder()
			.setAdvertiseMode(AdvertiseSettings.ADVERTISE_MODE_LOW_POWER)
			.setConnectable(true)
			.setTimeout(0)
			.setTxPowerLevel(AdvertiseSettings.ADVERTISE_TX_POWER_MEDIUM)
			.build();

		AdvertiseData data = new AdvertiseData.Builder()
			.setIncludeDeviceName(true)
			.addServiceUuid(new ParcelUuid(SERVICE_UUID))
			.build();

		bluetoothAdapter.getBluetoothLeAdvertiser().startAdvertising(settings, data, new AdvertiseCallback() {
			@Override
			public void onStartSuccess(AdvertiseSettings settingsInEffect) {
				// Advertising started successfully
				Log.d("MyBluetoothGattServer", "BLE advertising started");
			}

			@Override
			public void onStartFailure(int errorCode) {
				// Failed to start advertising
				Log.e("MyBluetoothGattServer", "BLE advertising failed with error code: " + errorCode);
			}
		});
	}
	private final AdvertiseCallback advertiseCallback = new AdvertiseCallback() {
    @Override
    public void onStartSuccess(AdvertiseSettings settingsInEffect) {
        Log.d("MyBluetoothGattServer", "BLE advertising started");
    }

    @Override
    public void onStartFailure(int errorCode) {
        Log.e("MyBluetoothGattServer", "BLE advertising failed with error code: " + errorCode);
    }
};



    public void stopAdvertising() {
        bluetoothAdapter.getBluetoothLeAdvertiser().stopAdvertising(null);
    }

    public void close() {
        gattServer.close();
    }
	
	private boolean isBleSupported() {
		return context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE);
	}

}
