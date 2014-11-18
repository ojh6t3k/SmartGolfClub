using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(ColorLED))]
public class ColorLEDInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ColorLED colorLED = (ColorLED)target;

		colorLED.pwmRed = (DigitalPin)EditorGUILayout.ObjectField("PWM Red", colorLED.pwmRed, typeof(DigitalPin), true);
		colorLED.pwmGreen = (DigitalPin)EditorGUILayout.ObjectField("PWM Green", colorLED.pwmGreen, typeof(DigitalPin), true);
		colorLED.pwmBlue = (DigitalPin)EditorGUILayout.ObjectField("PWM Blue", colorLED.pwmBlue, typeof(DigitalPin), true);
		colorLED.color = EditorGUILayout.ColorField("Color", colorLED.color);
		colorLED.calibrationRed = EditorGUILayout.Slider("Calibration Red", colorLED.calibrationRed, -1f, 1f);
		colorLED.calibrationGreen = EditorGUILayout.Slider("Calibration Green",colorLED.calibrationGreen, -1f, 1f);
		colorLED.calibrationBlue = EditorGUILayout.Slider("Calibration Blue",colorLED.calibrationBlue, -1f, 1f);
	}
}
