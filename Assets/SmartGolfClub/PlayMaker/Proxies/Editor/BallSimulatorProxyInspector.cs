using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartGolf;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(BallSimulatorProxy))]
public class BallSimulatorProxyInspector : Editor
{
	SerializedProperty ballSimulator;
	
	void OnEnable()
	{
		ballSimulator = serializedObject.FindProperty("ballSimulator");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		BallSimulatorProxy proxy = (BallSimulatorProxy)target;
		
		EditorGUILayout.PropertyField(ballSimulator, new GUIContent("ballSimulator"));
		
		if(proxy.ballSimulator == null)
		{
			EditorGUILayout.HelpBox("There is no Ball Simulator!", MessageType.Error);
		}
		else
		{
			proxy.eventOnSimulationStarted = ProxyInspectorUtil.EventField(target, "OnSimulationStarted", proxy.eventOnSimulationStarted, proxy.builtInOnSimulationStarted);
			proxy.eventOnSimulationStopped = ProxyInspectorUtil.EventField(target, "OnSimulationStopped", proxy.eventOnSimulationStopped, proxy.builtInOnSimulationStopped);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
