using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UILocalize)), CanEditMultipleObjects]
public class UILocalizeEditor : Editor
{
    public SerializedProperty KeyProperty;
    public SerializedProperty KeyUnitProperty;
    public SerializedProperty TypeProperty;

    private void OnEnable()
    {
        LocalizationData.LoadLocalizationLocal();
        LocalizationData.LoadLocalizationDLC();
        KeyProperty = serializedObject.FindProperty("key");
        KeyUnitProperty = serializedObject.FindProperty("keyUnit");
        TypeProperty = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 80f;

        string myKey = KeyProperty.stringValue;
        string myKeyUnit = KeyUnitProperty.stringValue;
        bool isPresent = false;

        if (!string.IsNullOrEmpty(myKeyUnit))
        {
            if (Localization.unitLocalize.ContainsKey(myKeyUnit))
            {
                if (Localization.unitLocalize[myKeyUnit].dictionary.ContainsKey(myKey))
                {
                    isPresent = true;
                }
            }
        }
        else
        {
            if (Localization.dictionary.ContainsKey(myKey))
            {
                isPresent = true;
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(KeyProperty);
        GUI.color = isPresent ? Color.green : Color.red;
        GUILayout.BeginVertical(GUILayout.Width(22f));
        GUILayout.Space(2f);
        GUILayout.Label(isPresent ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
        GUILayout.EndVertical();
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(KeyUnitProperty);
        GUI.color = isPresent ? Color.green : Color.red;
        GUILayout.BeginVertical(GUILayout.Width(22f));
        GUILayout.Space(2f);
        GUILayout.Label(isPresent ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
        GUILayout.EndVertical();
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(TypeProperty);

        if (isPresent)
        {
            EditorGUILayout.LabelField("Preview");
            string[] keys;
            string[] values;

            if (!string.IsNullOrEmpty(myKeyUnit))
            {
                Localization.unitLocalize[myKeyUnit].dictionary.TryGetValue(myKey, out values);
                keys = Localization.unitLocalize[myKeyUnit].knownLanguages;
            }
            else
            {
                Localization.dictionary.TryGetValue(myKey, out values);
                keys = Localization.knownLanguages;
            }

            if (values != null)
            {
                if (keys.Length != values.Length)
                {
                    EditorGUILayout.HelpBox(
                        "Number of keys doesn't match the number of values! Did you modify the dictionaries by hand at some point?",
                        MessageType.Error);
                }
                else
                {
                    for (int i = 0; i < keys.Length; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(keys[i], GUILayout.Width(66f));

                        if (GUILayout.Button(values[i], "AS TextArea", GUILayout.MinWidth(80f),
                            GUILayout.MaxWidth(Screen.width - 110f), GUILayout.MaxHeight(30)))
                        {
                            (target as UILocalize).value = values[i];
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                GUILayout.Label("No preview available");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}