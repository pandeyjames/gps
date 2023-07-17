using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Attach this script to a GameObject in your Unity scene

public class GPSManager : MonoBehaviour
{
    public TMP_Text latitudeText;
    public TMP_Text longitudeText;
    public TMP_Text accuracyText;

    public float accuracyThreshold = 10f; // Customize the accuracy threshold for triggering an action

    private bool isLocationInitialized = false;


    void Start()
    {
        // Check if GPS is available on the device
        /*if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled on the device.");
            return;
        }*/
        // Start the location service asynchronously
        StartCoroutine(StartLocationService());

    }
    IEnumerator StartLocationService()
    {
        // Start the location service
        Input.location.Start();

        // Wait until the location service initializes
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        // Check if the location service has started successfully
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // Location service is now active
            isLocationInitialized = true;
            Debug.Log("Location service started successfully.");

            // Disable the passive and network providers
            // Check for available location providers
            
        }
        else
        {
            Debug.Log("Failed to start location service.");
        }
    }

    void Update()
    {
        UpdateGps();
    }
    void  UpdateGps()
    {
        // Check if location service is running and initialized
        if (isLocationInitialized && Input.location.status == LocationServiceStatus.Running)
        {
            // Get the current GPS location data
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            float accuracy = Input.location.lastData.horizontalAccuracy;

            // Update the UI text fields with the GPS data
            latitudeText.text = "Latitude: " + latitude.ToString();
            longitudeText.text = "Longitude: " + longitude.ToString();
            accuracyText.text = "Accuracy: " + accuracy.ToString() + " m";

            // Check if accuracy meets the threshold for triggering an action
            if (accuracy <= accuracyThreshold)
            {
                PerformAction(); // Call a custom method to perform the desired action
            }
            
        }
    }

    private void OnDisable()
    {
        // Stop the location service when the script is disabled or the application is closed
        if (isLocationInitialized)
        {
            Input.location.Stop();
            isLocationInitialized = false;
        }
    }

    private void PerformAction()
    {
        Debug.Log("GPS accuracy threshold reached. Performing action...");
        // Implement your custom logic or action here based on the GPS data
    }
}

