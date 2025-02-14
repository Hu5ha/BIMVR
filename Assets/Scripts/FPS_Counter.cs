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

public class FPS_Counter : MonoBehaviour {

    public KeyCode ToggleFPSCounter = KeyCode.I;

    public float updateInterval = 0.5f;
 
    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    public Color guiColor = Color.white;

    private bool showFPS;

    private string gText;

	// Use this for initialization
	void Start () 
    {
        timeleft = updateInterval;
        //Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            gText = "" + (accum / frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
        if (Input.GetKeyDown(ToggleFPSCounter))
        {
            showFPS = !showFPS;
        }
	}
    void OnGUI()
    {
        if (showFPS)
        {
            GUI.color = guiColor;
            GUI.Label(new Rect(20, 20, 100, 30), "FPS: " + gText);
        }
    }
}
