using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartGolf;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(SwingCurveProxy))]
public class SwingCurveProxyInspector : Editor
{
	SerializedProperty swingCurve;
	
	void OnEnable()
	{
		swingCurve = serializedObject.FindProperty("swingCurve");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingCurveProxy proxy = (SwingCurveProxy)target;
		
		EditorGUILayout.PropertyField(swingCurve, new GUIContent("swingCurve"));
		
		if(proxy.swingCurve == null)
		{
			EditorGUILayout.HelpBox("There is no SwingCurve!", MessageType.Error);
		}
		else
		{
			proxy.eventOnRecordingStarted = ProxyInspectorUtil.EventField(target, "OnRecordingStarted", proxy.eventOnRecordingStarted, proxy.builtInOnRecordingStarted);
			proxy.eventOnRecordingStopped = ProxyInspectorUtil.EventField(target, "OnRecordingStopped", proxy.eventOnRecordingStopped, proxy.builtInOnRecordingStopped);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
