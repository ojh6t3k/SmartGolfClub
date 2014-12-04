using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(ToneModule))]
public class ToneModuleInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ToneModule tone = (ToneModule)target;

		tone.id = EditorGUILayout.IntField("ID", tone.id);

		int oldFrequency = tone.currentFrequency;
		int newFrequency = (int)EditorGUILayout.Slider("Frequency", oldFrequency, 0, 5000);
		int oldDuration = tone.currentDuration;
		int newDuration = EditorGUILayout.IntField("Duration", oldDuration);
		if(oldFrequency != newFrequency || oldDuration != newDuration)
			tone.Play(newFrequency, newDuration);

		EditorGUILayout.IntField("Remain Time", tone.remainTime);
		if(Application.isPlaying == true)
			EditorUtility.SetDirty(target);
	}
}
