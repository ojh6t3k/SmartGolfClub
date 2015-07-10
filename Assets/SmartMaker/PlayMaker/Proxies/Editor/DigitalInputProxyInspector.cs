using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(DigitalInputProxy))]
public class DigitalInputProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DigitalInputProxy proxy = (DigitalInputProxy)target;
		
		if(proxy.GetComponent<DigitalInput>() == null)
		{
			EditorGUILayout.HelpBox("There is no DigitalInput!", MessageType.Error);
		}
		else
		{
			proxy.eventOnStarted = ProxyInspectorUtil.EventField(target, "OnStarted", proxy.eventOnStarted, proxy.builtInOnStarted);
			proxy.eventOnStopped = ProxyInspectorUtil.EventField(target, "OnStopped", proxy.eventOnStopped, proxy.builtInOnStopped);
			proxy.eventOnChangedValue = ProxyInspectorUtil.EventField(target, "OnChangedValue", proxy.eventOnChangedValue, proxy.builtInOnChangedValue);
		}
	}
}
