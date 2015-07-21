using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(ClubGeometry))]
public class ClubGeometrynspector : Editor
{
	SerializedProperty characterCenter;
	SerializedProperty characterForward;
	SerializedProperty characterUp;
	SerializedProperty clubCenter;
	SerializedProperty clubForward;
	SerializedProperty clubUp;
	SerializedProperty clubFace;
	SerializedProperty displayDebug;
	
	void OnEnable()
	{
		characterCenter = serializedObject.FindProperty("characterCenter");
		characterForward = serializedObject.FindProperty("characterForward");
		characterUp = serializedObject.FindProperty("characterUp");
		clubCenter = serializedObject.FindProperty("clubCenter");
		clubForward = serializedObject.FindProperty("clubForward");
		clubUp = serializedObject.FindProperty("clubUp");
		clubFace = serializedObject.FindProperty("clubFace");
		displayDebug = serializedObject.FindProperty("displayDebug");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		ClubGeometry clubGeometry = (ClubGeometry)target;
		
		EditorGUILayout.PropertyField(characterCenter, new GUIContent("characterCenter"));
		EditorGUILayout.PropertyField(characterForward, new GUIContent("characterForward"));
		EditorGUILayout.PropertyField(characterUp, new GUIContent("characterUp"));
		EditorGUILayout.PropertyField(clubCenter, new GUIContent("clubCenter"));
		EditorGUILayout.PropertyField(clubForward, new GUIContent("clubForward"));
		EditorGUILayout.PropertyField(clubUp, new GUIContent("clubUp"));
		EditorGUILayout.PropertyField(clubFace, new GUIContent("clubFace"));
		EditorGUILayout.PropertyField(displayDebug, new GUIContent("displayDebug"));

		if(Application.isPlaying == true)
		{
			EditorGUILayout.FloatField("Roll Angle", clubGeometry.rollAngle);
			EditorGUILayout.FloatField("Yaw Angle", clubGeometry.yawAngle);
			EditorGUILayout.FloatField("Club Angle", clubGeometry.clubAngle);

			if(GUILayout.Button("Initialize") == true)
				clubGeometry.Initialize();

			EditorUtility.SetDirty(target);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
