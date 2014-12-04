using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("imuModule Calibration")]
	public class imuModuleCalibration : FsmStateAction
	{
		[RequiredField]
		public imuModule imu;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(imu != null)
			{
				imu.Calibration();
			}
			
			Finish();
		}
	}
}
