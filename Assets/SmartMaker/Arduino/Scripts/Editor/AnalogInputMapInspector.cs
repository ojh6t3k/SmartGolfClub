using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(AnalogInputMap))]
public class AnalogInputMapInspector : Editor
{
	SerializedProperty analogInput;
	SerializedProperty multiplier;
	SerializedProperty mapCurve;

	void OnEnable()
	{
		analogInput = serializedObject.FindProperty("analogInput");
		multiplier = serializedObject.FindProperty("multiplier");
		mapCurve = serializedObject.FindProperty("mapCurve");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		EditorGUILayout.PropertyField(analogInput, new GUIContent("analogInput"));
		EditorGUILayout.PropertyField(multiplier, new GUIContent("multiplier"));
		EditorGUILayout.LabelField("mapCurve");
		mapCurve.animationCurveValue = EditorGUILayout.CurveField(mapCurve.animationCurveValue, GUILayout.Height(100));

		this.serializedObject.ApplyModifiedProperties();
	}
}
