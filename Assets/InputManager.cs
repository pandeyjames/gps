using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if the home button (KeyCode.Escape) is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Treat the home button as the confirm button
            HandleConfirmButton();
        }
    }

    void HandleConfirmButton()
    {
        // Perform the desired action when the confirm button is pressed
        Debug.Log("Confirm button pressed!");
    }
}

