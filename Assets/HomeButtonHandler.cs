using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonHandler : MonoBehaviour
{
    private bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private bool WantsToQuit()
    {
        if (isQuitting)
        {
            // Perform custom action when the home button is pressed
            Debug.Log("Home button was pressed!");
            return false;
            // Optionally, prevent the app from quitting immediately
            // by returning false from this method.
        }

        return true;
    }

    private void OnEnable()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= WantsToQuit;
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Code to handle onPause
            Debug.Log("onPause from Homebutton Handler");
        }
        else
        {
            // Code to handle onResume
            Debug.Log("onResume from Homebutton Handler");
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            // Code to handle onFocus
            Debug.Log("onFocus from Homebutton Handler");
        }
    }

    private void OnNewIntent(AndroidJavaObject intent)
    {
        // Code to handle onNewIntent
        Debug.Log("onNewIntent from Homebutton Handler");
    }
}

