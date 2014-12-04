using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityRobot;


[CustomEditor(typeof(UnityRobot.UnityRobot))]
public class UnityRobotInspector : Editor
{
	bool foldout = true;
	int indexModuleList = 0;

	SerializedProperty portNames;
	SerializedProperty portName;
	SerializedProperty baudrate;
	SerializedProperty timeoutSec;
	SerializedProperty modules;

	void OnEnable()
	{
		portNames = serializedObject.FindProperty("portNames");
		portName = serializedObject.FindProperty("portName");
		baudrate = serializedObject.FindProperty("baudrate");
		timeoutSec = serializedObject.FindProperty("timeoutSec");
		modules = serializedObject.FindProperty("modules");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		UnityRobot.UnityRobot robot = (UnityRobot.UnityRobot)target;
		GUI.enabled = !robot.Connected;

		EditorGUILayout.BeginHorizontal();
		int index = -1;
		string[] list = new string[portNames.arraySize];
		for(int i=0; i<list.Length; i++)
		{
			list[i] = portNames.GetArrayElementAtIndex(i).stringValue;
			if(portName.stringValue.Equals(list[i]) == true)
				index = i;
		}
		index = EditorGUILayout.Popup("Port Name", index, list);
		if(index >= 0)
			portName.stringValue = list[index];
		else
			portName.stringValue = "";
		if(GUILayout.Button("Search") == true)
			robot.PortSearch();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
		EditorGUILayout.PropertyField(timeoutSec, new GUIContent("Timeout(sec)"));

		if(Application.isPlaying == true)
		{
			GUI.enabled = true;
			if(robot.Connected == true)
			{
				if(GUILayout.Button("Disconnect") == true)
					robot.Disconnect();
			}
			else
			{
				if(GUILayout.Button("Connect") == true)
					robot.Connect();
			}

			EditorUtility.SetDirty(target);
		}

		foldout = EditorGUILayout.Foldout(foldout, "Modules");
		if(foldout == true)
		{
			SerializedProperty module;

			EditorGUILayout.BeginHorizontal();
			string[] moduleList = new string[] { "Digital Pin"
												,"Analog Pin" };
			indexModuleList = EditorGUILayout.Popup(indexModuleList, moduleList, GUILayout.Width(120));
			if(GUILayout.Button("Create Module") == true)
			{
				modules.InsertArrayElementAtIndex(modules.arraySize);
				module = modules.GetArrayElementAtIndex(modules.arraySize - 1);
				GameObject go = new GameObject();
				go.transform.parent = robot.transform;
				go.name = moduleList[indexModuleList];
				switch(indexModuleList)
				{
				case 0:
					module.objectReferenceValue = go.AddComponent<DigitalPin>();
					break;

				case 1:
					module.objectReferenceValue = go.AddComponent<AnalogPin>();
					break;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Add Module") == true)
			{
				modules.InsertArrayElementAtIndex(modules.arraySize);
				module = modules.GetArrayElementAtIndex(modules.arraySize - 1);
				module.objectReferenceValue = null;
			}
			if(modules.arraySize > 0)
			{
				if(GUILayout.Button("Remove All") == true)
					modules.ClearArray();
			}
			EditorGUILayout.EndHorizontal();

			for(int i=0; i<modules.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				module = modules.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(module, new GUIContent(string.Format("Module {0:d}", i)));
				if(clipboard == null)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				if(GUILayout.Button("Paste", GUILayout.Width(50)) == true)
				{
					module.objectReferenceValue = clipboard;
				}
				if(i < (modules.arraySize - 1))
					GUI.enabled = true;
				else
					GUI.enabled = false;
				if(GUILayout.Button("+", GUILayout.Width(20)) == true)
				{
					modules.MoveArrayElement(i, i + 1);
				}
				if(i > 0)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				if(GUILayout.Button("-", GUILayout.Width(20)) == true)
				{
					modules.MoveArrayElement(i, i - 1);
				}
				GUI.enabled = true;
				if(GUILayout.Button("X", GUILayout.Width(20)) == true)
				{
					module.objectReferenceValue = null;
					modules.DeleteArrayElementAtIndex(i);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}

			this.serializedObject.ApplyModifiedProperties();
		}
	}

	static UnityModule clipboard;

	[MenuItem( "CONTEXT/Component/Copy Component Reference" )]
	public static void CopyControlReference( MenuCommand command )
	{
		clipboard = command.context as UnityModule;
	}
}
