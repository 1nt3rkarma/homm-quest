using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Scenario),true)]
public class ScenarioEditor : Editor
{

    public override void OnInspectorGUI()
    {
        var scenario = (Scenario)target;
        base.OnInspectorGUI();

        if (scenario.triggerMode == Scenario.TriggerModes.condition
            || scenario.repeatMode == Scenario.RepeatModes.limited)
        {
            EditorGUILayout.LabelField("Add. settings", EditorStyles.boldLabel);

            if (scenario.repeatMode == Scenario.RepeatModes.limited)
            {
                scenario.repeats = EditorGUILayout.IntField("Repeats", scenario.repeats);
            }
        }

    }
}
