using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gps : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text;
    void Start()
    {
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            //Debug.Log("Latitude: " + latitude + ", Longitude: " + longitude);
            text.text = "Latitude: " + latitude + ", Longitude: " + longitude;
        }
        else
        {
            text.text = "There is a problem with the location service: " + Input.location.status;
        }
    }
}
