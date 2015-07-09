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
	SerializedProperty swingLength;
	SerializedProperty swingCurve;

	void OnEnable()
	{
		animator = serializedObject.FindProperty("animator");
		swingState = serializedObject.FindProperty("swingState");
		layer = serializedObject.FindProperty("layer");
		swingLength = serializedObject.FindProperty("swingLength");
		swingCurve = serializedObject.FindProperty("swingCurve");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingAnimator swingAnimator = (SwingAnimator)target;
		
		EditorGUILayout.PropertyField(animator, new GUIContent("animator"));
		EditorGUILayout.PropertyField(swingState, new GUIContent("swingState"));
		EditorGUILayout.PropertyField(layer, new GUIContent("layer"));
		EditorGUILayout.PropertyField(swingLength, new GUIContent("swingLength"));
		EditorGUILayout.PropertyField(swingCurve, new GUIContent("swingCurve"));

		if(Application.isPlaying == true)
		{
			if(GUILayout.Button("Swing") == true)
				swingAnimator.Swing();

			swingAnimator.normalizedTime = EditorGUILayout.Slider(swingAnimator.normalizedTime, 0f, 1f);

			if(GUILayout.Button("Create Curve") == true)
				swingAnimator.CreateCurve();
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
