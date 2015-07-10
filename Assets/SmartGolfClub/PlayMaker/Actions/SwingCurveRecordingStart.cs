using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("SwingCurve.RecordingStart()")]
	public class SwingCurveRecordingStart : FsmStateAction
	{
		[RequiredField]
		public SwingCurve swingCurve;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(swingCurve != null)
				swingCurve.RecordingStart();
			
			Finish();
		}
	}
}

