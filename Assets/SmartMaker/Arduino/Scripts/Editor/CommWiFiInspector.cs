using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommWiFi))]
public class CommWiFiInspector : Editor
{
	bool foldout = true;

	SerializedProperty owner;
	SerializedProperty ipAddress;
	SerializedProperty port;
	SerializedProperty usingLibrary;
	SerializedProperty baudrate;
	SerializedProperty streamClass;

	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		ipAddress = serializedObject.FindProperty("ipAddress");
		port = serializedObject.FindProperty("port");
		usingLibrary = serializedObject.FindProperty("usingLibrary");
		baudrate = serializedObject.FindProperty("baudrate");
		streamClass = serializedObject.FindProperty("streamClass");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		CommWiFi wifi = (CommWiFi)target;
		
		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));
		
		GUI.enabled = !wifi.IsOpen;

		EditorGUILayout.PropertyField(ipAddress, new GUIContent("IP Address"));
		EditorGUILayout.PropertyField(port, new GUIContent("Port"));
				
		if(Application.isPlaying == false)
		{
			foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
			if(foldout == true)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(usingLibrary, new GUIContent("Library"));
				if(wifi.usingLibrary == CommWiFi.LibraryClass.Serial)
				{
					EditorGUILayout.PropertyField(streamClass, new GUIContent("Stream Class"));
					EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
				}
				EditorGUI.indentLevel--;
			}
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
