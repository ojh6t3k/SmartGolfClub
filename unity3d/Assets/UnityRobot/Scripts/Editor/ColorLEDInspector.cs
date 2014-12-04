using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(ColorLED))]
public class ColorLEDInspector : Editor
{
	SerializedProperty pwmRed;
	SerializedProperty pwmGreen;
	SerializedProperty pwmBlue;

	void OnEnable()
	{
		pwmRed = serializedObject.FindProperty("pwmRed");
		pwmGreen = serializedObject.FindProperty("pwmGreen");
		pwmBlue = serializedObject.FindProperty("pwmBlue");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		EditorGUILayout.PropertyField(pwmRed, new GUIContent("PWM Red"));
		EditorGUILayout.PropertyField(pwmGreen, new GUIContent("PWM Green"));
		EditorGUILayout.PropertyField(pwmBlue, new GUIContent("PWM Blue"));

		this.serializedObject.ApplyModifiedProperties();

		ColorLED colorLED = (ColorLED)target;
		colorLED.color = EditorGUILayout.ColorField("Color", colorLED.color);
		colorLED.calibrationRed = EditorGUILayout.Slider("Calibration Red", colorLED.calibrationRed, -1f, 1f);
		colorLED.calibrationGreen = EditorGUILayout.Slider("Calibration Green",colorLED.calibrationGreen, -1f, 1f);
		colorLED.calibrationBlue = EditorGUILayout.Slider("Calibration Blue",colorLED.calibrationBlue, -1f, 1f);
	}
}
