using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(BallSimulator))]
public class BallSimulatorInspector : Editor
{
	SerializedProperty ball;
	SerializedProperty lengthUnit;
	SerializedProperty forceScale;
	SerializedProperty targetDir;
	SerializedProperty impactZone;
	SerializedProperty desiredLine;
	SerializedProperty ballLine;
	SerializedProperty diplayDebug;
	SerializedProperty uiMiniMap;
	SerializedProperty uiDesiredLine;
	SerializedProperty uiBallLine;
	SerializedProperty uiBall;
	SerializedProperty uiStartPos;
	SerializedProperty uiDistance;
	SerializedProperty uiAngle;
	SerializedProperty maxDistance;
	SerializedProperty OnSimulationStarted;
	SerializedProperty OnSimulationStopped;

	void OnEnable()
	{
		ball = serializedObject.FindProperty("ball");
		lengthUnit = serializedObject.FindProperty("lengthUnit");
		forceScale = serializedObject.FindProperty("forceScale");
		targetDir = serializedObject.FindProperty("targetDir");
		impactZone = serializedObject.FindProperty("impactZone");
		desiredLine = serializedObject.FindProperty("desiredLine");
		ballLine = serializedObject.FindProperty("ballLine");
		diplayDebug = serializedObject.FindProperty("diplayDebug");
		uiMiniMap = serializedObject.FindProperty("uiMiniMap");
		uiDesiredLine = serializedObject.FindProperty("uiDesiredLine");
		uiBallLine = serializedObject.FindProperty("uiBallLine");
		uiBall = serializedObject.FindProperty("uiBall");
		uiStartPos = serializedObject.FindProperty("uiStartPos");
		uiDistance = serializedObject.FindProperty("uiDistance");
		uiAngle = serializedObject.FindProperty("uiAngle");
		maxDistance = serializedObject.FindProperty("maxDistance");
		OnSimulationStarted = serializedObject.FindProperty("OnSimulationStarted");
		OnSimulationStopped = serializedObject.FindProperty("OnSimulationStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		BallSimulator simulator = (BallSimulator)target;
		
		EditorGUILayout.PropertyField(ball, new GUIContent("ball"));
		EditorGUILayout.PropertyField(lengthUnit, new GUIContent("lengthUnit"));
		EditorGUILayout.PropertyField(forceScale, new GUIContent("forceScale"));
		EditorGUILayout.PropertyField(targetDir, new GUIContent("targetDir"));
		EditorGUILayout.PropertyField(impactZone, new GUIContent("impactZone"));
		EditorGUILayout.PropertyField(desiredLine, new GUIContent("desiredLine"));
		EditorGUILayout.PropertyField(ballLine, new GUIContent("ballLine"));
		EditorGUILayout.PropertyField(diplayDebug, new GUIContent("diplayDebug"));
		EditorGUILayout.PropertyField(uiMiniMap, new GUIContent("uiMiniMap"));
		EditorGUILayout.PropertyField(uiDesiredLine, new GUIContent("uiDesiredLine"));
		EditorGUILayout.PropertyField(uiBallLine, new GUIContent("uiBallLine"));
		EditorGUILayout.PropertyField(uiBall, new GUIContent("uiBall"));
		EditorGUILayout.PropertyField(uiStartPos, new GUIContent("uiStartPos"));
		EditorGUILayout.PropertyField(uiDistance, new GUIContent("uiDistance"));
		EditorGUILayout.PropertyField(uiAngle, new GUIContent("uiAngle"));
		EditorGUILayout.PropertyField(maxDistance, new GUIContent("maxDistance"));
		EditorGUILayout.PropertyField(OnSimulationStarted, new GUIContent("OnSimulationStarted"));
		EditorGUILayout.PropertyField(OnSimulationStopped, new GUIContent("OnSimulationStopped"));
		
		if(Application.isPlaying == true)
		{
			if(GUILayout.Button("Reset") == true)
				simulator.Reset();

			if(GUILayout.Button("Play") == true)
				simulator.Play();

			EditorGUILayout.FloatField("Distance", simulator.distance);
			EditorGUILayout.FloatField("Desired Distance", simulator.desiredDistance);
			EditorGUILayout.FloatField("Angle", simulator.angle);

			if(simulator.isPlaying == true)
				EditorUtility.SetDirty(target);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
