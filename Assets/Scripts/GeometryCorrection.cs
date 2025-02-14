/* 
 * Copyright (c) 2019, Collaprime Oy.
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
public class GeometryCorrection : MonoBehaviour
{

    public Vector2 corner1 = new Vector2(0, 1);
    public Vector2 corner2 = new Vector2(1, 1);
    public Vector2 corner3 = new Vector2(0, 0);
    public Vector2 corner4 = new Vector2(1, 0);

    public Color gradientColor = new Color(0, 0, 0, 0.3f);

    public float rightMaskSlope = 0;
    public float rightMaskAmount = 0;

    public float leftMaskSlope = 0;
    public float leftMaskAmount = 0;

    public float bottomMaskAmount = 0;

    public Shader correctionShader = null;
    public Material correctionMaterial = null;

    private Camera cam;

    public bool flip;

    void Update()
    {
        /*
        if (!correctionMaterial)
        {
            correctionMaterial = CreateMaterial();
        }
        if(cam == null)
        {
            cam = GetComponent<Camera>();
        }
        */
    }

    // Use this for initialization
    void Start()
    {
        correctionMaterial = CreateMaterial();
        cam = GetComponent<Camera>();
    }

    public void SetMaskAmount(float maskAmount)
    {
        correctionMaterial.SetFloat("maskAmount", maskAmount);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (correctionMaterial)
        {
            correctionMaterial.SetVector("corner1", corner1);
            correctionMaterial.SetVector("corner2", corner2);
            correctionMaterial.SetVector("corner3", corner3);
            correctionMaterial.SetVector("corner4", corner4);

            correctionMaterial.SetColor("gradientColor", gradientColor);

            correctionMaterial.SetFloat("leftMaskSlope", leftMaskSlope);
            correctionMaterial.SetFloat("leftMaskAmount", leftMaskAmount);

            correctionMaterial.SetFloat("rightMaskSlope", rightMaskSlope);
            correctionMaterial.SetFloat("rightMaskAmount", rightMaskAmount);

            correctionMaterial.SetFloat("bottomMaskAmount", bottomMaskAmount);

            Graphics.Blit(source, destination, correctionMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }


    Material CreateMaterial()
    {
        if (correctionShader == null)
        {
            correctionShader = Shader.Find("Hidden/GeometryCorrection");
        }
        return new Material(correctionShader);
    }
}
