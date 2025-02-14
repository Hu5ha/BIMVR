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
using System;
using System.Collections.Generic;

[Serializable]
public class SaveTokenizer
{
    public float headHeight;
    public float headDistance;
    public float headRotation;

    public float uiWidth;
    public float uiHeight;
    public float uiPosX;
    public float uiPosY;

    public List<ScreenToken> screens;

    [Serializable]
    public class ScreenToken
    {
        public float screenWidth;
        public float screenHeight;

        public Vector3 position;
        public Quaternion rotation;

        public bool geometryCorrection;

        //Camera coverage
        public float rectWidth;
        public float rectXPosition;
        public float rectHeight;
        public float rectYPosition;

        //GeometryCorrection
        public Vector2 corner1;
        public Vector2 corner2;
        public Vector2 corner3;
        public Vector2 corner4;

        public Color gradientColor;

        public float rightMaskSlope;
        public float rightMaskAmount;

        public float leftMaskSlope;
        public float leftMaskAmount;

        public float bottomMaskAmount;
    }
    

    public SaveTokenizer(List<CustomMatrixStereo> screens_)
    {
        screens = new List<ScreenToken>();
        foreach (CustomMatrixStereo cms in screens_)
        {
            screens.Add(Tokenize(cms));
        }
    }

    public SaveTokenizer(List<CustomMatrixStereo> screens_, float headHeight_, float headDistance_, float headRotation_)
    {
        headHeight = headHeight_;
        headDistance = headDistance_;
        headRotation = headRotation_;
        screens = new List<ScreenToken>();
        foreach (CustomMatrixStereo cms in screens_)
        {
            screens.Add(Tokenize(cms));
        }
    }

    public SaveTokenizer(List<CustomMatrixStereo> screens_, float headHeight_, float headDistance_, float headRotation_, float uiWidth_, float uiHeight_, float uiPosX_, float uiPosY_)
    {
        headHeight = headHeight_;
        headDistance = headDistance_;
        headRotation = headRotation_;
        screens = new List<ScreenToken>();
        foreach (CustomMatrixStereo cms in screens_)
        {
            screens.Add(Tokenize(cms));
        }

        uiWidth = uiWidth_;
        uiHeight = uiHeight_;
        uiPosX = uiPosX_;
        uiPosY = uiPosY_;
    }

    public ScreenToken Tokenize(CustomMatrixStereo cms)
    {
        var token = new ScreenToken();
        token.screenWidth = cms.screenWidth;
        token.screenHeight = cms.screenHeight;

        //Original, breaks screen setup if moved and saved
        //token.position = cms.transform.position;
        //token.rotation = cms.transform.rotation;
        //Fix?
        token.position = cms.transform.localPosition;
        token.rotation = cms.transform.localRotation;

        token.geometryCorrection = cms.geometryCorrection;

        token.rectWidth = cms.rectWidth;
        token.rectXPosition = cms.rectXPosition;
        token.rectHeight = cms.rectHeight;
        token.rectYPosition = cms.rectYPosition;
        
        if (cms.geometryCorrection == true)
        {
            token.corner1 = cms.leftCamera.GetComponent<GeometryCorrection>().corner1;
            token.corner2 = cms.leftCamera.GetComponent<GeometryCorrection>().corner2;
            token.corner3 = cms.leftCamera.GetComponent<GeometryCorrection>().corner3;
            token.corner4 = cms.leftCamera.GetComponent<GeometryCorrection>().corner4;
            
            token.gradientColor = cms.leftCamera.GetComponent<GeometryCorrection>().gradientColor;

            token.rightMaskSlope = cms.leftCamera.GetComponent<GeometryCorrection>().rightMaskSlope;
            token.leftMaskSlope = cms.leftCamera.GetComponent<GeometryCorrection>().leftMaskSlope;

            token.leftMaskAmount = cms.leftCamera.GetComponent<GeometryCorrection>().leftMaskAmount;
            token.rightMaskAmount = cms.leftCamera.GetComponent<GeometryCorrection>().rightMaskAmount;

            token.bottomMaskAmount = cms.leftCamera.GetComponent<GeometryCorrection>().bottomMaskAmount;
        }
        return token;
    }
}
