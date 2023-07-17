package com.DefaultCompany.GPS; // Replace with your actual package name

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class HomeButtonRemap extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent.getAction().equals(Intent.ACTION_CLOSE_SYSTEM_DIALOGS)) {
            String reason = intent.getStringExtra("reason");
            if (reason != null && reason.equals("homekey")) {
                // Perform custom action here
            }
        }
    }
}
