using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EncounterWave))]
public class EncounterWaveEditor : Editor
{
    bool showUnitList = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        var wave = (EncounterWave)target;

        switch (wave.mode)
        {
            case EncounterWave.WaveModes.generic:

                showUnitList = EditorGUILayout.Foldout(showUnitList, "Generation pull types");
                if (wave.genericTypes == null)
                {
                    wave.genericTypes = new List<Unit>(1);
                    wave.genericTypesChances = new List<int>(1);
                }
                if (wave.genericTypes.Count == 0)
                {
                    wave.genericTypes.Add(null);
                    wave.genericTypesChances.Add(1);
                }

                if (showUnitList)
                {
                    EditorGUI.indentLevel += 1;
                    for (int i = 0; i < wave.genericTypes.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        wave.genericTypes[i] = (Unit)EditorGUILayout.ObjectField(
                            wave.genericTypes[i], typeof(Unit), true);

                        int chance = EditorGUILayout.IntField(
                            wave.genericTypesChances[i], GUILayout.Width(48));
                        wave.genericTypesChances[i] = Mathf.Clamp(chance, 1, 100);
                  
                        EditorGUILayout.LabelField("%", GUILayout.Width(32));

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        wave.genericTypes.Add(null);
                        wave.genericTypesChances.Add(50);
                    }
                    if (GUILayout.Button("-") && wave.genericTypes.Count > 1)
                    {
                        wave.genericTypes.RemoveAt(wave.genericTypes.Count - 1);
                        wave.genericTypesChances.RemoveAt(wave.genericTypesChances.Count - 1);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel -= 1;
                }

                break;
            case EncounterWave.WaveModes.defined:

                showUnitList = EditorGUILayout.Foldout(showUnitList, "Wave units");
                if (wave.definedUnits == null)
                    wave.definedUnits = new List<Unit>(1);
                if (wave.definedUnits.Count == 0)
                    wave.definedUnits.Add(null);

                if (showUnitList)
                {
                    EditorGUI.indentLevel += 1;
                    for (int i = 0; i < wave.definedUnits.Count; i++)
                        wave.definedUnits[i] = (Unit)EditorGUILayout.ObjectField(
                            wave.definedUnits[i], typeof(Unit), true);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                        wave.definedUnits.Add(null);
                    if (GUILayout.Button("-") && wave.definedUnits.Count > 1)
                        wave.definedUnits.RemoveAt(wave.definedUnits.Count-1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel -= 1;
                }

                break;
        }
        serializedObject.ApplyModifiedProperties();
        //EditorUtility.SetDirty(target);
    }
}
