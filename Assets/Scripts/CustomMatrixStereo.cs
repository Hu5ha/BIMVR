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
using System.Collections;

[ExecuteInEditMode]
public class CustomMatrixStereo : MonoBehaviour
{
    [Header("Screen Size")]
    public float screenWidth = 3.33f;
    public float screenHeight = 2.5f;
    
    [Header("Cameras")]
    public Camera leftCamera;
    public Camera rightCamera;
    private Transform rightCamTransform;
    private Transform leftCamTransform;

    [Header("Camera Viewport Rect")]
    public float rectWidth;
    public float rectXPosition;
    public float rectHeight = 1;
    public float rectYPosition = 0;

    [Header("Other settings")]
    public bool geometryCorrection = false;
    public bool drawScreen = true;
    public bool updateMatrix = true;

    private Transform myTransform;

     #region matrix variables
    private Vector3 windowTopLeft = Vector3.zero;
    private Vector3 windowTopRight = Vector3.zero;
    private Vector3 windowBottomLeft = Vector3.zero;
    private Vector3 windowBottomRight = Vector3.zero;

    private float left;
    private float right;
    private float bottom;
    private float top;

    private float x;
    private float y;
    private float a;
    private float b;
    private float c;
    private float d;
    private float e;

    private Matrix4x4 m;
    #endregion

    // Use this for initialization
    void Start()
    {
        myTransform = transform;

        if (leftCamera)
        {
            leftCamTransform = leftCamera.transform;
        }
        if (rightCamera)
        {
            rightCamTransform = rightCamera.transform;
        }
    }

    void Update()
    {
        if (leftCamTransform == null && leftCamera)
        {
            leftCamTransform = leftCamera.transform;
        }
        if (rightCamTransform == null && rightCamera)
        {
            rightCamTransform = rightCamera.transform;
        }

        if (leftCamera != null && updateMatrix)
        {
            leftCamTransform.rotation = myTransform.rotation;
            SetCameraMatrix(leftCamera, leftCamera.nearClipPlane, leftCamera.farClipPlane, GetRelativePosition(leftCamTransform.position), screenWidth, screenHeight);
        }

        if (rightCamera != null && updateMatrix)
        {
            rightCamTransform.rotation = myTransform.rotation;
            SetCameraMatrix(rightCamera, rightCamera.nearClipPlane, rightCamera.farClipPlane, GetRelativePosition(rightCamTransform.position), screenWidth, screenHeight);
        }

        if (drawScreen)
        {
            //Calculate screen corners relative to center
            windowTopLeft = myTransform.position + (myTransform.up * screenHeight * 0.5f) + (-myTransform.right * screenWidth * 0.5f);
            windowTopRight = myTransform.position + (myTransform.up * screenHeight * 0.5f) + (myTransform.right * screenWidth * 0.5f);
            windowBottomLeft = myTransform.position + (-myTransform.up * screenHeight * 0.5f) + (-myTransform.right * screenWidth * 0.5f);
            windowBottomRight = myTransform.position + (-myTransform.up * screenHeight * 0.5f) + (myTransform.right * screenWidth * 0.5f);

            //Draw lines in editor window to visualize screen
            Debug.DrawLine(windowTopLeft, windowTopRight);
            Debug.DrawLine(windowTopLeft, windowBottomLeft);
            Debug.DrawLine(windowTopRight, windowBottomRight);
            Debug.DrawLine(windowBottomLeft, windowBottomRight);

            Debug.DrawLine(leftCamTransform.position, windowTopRight);
            Debug.DrawLine(leftCamTransform.position, windowTopLeft);
            Debug.DrawLine(leftCamTransform.position, windowBottomRight);
            Debug.DrawLine(leftCamTransform.position, windowBottomLeft);

            Debug.DrawLine(rightCamTransform.position, windowTopRight);
            Debug.DrawLine(rightCamTransform.position, windowTopLeft);
            Debug.DrawLine(rightCamTransform.position, windowBottomRight);
            Debug.DrawLine(rightCamTransform.position, windowBottomLeft);
        }
    }

    Vector3 GetRelativePosition(Vector3 worldPosition)
    {
        Vector3 viewerRelativePosition = Vector3.zero;
        Vector3 viewerDirection = worldPosition - myTransform.position;
        viewerRelativePosition.z = Vector3.Dot(-leftCamTransform.forward, viewerDirection);
        viewerRelativePosition.x = Vector3.Dot(-leftCamTransform.right, viewerDirection);
        viewerRelativePosition.y = Vector3.Dot(leftCamTransform.up, viewerDirection);

        return viewerRelativePosition;

    }

    void SetCameraMatrix(Camera cam, float nearClipPlane, float farClipPlane, Vector3 viewerRelativePosition, float screenWidth, float screenHeight)
    {
        left = -(screenWidth / 2) + viewerRelativePosition.x;
        right = left + screenWidth;
        bottom = -(screenHeight / 2) - viewerRelativePosition.y;
        top = bottom + screenHeight;

        x = (2.0f * viewerRelativePosition.z) / (right - left);
        y = (2.0f * viewerRelativePosition.z) / (top - bottom);
        a = (right + left) / (right - left);
        b = (top + bottom) / (top - bottom);
        c = -(farClipPlane + nearClipPlane) / (farClipPlane - nearClipPlane);
        d = -(2.0f * farClipPlane * nearClipPlane) / (farClipPlane - nearClipPlane);
        e = -1.0f;

        m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;

        if (Application.isEditor)
        {
            cam.projectionMatrix = m;
        }
        else
        {

            if (cam.stereoTargetEye == StereoTargetEyeMask.Left)
            {
                cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, m);
            }
            else if (cam.stereoTargetEye == StereoTargetEyeMask.Right)
            {
                cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, m);
            }
        }
    }

}
