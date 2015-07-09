using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(SwingCurve))]
public class SwingCurveInspector : Editor
{
	SerializedProperty clubGeometry;
	SerializedProperty swingData;
	SerializedProperty maxRecordingTime;

	void OnEnable()
	{
		clubGeometry = serializedObject.FindProperty("clubGeometry");
		swingData = serializedObject.FindProperty("swingData");
		maxRecordingTime = serializedObject.FindProperty("maxRecordingTime");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingCurve swingCurve = (SwingCurve)target;
		
		EditorGUILayout.PropertyField(clubGeometry, new GUIContent("clubGeometry"));
		EditorGUILayout.PropertyField(swingData, new GUIContent("swingData"));
		EditorGUILayout.PropertyField(maxRecordingTime, new GUIContent("maxRecordingTime"));

		if(swingCurve.dataEnabled == true)
		{
			EditorGUILayout.CurveField("Roll Angle", swingCurve.rollAngles);
			EditorGUILayout.CurveField("Yaw Angle", swingCurve.yawAngles);
			EditorGUILayout.CurveField("Club Angle", swingCurve.clubAngles);
			EditorGUILayout.CurveField("Club Velocity", swingCurve.clubVelocities);
			EditorGUILayout.FloatField("Total Time", swingCurve.totalTime);
			EditorGUILayout.FloatField("Top Time", swingCurve.topTime);
			EditorGUILayout.FloatField("Finish Time", swingCurve.finishTime);
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
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
