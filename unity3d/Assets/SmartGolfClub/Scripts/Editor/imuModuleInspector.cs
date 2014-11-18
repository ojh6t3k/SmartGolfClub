using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityRobot;

[CustomEditor(typeof(imuModule))]
public class imuModuleInspector : Editor
{
	public override void OnInspectorGUI()
	{
		imuModule imu = (imuModule)target;
		
		imu.id = EditorGUILayout.IntField("ID", imu.id);
		imu.target = (GameObject)EditorGUILayout.ObjectField("Target", imu.target, typeof(GameObject), true);
		imu.offsetAngle = EditorGUILayout.Vector3Field("Offset Angle", imu.offsetAngle);

		if(Application.isPlaying == true)
		{
			if(GUILayout.Button("Calibration") == true)
				imu.Calibration();
		}
	}
}
