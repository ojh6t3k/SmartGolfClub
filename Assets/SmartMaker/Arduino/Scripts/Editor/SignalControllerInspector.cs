using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(SignalController))]
public class SignalControllerInspector : Editor
{
	bool foldout = true;
	bool foldout2 = true;
	
	SerializedProperty appAction;
	SerializedProperty loop;
	SerializedProperty signals;
	SerializedProperty index;
	SerializedProperty multiplier;
	SerializedProperty speed;
	SerializedProperty OnStarted;
	SerializedProperty OnCompleted;
	SerializedProperty OnStopped;
	
	void OnEnable()
	{
		appAction = serializedObject.FindProperty("appAction");
		loop = serializedObject.FindProperty("loop");
		signals = serializedObject.FindProperty("signals");
		index = serializedObject.FindProperty("index");
		multiplier = serializedObject.FindProperty("multiplier");
		speed = serializedObject.FindProperty("speed");
		OnStarted = serializedObject.FindProperty("OnStarted");
		OnCompleted = serializedObject.FindProperty("OnCompleted");
		OnStopped = serializedObject.FindProperty("OnStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SignalController signalController = (SignalController)target;
		
		EditorGUILayout.PropertyField(appAction, new GUIContent("AppAction"));

		foldout = EditorGUILayout.Foldout(foldout, "Play Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(index, new GUIContent("index"));
			EditorGUILayout.PropertyField(multiplier, new GUIContent("multiplier"));
			EditorGUILayout.PropertyField(speed, new GUIContent("speed"));
			EditorGUILayout.PropertyField(loop, new GUIContent("loop"));
			EditorGUI.indentLevel--;
		}
		if(Application.isPlaying == true)
		{
			if(signalController.isPlaying == false)
			{
				if(GUILayout.Button("Play") == true)
					signalController.Play();
			}
			else
			{
				if(GUILayout.Button("Stop") == true)
					signalController.Stop();

				EditorUtility.SetDirty(target);
			}
		}

		foldout2 = EditorGUILayout.Foldout(foldout2, "Signals");
		if(foldout2 == true)
		{
			SerializedProperty signal;
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Add Signal") == true)
			{
				signals.InsertArrayElementAtIndex(signals.arraySize);
				signal = signals.GetArrayElementAtIndex(signals.arraySize - 1);
			}
			if(signals.arraySize > 0)
			{
				if(GUILayout.Button("Remove All") == true)
					signals.ClearArray();
			}
			EditorGUILayout.EndHorizontal();
			for(int i=0; i<signals.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				signal = signals.GetArrayElementAtIndex(i);
				GUILayout.Label(string.Format("Signal {0:d}", i), GUILayout.Width(60));
				GUILayout.FlexibleSpace();
				EditorGUILayout.PropertyField(signal, GUIContent.none);
				
				if(GUILayout.Button("+", GUILayout.Width(20)) == true)
				{
					signals.MoveArrayElement(i, i + 1);
				}

				if(i > 0)
					GUI.enabled = true;
				else
					GUI.enabled = false;

				if(GUILayout.Button("-", GUILayout.Width(20)) == true)
				{
					signals.MoveArrayElement(i, i - 1);
				}

				GUI.enabled = true;

				if(GUILayout.Button("x", GUILayout.Width(20)) == true)
				{
					signals.DeleteArrayElementAtIndex(i);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnStarted);
		EditorGUILayout.PropertyField(OnCompleted);
		EditorGUILayout.PropertyField(OnStopped);

		this.serializedObject.ApplyModifiedProperties();
	}
}
