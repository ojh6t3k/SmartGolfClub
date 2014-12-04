using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(AnalogPinDrag))]
public class AnalogPinDragInspector : Editor
{
	public override void OnInspectorGUI()
	{
		AnalogPinDrag analogPinDrag = (AnalogPinDrag)target;

		analogPinDrag.analogPin = (AnalogPin)EditorGUILayout.ObjectField("Analog Pin", analogPinDrag.analogPin, typeof(AnalogPin), true);
		analogPinDrag.dragMinRatio = EditorGUILayout.Slider("Drag MinValue", analogPinDrag.dragMinRatio, 0f, 1f);
		analogPinDrag.dragMaxRatio = EditorGUILayout.Slider("Drag MaxValue", analogPinDrag.dragMaxRatio, 0f, 1f);
		analogPinDrag.dragForceScaler = EditorGUILayout.FloatField("Force Scaler", analogPinDrag.dragForceScaler);
		EditorGUILayout.LabelField("Is Dragging", analogPinDrag.isDragging.ToString());
		EditorGUILayout.LabelField("Value", analogPinDrag.Value.ToString("F2"));
		EditorGUILayout.LabelField("DragForce", analogPinDrag.DragForce.ToString("F2"));
		
		if(Application.isPlaying == true)
			EditorUtility.SetDirty(target);
	}
}
