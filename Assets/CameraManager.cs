using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject caveCameraRoot; // CAVE camera root
    public GameObject viveCameraRig; // Vive Camera Rig

    public static bool isViveMode = false; // Tracks active mode

    void Update()
    {
        // Keyboard switching between modes
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchToCAVE();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SwitchToVive();
        }
    }

    public void SwitchToCAVE()
    {
        isViveMode = false;
        caveCameraRoot.SetActive(true);
        viveCameraRig.SetActive(false);
        Debug.Log("Switched to CAVE mode.");
    }

    public void SwitchToVive()
    {
        isViveMode = true;
        caveCameraRoot.SetActive(false);
        viveCameraRig.SetActive(true);
        Debug.Log("Switched to HTC Vive mode.");
    }
}