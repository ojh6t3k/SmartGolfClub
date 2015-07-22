using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartGolf;

[CustomEditor(typeof(ImpactZone))]
public class ImpactZoneInspector : Editor
{
	SerializedProperty boundBox;
	SerializedProperty ball;
	SerializedProperty swingCurve;
	SerializedProperty displayDebug;

	void OnEnable()
	{
		boundBox = serializedObject.FindProperty("boundBox");
		ball = serializedObject.FindProperty("ball");
		swingCurve = serializedObject.FindProperty("swingCurve");
		displayDebug = serializedObject.FindProperty("displayDebug");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ImpactZone impactZone = (ImpactZone)target;
		
		EditorGUILayout.PropertyField(boundBox, new GUIContent("boundBox"));
		EditorGUILayout.PropertyField(ball, new GUIContent("ball"));
		EditorGUILayout.PropertyField(swingCurve, new GUIContent("swingCurve"));
		EditorGUILayout.PropertyField(displayDebug, new GUIContent("displayDebug"));

		if(Application.isPlaying == true)
		{
			if(GUILayout.Button("Analyze Curve") == true)
			{
				impactZone.AnalyzeCurve();
				EditorUtility.SetDirty(target);
			}

			if(impactZone.analyzed == true)
			{
				EditorGUILayout.FloatField("Impact Time", impactZone.impactTime);
				EditorGUILayout.FloatField("Path Angle", impactZone.pathAngle);
				EditorGUILayout.FloatField("Face Angle", impactZone.faceAngle);
				EditorGUILayout.FloatField("Attack Angle", impactZone.attackAngle);
			}
		}

		this.serializedObject.ApplyModifiedProperties();
	}
}
