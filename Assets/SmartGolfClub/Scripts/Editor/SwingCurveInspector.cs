using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(SwingCurve))]
public class SwingCurveInspector : Editor
{
	SerializedProperty dataName;
	SerializedProperty clubGeometry;
	SerializedProperty swingData;
	SerializedProperty maxRecordingTime;
	SerializedProperty upSwingLine;
	SerializedProperty downSwingLine;
	SerializedProperty OnRecordingStarted;
	SerializedProperty OnRecordingStopped;

	void OnEnable()
	{
		dataName = serializedObject.FindProperty("dataName");
		clubGeometry = serializedObject.FindProperty("clubGeometry");
		swingData = serializedObject.FindProperty("swingData");
		maxRecordingTime = serializedObject.FindProperty("maxRecordingTime");
		upSwingLine = serializedObject.FindProperty("upSwingLine");
		downSwingLine = serializedObject.FindProperty("downSwingLine");
		OnRecordingStarted = serializedObject.FindProperty("OnRecordingStarted");
		OnRecordingStopped = serializedObject.FindProperty("OnRecordingStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingCurve swingCurve = (SwingCurve)target;

		EditorGUILayout.PropertyField(dataName, new GUIContent("dataName"));
		EditorGUILayout.PropertyField(clubGeometry, new GUIContent("clubGeometry"));
		EditorGUILayout.PropertyField(swingData, new GUIContent("swingData"));
		EditorGUILayout.PropertyField(maxRecordingTime, new GUIContent("maxRecordingTime"));
		EditorGUILayout.PropertyField(upSwingLine, new GUIContent("upSwingLine"));
		EditorGUILayout.PropertyField(downSwingLine, new GUIContent("downSwingLine"));

		if(swingCurve.swingData != null)
		{
			if(GUILayout.Button("Load Curve") == true)
				swingCurve.LoadCurve();
		}

		if(swingCurve.dataEnabled == true)
		{
			EditorGUILayout.CurveField("Roll Angle", swingCurve.rollAngles);
			EditorGUILayout.CurveField("Yaw Angle", swingCurve.yawAngles);
			EditorGUILayout.CurveField("Club Angle", swingCurve.clubAngles);
			EditorGUILayout.CurveField("Club Velocity", swingCurve.clubVelocities);
			EditorGUILayout.CurveField("Up Swing Map", swingCurve.upswingMap);
			EditorGUILayout.CurveField("Down Swing Map", swingCurve.downswingMap);
			EditorGUILayout.FloatField("Total Time", swingCurve.totalTime);
			EditorGUILayout.FloatField("Top Time", swingCurve.topTime);
			EditorGUILayout.FloatField("Finish Time", swingCurve.finishTime);

			swingCurve.displayLine = EditorGUILayout.Toggle("Display Line", swingCurve.displayLine);

			if(GUILayout.Button("Save Curve") == true)
				swingCurve.SaveCurve(EditorUtility.SaveFilePanel("Save Curve", "Assets", swingCurve.dataName, "xml"));
			if(GUILayout.Button("Clear Curve") == true)
				swingCurve.ClearCurve();
		}

		if(Application.isPlaying == true)
		{
			if(swingCurve.isRecording == false)
			{
				if(GUILayout.Button("Recording Start") == true)
					swingCurve.RecordingStart();
			}
			else
			{
				if(GUILayout.Button("Recording Stop") == true)
					swingCurve.RecordingStop();
			}

			EditorUtility.SetDirty(target);
		}

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(OnRecordingStarted, new GUIContent("OnRecordingStarted"));
		EditorGUILayout.PropertyField(OnRecordingStopped, new GUIContent("OnRecordingStopped"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
