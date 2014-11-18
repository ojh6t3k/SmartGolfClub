using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(AnalogPin))]
public class AnalogPinInspector : Editor
{
	public override void OnInspectorGUI()
	{
		AnalogPin analogPin = (AnalogPin)target;

		analogPin.id = EditorGUILayout.IntField("ID", analogPin.id);
		analogPin.maxValue = EditorGUILayout.IntField("Max Value", analogPin.maxValue);
		EditorGUILayout.LabelField("RawValue", analogPin.RawValue.ToString());
		EditorGUILayout.LabelField("Value", analogPin.Value.ToString("F2"));

		if(Application.isPlaying == true)
			EditorUtility.SetDirty(target);
	}
}
