using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloatingTextSettings))]
public class FloatingTextSettingsEditor : Editor
{
    FloatingTextSettings script;

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        script = (FloatingTextSettings)target;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.BeginVertical("box");
        script.floatingDirectionType = (FloatingTextSettings.FloatingType)EditorGUILayout.EnumPopup("Floating Type", script.floatingDirectionType);
        if(script.floatingDirectionType == FloatingTextSettings.FloatingType.FixedDirection)
        {
            script.FloatDirection = EditorGUILayout.Vector2Field("Floating Direction", script.FloatDirection);
        }else if(script.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
        {
            script.XFloatCurve = EditorGUILayout.CurveField("Floating X Axi", script.XFloatCurve);
            script.YFloatCurve = EditorGUILayout.CurveField("Floating Y Axi", script.YFloatCurve);
        }
        else if (script.floatingDirectionType == FloatingTextSettings.FloatingType.RandomizeAxis)
        {
            script.XFloatRange = EditorGUILayout.Vector2Field("Floating X Range", script.XFloatRange);
            script.YFloatRange = EditorGUILayout.Vector2Field("Floating Y Range", script.YFloatRange);
        }
        EditorGUILayout.EndVertical();

        script.StartSequenceDuration = EditorGUILayout.FloatField("Start Sequence Duration", script.StartSequenceDuration);
        script.StaticDuration = EditorGUILayout.FloatField("Static Duration", script.StaticDuration);
        script.FloatingDuration = EditorGUILayout.FloatField("Floating Duration", script.FloatingDuration);
        script.FinishSequenceDuration = EditorGUILayout.FloatField("Finish Sequence Duration", script.FinishSequenceDuration);

        script.StartScaleCurve = EditorGUILayout.CurveField("Start Scale Curve", script.StartScaleCurve);
        script.FinishScaleCurve = EditorGUILayout.CurveField("Finish Scale Curve", script.FinishScaleCurve);
        script.FadeInCurve = EditorGUILayout.CurveField("Fade In Curve", script.FadeInCurve);
        script.FadeOverLifeTime = EditorGUILayout.CurveField("Fade Over Lifetime", script.FadeOverLifeTime);
        script.FadeOutCurve = EditorGUILayout.CurveField("Fade Out Curve", script.FadeOutCurve);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}