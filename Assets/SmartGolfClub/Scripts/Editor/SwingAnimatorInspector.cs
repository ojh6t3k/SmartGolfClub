using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(SwingAnimator))]
public class SwingAnimatorInspector : Editor
{
	SerializedProperty animator;
	SerializedProperty swingState;
	SerializedProperty layer;
	SerializedProperty speed;
	SerializedProperty swingClip;
	SerializedProperty swingCurve;

	void OnEnable()
	{
		animator = serializedObject.FindProperty("animator");
		swingState = serializedObject.FindProperty("swingState");
		layer = serializedObject.FindProperty("layer");
		speed = serializedObject.FindProperty("speed");
		swingClip = serializedObject.FindProperty("swingClip");
		swingCurve = serializedObject.FindProperty("swingCurve");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingAnimator swingAnimator = (SwingAnimator)target;
		
		EditorGUILayout.PropertyField(animator, new GUIContent("animator"));
		EditorGUILayout.PropertyField(swingState, new GUIContent("swingState"));
		EditorGUILayout.PropertyField(layer, new GUIContent("layer"));
		EditorGUILayout.PropertyField(speed, new GUIContent("speed"));
		EditorGUILayout.PropertyField(swingClip, new GUIContent("swingClip"));
		EditorGUILayout.PropertyField(swingCurve, new GUIContent("swingCurve"));

		if(Application.isPlaying == true)
		{
			swingAnimator.normalizedTime = EditorGUILayout.Slider(swingAnimator.normalizedTime, 0f, 1f);

			GUI.enabled = !swingAnimator.isCreatingCurve;

			if(GUILayout.Button("Swing") == true)
				swingAnimator.Swing();
			if(GUILayout.Button("Create Curve") == true)
				swingAnimator.CreateCurve();
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
