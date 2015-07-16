using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;
using SmartGolf;

[CustomEditor(typeof(SwingMapper))]
public class SwingMapperInspector : Editor
{
	SerializedProperty referenceAnimator;
	SerializedProperty controlAnimator;
	SerializedProperty userClub;
	SerializedProperty userCurve;
	SerializedProperty imu;
	SerializedProperty replaySpeed;
	SerializedProperty OnReplayStarted;
	SerializedProperty OnReplayStopped;
	
	void OnEnable()
	{
		referenceAnimator = serializedObject.FindProperty("referenceAnimator");
		controlAnimator = serializedObject.FindProperty("controlAnimator");
		userClub = serializedObject.FindProperty("userClub");
		userCurve = serializedObject.FindProperty("userCurve");
		imu = serializedObject.FindProperty("imu");
		replaySpeed = serializedObject.FindProperty("replaySpeed");
		OnReplayStarted = serializedObject.FindProperty("OnReplayStarted");
		OnReplayStopped = serializedObject.FindProperty("OnReplayStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingMapper swingMapper = (SwingMapper)target;
		
		EditorGUILayout.PropertyField(referenceAnimator, new GUIContent("referenceAnimator"));
		EditorGUILayout.PropertyField(controlAnimator, new GUIContent("controlAnimator"));
		EditorGUILayout.PropertyField(userClub, new GUIContent("userClub"));
		EditorGUILayout.PropertyField(userCurve, new GUIContent("userCurve"));
		EditorGUILayout.PropertyField(imu, new GUIContent("imu"));
		EditorGUILayout.PropertyField(replaySpeed, new GUIContent("replaySpeed"));
		
		if(Application.isPlaying == true)
		{
			bool realtimeMapping = EditorGUILayout.Toggle("Realtime Mapping", swingMapper.realtimeMapping);
			if(realtimeMapping != swingMapper.realtimeMapping)
				swingMapper.realtimeMapping = realtimeMapping;
			
			if(swingMapper.isReplaying == false)
			{
				if(GUILayout.Button("Replay Start") == true)
					swingMapper.ReplayStart();
			}
			else
			{
				if(GUILayout.Button("Replay Stop") == true)
					swingMapper.ReplayStop();
			}

			EditorUtility.SetDirty(target);
		}

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(OnReplayStarted, new GUIContent("OnReplayStarted"));
		EditorGUILayout.PropertyField(OnReplayStopped, new GUIContent("OnReplayStopped"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
