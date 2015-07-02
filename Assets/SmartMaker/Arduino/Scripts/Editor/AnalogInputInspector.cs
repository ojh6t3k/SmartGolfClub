using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(AnalogInput))]
public class AnalogInputInspector : Editor
{
	bool foldout = true;

	SerializedProperty owner;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty resolution;
	SerializedProperty OnStarted;
	SerializedProperty OnExcuted;
	SerializedProperty OnStopped;

	SerializedProperty filter;
	SerializedProperty minValue;
	SerializedProperty maxValue;
	SerializedProperty smooth;
	SerializedProperty sensitivity;

	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		resolution = serializedObject.FindProperty("resolution");
		OnStarted = serializedObject.FindProperty("OnStarted");
		OnExcuted = serializedObject.FindProperty("OnExcuted");
		OnStopped = serializedObject.FindProperty("OnStopped");

		filter = serializedObject.FindProperty("filter");
		minValue = serializedObject.FindProperty("minValue");
		maxValue = serializedObject.FindProperty("maxValue");
		smooth = serializedObject.FindProperty("smooth");
		sensitivity = serializedObject.FindProperty("sensitivity");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		AnalogInput aInput = (AnalogInput)target;

		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));
		
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin (A_)"));
			EditorGUI.indentLevel--;
		}
		aInput.autoUpdate = EditorGUILayout.Toggle("Auto update", aInput.autoUpdate);

		EditorGUILayout.PropertyField(resolution, new GUIContent("Resolution"));
		EditorGUILayout.FloatField("Analog Value", aInput.Value);

		EditorGUILayout.PropertyField(filter, new GUIContent("Filter"));
		if(filter.boolValue == true)
		{
			EditorGUILayout.PropertyField(minValue, new GUIContent("Min Value"));
			EditorGUILayout.PropertyField(maxValue, new GUIContent("Max Value"));
			EditorGUILayout.PropertyField(smooth, new GUIContent("Smooth"));
			if(smooth.boolValue == true)
				EditorGUILayout.PropertyField(sensitivity, new GUIContent("Sensitivity"));

			if(Application.isPlaying == true)
			{
				if(GUILayout.Button("Reset") == true)
					aInput.Reset();
				
				DrawCurve(aInput.OriginValues, "Original Value", 0f, 1f);
				DrawCurve(aInput.FilterValues, "Filtered Value", 0f, 1f);

				EditorUtility.SetDirty(target);
			}
		}

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnStarted);
		EditorGUILayout.PropertyField(OnExcuted);
		EditorGUILayout.PropertyField(OnStopped);

		if(Application.isPlaying == true && aInput.autoUpdate == true)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}

	private void DrawCurve(float[] values, string label, float min, float max)
	{
		EditorGUILayout.Space();
		
		AnimationCurve curve = new AnimationCurve();
		float lastValue = 0f;
		if(values.Length > 0)
		{
			curve.AddKey(0f, max);
			curve.AddKey(0.1f, min);
			for(int i=0; i<values.Length; i++)
				curve.AddKey(0.1f * (i + 2), values[i]);
			lastValue = values[values.Length - 1];
		}
		EditorGUILayout.LabelField(label + string.Format(" {0:F2}", lastValue));
		GUI.enabled = false;
		EditorGUILayout.CurveField(curve, GUILayout.Height(100));
		GUI.enabled = true;
	}
}
