using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(DigitalPin))]
public class DigitalPinInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DigitalPin digitalPin = (DigitalPin)target;

		digitalPin.id = EditorGUILayout.IntField("ID", digitalPin.id);
		digitalPin.mode = (DigitalPin.Mode)EditorGUILayout.EnumPopup("Mode", digitalPin.mode);
		if(digitalPin.mode == DigitalPin.Mode.OUTPUT)
		{
			int index = 0;
			if(digitalPin.digitalValue == true)
				index = 1;
			int newIndex = GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
			if(index != newIndex)
			{
				if(newIndex == 0)
					digitalPin.digitalValue = false;
				else
					digitalPin.digitalValue = true;
			}
		}
		else if(digitalPin.mode == DigitalPin.Mode.INPUT || digitalPin.mode == DigitalPin.Mode.INPUT_PULLUP)
		{
			int index = 0;
			if(digitalPin.digitalValue == true)
				index = 1;
			GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
		}
		else if(digitalPin.mode == DigitalPin.Mode.PWM)
		{
			digitalPin.analogValue = EditorGUILayout.Slider("Ratio", digitalPin.analogValue, 0f, 1f);
		}

		EditorUtility.SetDirty(target);
	}
}
