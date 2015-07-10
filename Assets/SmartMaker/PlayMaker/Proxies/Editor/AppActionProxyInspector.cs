using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(AppActionProxy))]
public class AppActionProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		AppActionProxy proxy = (AppActionProxy)target;
		
		if(proxy.GetComponent<AppAction>() == null)
		{
			EditorGUILayout.HelpBox("There is no AppAction!", MessageType.Error);
		}
		else
		{
			proxy.eventOnStarted = ProxyInspectorUtil.EventField(target, "OnStarted", proxy.eventOnStarted, proxy.builtInOnStarted);
			proxy.eventOnExcuted = ProxyInspectorUtil.EventField(target, "OnExcuted", proxy.eventOnExcuted, proxy.builtInOnExcuted);
			proxy.eventOnStopped = ProxyInspectorUtil.EventField(target, "OnStopped", proxy.eventOnStopped, proxy.builtInOnStopped);
		}
	}
}
