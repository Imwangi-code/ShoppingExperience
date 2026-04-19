using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName; // Type the name of your scene here in the Inspector
    public XRNode controller = XRNode.RightHand; // Usually A is on the Right Hand

    private bool isPressed = false;

    void Update()
    {
        // Get the device (Right Controller)
        InputDevice device = InputDevices.GetDeviceAtXRNode(controller);

        // Check if the 'PrimaryButton' (A button) is being pressed
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressedNow))
        {
            if (pressedNow && !isPressed)
            {
                isPressed = true;
                LoadNextScene();
            }
            else if (!pressedNow)
            {
                isPressed = false;
            }
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Scene Name is empty! Please type the name of the scene in the Inspector.");
        }
    }
}