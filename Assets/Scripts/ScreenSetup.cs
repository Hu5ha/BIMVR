/* 
 * Copyright (c) 2020, Collaprime Oy.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information
 * of the Collaprime Oy.
 * You shall not disclose such Confidential Information and shall use
 * it only in accordance with the terms of the license agreement
 * you entered into with the Collaprime Oy.
 *
 * http://www.collaprime.com
 */

using UnityEngine;
using UnityEngine.XR;
using System;
using System.IO;
using System.Collections.Generic;


[ExecuteInEditMode]
public class ScreenSetup : MonoBehaviour
{
    private GameObject screen;
    private GameObject cameras;

    [Header("Prefabs")]
    public GameObject screenPrefab;
    public GameObject camerasPrefab;
    public GameObject VRPrefab;

    [Header("Parents")]
    public Transform screenParent;
    public Transform camerasParent;

    [Header("Head")]
    public float headHeight;
    [Tooltip("Head distance from the screen. Use negative value.")]
    public float headDistance;
    [Tooltip("Multiplier value between 0 and 1. Used to match camera rotation with screen rotation. \n\nAt 0 cameras are facing front, at 1 cameras match screen's rotation.")]
    [Range(0, 1)] public float eyeFacingAdjustment;

    [Header("UI")]
    public Camera UICamera;
    public float UIWidth = 1;
    public float UIHeight = 1;
    public float UIPosX = 0;
    public float UIPosY = 0;

    [Header("Other Settings")]
    public bool VRHmdDetection = false;
    public string directory = "c:\\Collaprime\\";
    public string fileName = "ScreenSetup.json";
    public List<CustomMatrixStereo> screens;


    private void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Clone"))
        {
            GameObject.DestroyImmediate(go);
        }
        screens = new List<CustomMatrixStereo>();
        LoadSettingsFromJSON(directory + fileName);

        //Debug.Log("XR Device Present: " + XRDevice.isPresent);
        //Debug.Log("XR Model: " + XRDevice.model);
        //Debug.Log("XR Device Active: " + XRSettings.isDeviceActive);

