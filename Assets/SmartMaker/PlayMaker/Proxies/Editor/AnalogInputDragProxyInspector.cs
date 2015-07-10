using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(AnalogInputDragProxy))]
public class AnalogInputDragProxyInspector : Editor
{
	SerializedProperty analogInputDrag;

	void OnEnable()
	{
		analogInputDrag = serializedObject.FindProperty("analogInputDrag");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		AnalogInputDragProxy proxy = (AnalogInputDragProxy)target;

		EditorGUILayout.PropertyField(analogInputDrag, new GUIContent("analogInputDrag"));
		
		if(proxy.analogInputDrag == null)
		{
			EditorGUILayout.HelpBox("There is no AnalogInputDrag!", MessageType.Error);
		}
		else
		{
			proxy.eventOnDragStart = ProxyInspectorUtil.EventField(target, "OnDragStart", proxy.eventOnDragStart, proxy.builtInOnDragStart);
			proxy.eventOnDragMove = ProxyInspectorUtil.EventField(target, "OnDragMove", proxy.eventOnDragMove, proxy.builtInOnDragMove);
			proxy.eventOnDragEnd = ProxyInspectorUtil.EventField(target, "OnDragEnd", proxy.eventOnDragEnd, proxy.builtInOnDragEnd);
		}

		this.serializedObject.ApplyModifiedProperties();
	}
}
