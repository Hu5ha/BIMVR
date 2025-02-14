using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;

[InitializeOnLoad]
public class EditorSettings : MonoBehaviour
{
    static EditorSettings()
    {
        PlayerSettings.virtualRealitySupported = true;
        

        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone, new string[] { "stereo" });
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        }
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
}
