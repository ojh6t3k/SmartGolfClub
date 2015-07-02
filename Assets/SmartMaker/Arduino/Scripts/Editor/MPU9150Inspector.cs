using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using SmartMaker;

[CustomEditor(typeof(MPU9150))]
public class MPU9150Inspector : Editor
{
	bool foldout = true;

	SerializedProperty owner;
	SerializedProperty id;
	SerializedProperty targetObject;
	SerializedProperty smoothFollow;
	SerializedProperty offsetAngles;
	SerializedProperty OnCalibrated;

	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		id = serializedObject.FindProperty("id");
		targetObject = serializedObject.FindProperty("target");
		smoothFollow = serializedObject.FindProperty("smoothFollow");
		offsetAngles = serializedObject.FindProperty("offsetAngles");
		OnCalibrated = serializedObject.FindProperty("OnCalibrated");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		MPU9150 imu = (MPU9150)target;

		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));

		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(targetObject, new GUIContent("Target"));
		EditorGUILayout.PropertyField(smoothFollow, new GUIContent("Smooth Follow"));
		EditorGUILayout.PropertyField(offsetAngles, new GUIContent("Offset"));

		if(Application.isPlaying == true)
		{
			GUI.enabled = false;
			EditorGUILayout.Vector3Field("Angle", imu.Rotation.eulerAngles);
			GUI.enabled = true;
			if(GUILayout.Button("Calibration") == true)
				imu.Calibration();
		}

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnCalibrated);

		this.serializedObject.ApplyModifiedProperties();
	}
}
