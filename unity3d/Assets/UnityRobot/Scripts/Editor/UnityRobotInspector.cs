using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;


[CustomEditor(typeof(UnityRobot.UnityRobot))]
public class UnityRobotInspector : Editor
{
	bool foldout = true;
	int indexModuleList = 0;


	public override void OnInspectorGUI()
	{
		UnityRobot.UnityRobot robot = (UnityRobot.UnityRobot)target;

		GUI.enabled = !robot.Connected;

		EditorGUILayout.BeginHorizontal();
		int index = -1;
		for(int i=0; i<robot.portNames.Count; i++)
		{
			if(robot.portName.Equals(robot.portNames[i]) == true)
				index = i;
		}
		index = EditorGUILayout.Popup("Port Name", index, robot.portNames.ToArray());
		if(index >= 0)
			robot.portName = robot.portNames[index];
		else
			robot.portName = "";
		if(GUILayout.Button("Search") == true)
			robot.PortSearch();
		EditorGUILayout.EndHorizontal();

		robot.baudrate = EditorGUILayout.IntField("Baudrate", robot.baudrate);
		robot.timeoutSec = EditorGUILayout.FloatField("Timeout(sec)", robot.timeoutSec);

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
			ArrayList modules = new ArrayList(robot.modules);

			EditorGUILayout.BeginHorizontal();
			string[] moduleList = new string[] { "Digital Pin"
				,"Analog Pin" };
			indexModuleList = EditorGUILayout.Popup(indexModuleList, moduleList, GUILayout.Width(120));
			if(GUILayout.Button("Create Module") == true)
			{
				GameObject go = new GameObject();
				go.transform.parent = robot.transform;
				go.name = moduleList[indexModuleList];
				switch(indexModuleList)
				{
				case 0:
					modules.Add(go.AddComponent<DigitalPin>());
					break;

				case 1:
					modules.Add(go.AddComponent<AnalogPin>());
					break;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Add Module") == true)
				modules.Add(null);
			if(modules.Count > 0)
			{
				if(GUILayout.Button("Remove All") == true)
					modules.Clear();
			}
			EditorGUILayout.EndHorizontal();

			for(int i=0; i<modules.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				modules[i] = EditorGUILayout.ObjectField(string.Format("Module {0:d}", i), (UnityEngine.Object)modules[i], typeof(UnityModule), true);
				if(clipboard == null)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				if(GUILayout.Button("Paste", GUILayout.Width(50)) == true)
				{
					modules[i] = clipboard;
				}
				if(i < (modules.Count - 1))
					GUI.enabled = true;
				else
					GUI.enabled = false;
				if(GUILayout.Button("+", GUILayout.Width(20)) == true)
				{
					modules.Insert(i, modules[i + 1]);
					modules.RemoveAt(i + 2);
				}
				if(i > 0)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				if(GUILayout.Button("-", GUILayout.Width(20)) == true)
				{
					modules.Insert(i - 1, modules[i]);
					modules.RemoveAt(i + 1);
				}
				GUI.enabled = true;
				if(GUILayout.Button("X", GUILayout.Width(20)) == true)
				{
					modules.RemoveAt(i);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}
			robot.modules = (UnityModule[])modules.ToArray(typeof(UnityModule));
		}
	}

	static UnityModule clipboard;

	[MenuItem( "CONTEXT/Component/Copy Component Reference" )]
	public static void CopyControlReference( MenuCommand command )
	{
		clipboard = command.context as UnityModule;
	}
}
