using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartGolf;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(SwingMapperProxy))]
public class SwingMapperProxyInspector : Editor
{
	SerializedProperty swingMapper;
	
	void OnEnable()
	{
		swingMapper = serializedObject.FindProperty("swingMapper");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		SwingMapperProxy proxy = (SwingMapperProxy)target;
		
		EditorGUILayout.PropertyField(swingMapper, new GUIContent("swingMapper"));
		
		if(proxy.swingMapper == null)
		{
			EditorGUILayout.HelpBox("There is no SwingMapper!", MessageType.Error);
		}
		else
		{
			proxy.eventOnReplayStarted = ProxyInspectorUtil.EventField(target, "OnReplayStarted", proxy.eventOnReplayStarted, proxy.builtInOnReplayStarted);
			proxy.eventOnReplayStopped = ProxyInspectorUtil.EventField(target, "OnReplayStopped", proxy.eventOnReplayStopped, proxy.builtInOnReplayStopped);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
