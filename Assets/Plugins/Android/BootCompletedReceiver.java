package com.DefaultCompany.GPS;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import com.unity3d.player.UnityPlayerActivity;

public class BootCompletedReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent.getAction().equals(Intent.ACTION_BOOT_COMPLETED)) {
            // Start the UnityPlayerActivity on boot completed
            Intent unityIntent = new Intent(context, UnityPlayerActivity.class);
            unityIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            context.startActivity(unityIntent);
        }
    }
}
