using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(AnalogPin))]
public class AnalogPinInspector : Editor
{
	SerializedProperty id;
	SerializedProperty maxValue;
	
	void OnEnable()
	{
		id = serializedObject.FindProperty("id");
		maxValue = serializedObject.FindProperty("maxValue");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		AnalogPin analogPin = (AnalogPin)target;

		EditorGUILayout.PropertyField(id, new GUIContent("ID"));
		EditorGUILayout.PropertyField(maxValue, new GUIContent("Max Value"));
		EditorGUILayout.LabelField("RawValue", analogPin.RawValue.ToString());
		EditorGUILayout.LabelField("Value", analogPin.Value.ToString("F2"));

		if(Application.isPlaying == true)
			EditorUtility.SetDirty(target);
		this.serializedObject.ApplyModifiedProperties();
	}
}