        if (VRHmdDetection && XRDevice.model != "")
        {
            Debug.Log("VR Device found!");
            Instantiate(VRPrefab);
            this.gameObject.SetActive(false);
            Debug.Log("XR Enabled: " + XRSettings.enabled);
        }
        else if (VRHmdDetection)
        {
            Debug.Log("No VR device detected.");
        }
    }

    private void Update()
    {
        screenParent.localPosition = new Vector3(0, screens[0].screenHeight / 2, 0);
        screenParent.localRotation = Quaternion.Euler(0, 0, 0);
        camerasParent.localPosition = new Vector3(0, headHeight, headDistance);


        foreach (CustomMatrixStereo cms in screens)
        {
            float screenRotation = cms.transform.localEulerAngles.y;
            if (screenRotation > 180)
                screenRotation = screenRotation - 360;
            cms.leftCamera.transform.parent.localRotation = Quaternion.Euler(0, screenRotation * eyeFacingAdjustment * 1f, 0);

        }

        UICamera.rect = new Rect(UIPosX, UIPosY, UIWidth, UIHeight);
    }

    public void SaveSettingsToJSON()
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        SaveTokenizer saveToken = new SaveTokenizer(screens, headHeight, headDistance, eyeFacingAdjustment, UIWidth, UIHeight, UIPosX, UIPosY);

        File.WriteAllText(directory + fileName, JsonUtility.ToJson(saveToken, true));
        Debug.Log("Saved settings to " + directory + fileName);
    }

    public void LoadSettingsFromJSON(string fileToLoad)
    {
        if (File.Exists(fileToLoad))
        {
            Debug.Log("LoadSettingsFromJSON() Save file found. " + fileToLoad);

            string saveString = File.ReadAllText(fileToLoad);
            SaveTokenizer loadToken = JsonUtility.FromJson<SaveTokenizer>(saveString);

            headHeight = loadToken.headHeight;
            headDistance = loadToken.headDistance;
            eyeFacingAdjustment = loadToken.headRotation;

            foreach (SaveTokenizer.ScreenToken token in loadToken.screens)
            {
                AddScreen(token);
            }

            screenParent.localPosition = new Vector3(0, loadToken.screens[0].screenHeight / 2, 0);
            screenParent.localRotation = Quaternion.Euler(0, 0, 0);
            camerasParent.localPosition = new Vector3(0, headHeight, headDistance);

            UIHeight = loadToken.uiHeight;
            UIWidth = loadToken.uiWidth;
            UIPosX = loadToken.uiPosX;
            UIPosY = loadToken.uiPosY;
            UICamera.rect = new Rect(UIPosX, UIPosY, UIWidth, UIHeight);

        }
        else
        {
            Debug.Log("Save file not found. Generating single screen setup.");
            GenerateSingleScreenSetup();

        }
    }

    public void GenerateSingleScreenSetup()
    {
        //Instantiate screen prefab and add it to the CMS list
        screen = Instantiate(screenPrefab, screenParent);
        CustomMatrixStereo cms = screen.GetComponent<CustomMatrixStereo>();
        screens.Add(cms);

        //Instantiate cameras prefab and set references to the CMS script
        cameras = Instantiate(camerasPrefab, camerasParent);
        Camera leftCamera = cameras.transform.Find("LeftCamera").GetComponent<Camera>();
        Camera rightCamera = cameras.transform.Find("RightCamera").GetComponent<Camera>();
        cms.leftCamera = leftCamera;
        cms.rightCamera = rightCamera;

        //Set values from loaded cms to the instantiated cms.
        cms.screenWidth = 4.44f;
        cms.screenHeight = 2.5f;

        //Set UI Canvas values
        UIWidth = 1;
        UIHeight = 1;
        UIPosX = 0;
        UIPosY = 0;

        headDistance = -2.25f;
        headHeight = 1.7f;
    }

    public void AddScreen(SaveTokenizer.ScreenToken token)
    {
        if (token == null) throw new ArgumentNullException("Save Token is null");

        //Instantiate screen prefab and add it to the CMS list
        screen = Instantiate(screenPrefab, screenParent);
        CustomMatrixStereo cms = screen.GetComponent<CustomMatrixStereo>();
        screens.Add(cms);

        //Instantiate cameras prefab and set references to the CMS script
        cameras = Instantiate(camerasPrefab, camerasParent);
        Camera leftCamera = cameras.transform.Find("LeftCamera").GetComponent<Camera>();
        Camera rightCamera = cameras.transform.Find("RightCamera").GetComponent<Camera>();
        cms.leftCamera = leftCamera;
        cms.rightCamera = rightCamera;


        //Set values from loaded cms to the instantiated cms.
        cms.screenWidth = token.screenWidth;
        cms.screenHeight = token.screenHeight;

        cms.transform.localPosition = token.position;
        cms.transform.localRotation = token.rotation;

        cms.geometryCorrection = token.geometryCorrection;

        cms.rectHeight = token.rectHeight;
        cms.rectWidth = token.rectWidth;
        cms.rectXPosition = token.rectXPosition;
        cms.rectYPosition = token.rectYPosition;

        //Set camera rect values
        leftCamera.rect = new Rect(token.rectXPosition, token.rectYPosition, token.rectWidth, token.rectHeight);
        rightCamera.rect = new Rect(token.rectXPosition, token.rectYPosition, token.rectWidth, token.rectHeight);

        if (cms.geometryCorrection == true)
        {
            GeometryCorrection gc1 = leftCamera.gameObject.AddComponent<GeometryCorrection>();
            GeometryCorrection gc2 = rightCamera.gameObject.AddComponent<GeometryCorrection>();
            List<GeometryCorrection> geometryCorrections = new List<GeometryCorrection>() { gc1, gc2 };

            foreach (GeometryCorrection gc in geometryCorrections)
            {
                gc.corner1 = token.corner1;
                gc.corner2 = token.corner2;
                gc.corner3 = token.corner3;
                gc.corner4 = token.corner4;

                gc.leftMaskAmount = token.leftMaskAmount;
                gc.leftMaskSlope = token.leftMaskSlope;

                gc.rightMaskAmount = token.rightMaskAmount;
                gc.rightMaskSlope = token.rightMaskSlope;

                gc.gradientColor.a = token.gradientColor.a;
                gc.bottomMaskAmount = token.bottomMaskAmount;
            }
        }
    }
}
