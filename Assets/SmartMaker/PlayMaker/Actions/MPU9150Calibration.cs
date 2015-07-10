using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("MPU9150.Calibration()")]
	public class MPU9150Calibration : FsmStateAction
	{
		[RequiredField]
		public MPU9150 mpu9150;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(mpu9150 != null)
				mpu9150.Calibration();
			
			Finish();
		}
	}
}
