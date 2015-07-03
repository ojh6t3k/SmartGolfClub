using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(SwingAnalyzer))]
public class SwingAnalyzerInspector : Editor
{
	SerializedProperty characterRoot;
	SerializedProperty characterForward;
	SerializedProperty clubRoot;
	SerializedProperty clubForward;
	SerializedProperty clubUp;
	
	void OnEnable()
	{
		characterRoot = serializedObject.FindProperty("characterRoot");
		characterForward = serializedObject.FindProperty("characterForward");
		clubRoot = serializedObject.FindProperty("clubRoot");
		clubForward = serializedObject.FindProperty("clubForward");
		clubUp = serializedObject.FindProperty("clubUp");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		SwingAnalyzer swingAnalyzer = (SwingAnalyzer)target;
		
		EditorGUILayout.PropertyField(characterRoot, new GUIContent("characterRoot"));
		EditorGUILayout.PropertyField(characterForward, new GUIContent("characterForward"));
		EditorGUILayout.PropertyField(clubRoot, new GUIContent("clubRoot"));
		EditorGUILayout.PropertyField(clubForward, new GUIContent("clubForward"));
		EditorGUILayout.PropertyField(clubUp, new GUIContent("clubUp"));

		EditorGUILayout.FloatField("Roll Angle", swingAnalyzer.rollAngle);
		EditorGUILayout.FloatField("Pitch Angle", swingAnalyzer.pitchAngle);

		if(GUILayout.Button("Initialize") == true)
			swingAnalyzer.Initialize();
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
