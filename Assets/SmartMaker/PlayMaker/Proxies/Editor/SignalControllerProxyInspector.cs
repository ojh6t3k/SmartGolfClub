using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(SignalControllerProxy))]
public class SignalControllerProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		SignalControllerProxy proxy = (SignalControllerProxy)target;
		
		if(proxy.GetComponent<SignalController>() == null)
		{
			EditorGUILayout.HelpBox("There is no SignalController!", MessageType.Error);
		}
		else
		{
			proxy.eventOnStarted = ProxyInspectorUtil.EventField(target, "OnStarted", proxy.eventOnStarted, proxy.builtInOnStarted);
			proxy.eventOnStopped = ProxyInspectorUtil.EventField(target,"OnStopped", proxy.eventOnStopped, proxy.builtInOnStopped);
			proxy.eventOnCompleted = ProxyInspectorUtil.EventField(target,"OnCompleted", proxy.eventOnCompleted, proxy.builtInOnCompleted);
		}
	}
}
