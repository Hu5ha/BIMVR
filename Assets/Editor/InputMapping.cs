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

using UnityEditor;

[InitializeOnLoad]
public class InputMapping
{
    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

    public class InputAxis
    {
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;

        public float gravity;
        public float dead;
        public float sensitivity;

        public bool snap = false;
        public bool invert = false;

        public AxisType type;

        public int axis;
        public int joyNum;
    }

    static InputMapping()
    {
        AddAxis(new InputAxis()
        {
            name = "XboxOne_A",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 0",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_B",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 1",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_X",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 2",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_Y",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 4",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_LS_h",
            dead = 0.19f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 1,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_LS_v",
            dead = 0.19f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 2,
            invert = true,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_RS_h",
            dead = 0.19f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 4,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_RS_v",
            dead = 0.19f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 5,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_LT",
            dead = 0.01f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 9,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_RT",
            dead = 0.01f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 10,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_LS_B",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 8",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_RS_B",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 9",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_LB",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 4",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_RB",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 5",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_DPAD_h",
            dead = 0.01f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 6,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_DPAD_v",
            dead = 0.01f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 7,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_Start",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 7",
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "XboxOne_Back",
            gravity = 1000,
            dead = 0.001f,
            sensitivity = 1000f,
            type = AxisType.KeyOrMouseButton,
            axis = 3,
            positiveButton = "joystick button 6",
            joyNum = 0,
        });
    }

    private static bool AxisDefined(string axisName)
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.Next(true);
        axesProperty.Next(true);
        while (axesProperty.Next(false))
        {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);
            if (axis.stringValue == axisName) return true;
        }
        return false;
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        }
        while (child.Next(false));
        return null;
    }

    private static void AddAxis(InputAxis axis)
    {
        if (AxisDefined(axis.name)) return;

        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.arraySize++;
        serializedObject.ApplyModifiedProperties();

        SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
        GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

        serializedObject.ApplyModifiedProperties();
    }
}


