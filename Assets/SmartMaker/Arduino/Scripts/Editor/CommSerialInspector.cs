using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommSerial))]
public class CommSerialInspector : Editor
{
	bool foldout = true;
	bool foldout2 = true;

	SerializedProperty owner;
	SerializedProperty portNames;
	SerializedProperty portName;
	SerializedProperty baudrate;
	SerializedProperty streamClass;
	SerializedProperty uiText;
	SerializedProperty uiPanel;
	SerializedProperty uiItem;

	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		portNames = serializedObject.FindProperty("portNames");
		portName = serializedObject.FindProperty("portName");
		baudrate = serializedObject.FindProperty("baudrate");
		streamClass = serializedObject.FindProperty("streamClass");
		uiText = serializedObject.FindProperty("uiText");
		uiPanel = serializedObject.FindProperty("uiPanel");
		uiItem = serializedObject.FindProperty("uiItem");
	}
	
	public override void OnInspectorGUI()
	{
#if !UNITY_STANDALONE
		EditorGUILayout.HelpBox("This component only can work on standalone platform(windows, osx, linux..)", MessageType.Error);
#endif
		this.serializedObject.Update();

		CommSerial serial = (CommSerial)target;

		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));

		GUI.enabled = !serial.IsOpen;

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
			serial.PortSearch();
		EditorGUILayout.EndHorizontal();

		if(Application.isPlaying == false)
		{
			foldout2 = EditorGUILayout.Foldout(foldout2, "Sketch Options");
			if(foldout2 == true)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(streamClass, new GUIContent("Stream Class"));
				EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
				EditorGUI.indentLevel--;
			}
		}

		foldout = EditorGUILayout.Foldout(foldout, "UI objects");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(uiText, new GUIContent("UI Text"));
			EditorGUILayout.PropertyField(uiPanel, new GUIContent("UI Panel"));
			EditorGUILayout.PropertyField(uiItem, new GUIContent("UI Item"));
			EditorGUI.indentLevel--;
		}

		this.serializedObject.ApplyModifiedProperties();
	}
}
