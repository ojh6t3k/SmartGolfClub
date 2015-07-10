using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(ArduinoAppProxy))]
public class ArduinoAppProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ArduinoAppProxy proxy = (ArduinoAppProxy)target;

		if(proxy.GetComponent<ArduinoApp>() == null)
		{
			EditorGUILayout.HelpBox("There is no ArduinoApp!", MessageType.Error);
		}
		else
		{
			proxy.eventOnConnected = ProxyInspectorUtil.EventField(target, "OnConnected", proxy.eventOnConnected, proxy.builtInOnConnected);
			proxy.eventOnConnectionFailed = ProxyInspectorUtil.EventField(target, "OnConnectionFailed", proxy.eventOnConnectionFailed, proxy.builtInOnConnectionFailed);
			proxy.eventOnDisconnected = ProxyInspectorUtil.EventField(target, "OnDisonnected", proxy.eventOnDisconnected, proxy.builtInOnDisconnected);
		}
	}
}
