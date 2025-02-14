using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class Hotkeys : MonoBehaviour
{

    [Tooltip("Exit application")]
    public KeyCode ExitApplication = KeyCode.Escape;

    [Tooltip("Toggle stereo mode on or off")]
    public KeyCode ToggleStereo = KeyCode.F10;
    private List<GameObject> rightCameras = new List<GameObject>();
    private List<GameObject> leftCameras = new List<GameObject>();

    [Tooltip("Swaps left and right eye to correct 3D effect")]
    public KeyCode SwapEyes = KeyCode.F11;
    private bool eyesSwapped = false;

    [Tooltip("Force to 3 screen mode")]
    public KeyCode Force3Screens = KeyCode.F9;
    private ScreenSetup screenSetup;
    private bool forced3Screen = false;


    void Update()
    {
        if (Input.GetKeyDown(ExitApplication))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(ToggleStereo))
        {
            PopulateCameraLists();
            foreach (GameObject go in rightCameras)
            {
                go.SetActive(!go.activeInHierarchy);
            }
            foreach (GameObject go in leftCameras)
            {
                if (go.GetComponent<Camera>().stereoTargetEye == StereoTargetEyeMask.Both)
                {
                    if (eyesSwapped)
                    {
                        go.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
                    }
                    else
                    {
                        go.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
                    }
                } else
                {
                    go.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Both;
                }
            }    
        }

        if (Input.GetKeyDown(SwapEyes))
        {
            PopulateCameraLists();
            foreach (GameObject go in leftCameras)
            {
                SwapTargetEye(go.GetComponent<Camera>());
            }
            foreach (GameObject go in rightCameras)
            {
                SwapTargetEye(go.GetComponent<Camera>());
            }
            eyesSwapped = !eyesSwapped;
        }


        if (Input.GetKeyDown(Force3Screens))
        {
            forced3Screen = !forced3Screen;
            screenSetup = this.GetComponent<ScreenSetup>();


            if (screenSetup.screens.Count == 3 || screenSetup.screens.Count == 4) //Check if there are 3 or 4 screens to make usable only in cave setups
            {
                Debug.Log("True, count is " + screenSetup.screens.Count);

                if (forced3Screen)
                {

                    screenSetup.screens[0].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[0].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[0].leftCamera.rect = new Rect(0, 0, 0.333f, 1);
                    screenSetup.screens[0].rightCamera.rect = new Rect(0, 0, 0.333f, 1);

                    screenSetup.screens[1].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[1].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[1].leftCamera.rect = new Rect(0.333f, 0, 0.333f, 1);
                    screenSetup.screens[1].rightCamera.rect = new Rect(0.333f, 0, 0.333f, 1);

                    screenSetup.screens[2].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[2].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[2].leftCamera.rect = new Rect(0.666f, 0, 0.333f, 1);
                    screenSetup.screens[2].rightCamera.rect = new Rect(0.666f, 0, 0.333f, 1);

                    screenSetup.screens[3].leftCamera.gameObject.SetActive(false);
                    screenSetup.screens[3].rightCamera.gameObject.SetActive(false);

                    screenSetup.UICamera.rect = new Rect(0.333f, 0, 0.333f, 1);

                } else if (!forced3Screen)
                {
                    screenSetup.screens[0].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[0].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[0].leftCamera.rect = new Rect(0, 0, 0.25f, 1);
                    screenSetup.screens[0].rightCamera.rect = new Rect(0, 0, 0.25f, 1);

                    screenSetup.screens[1].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[1].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[1].leftCamera.rect = new Rect(0.25f, 0, 0.25f, 1);
                    screenSetup.screens[1].rightCamera.rect = new Rect(0.25f, 0, 0.25f, 1);

                    screenSetup.screens[2].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[2].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[2].leftCamera.rect = new Rect(0.5f, 0, 0.25f, 1);
                    screenSetup.screens[2].rightCamera.rect = new Rect(0.5f, 0, 0.25f, 1);

                    screenSetup.screens[3].leftCamera.gameObject.SetActive(true);
                    screenSetup.screens[3].rightCamera.gameObject.SetActive(true);
                    screenSetup.screens[3].leftCamera.rect = new Rect(0.75f, 0, 0.25f, 1);
                    screenSetup.screens[3].rightCamera.rect = new Rect(0.75f, 0, 0.25f, 1);

                    screenSetup.UICamera.rect = new Rect(0.25f, 0, 0.25f, 1);
                }
            }
        }
    }

    public void PopulateCameraLists()
    {
        if (rightCameras.Count == 0)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("RightCamera"))
            {
                rightCameras.Add(go);
            }
        }
        if (leftCameras.Count == 0)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("LeftCamera"))
            {
                leftCameras.Add(go);
            }
        }
    }

    public void SwapTargetEye(Camera c)
    {        
        if (c.stereoTargetEye == StereoTargetEyeMask.Left)
        {
            c.stereoTargetEye = StereoTargetEyeMask.Right;
        }
        else if (c.stereoTargetEye == StereoTargetEyeMask.Right)
        {
            c.stereoTargetEye = StereoTargetEyeMask.Left;
        }
    }
}
