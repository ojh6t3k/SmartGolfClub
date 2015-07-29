using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("SwingCurve.GetClubAngle()")]
	public class SwingCurveGetClubAngle : FsmStateAction
	{
		[RequiredField]
		public SwingCurve swingCurve;

		[RequiredField]
		public FsmFloat time;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(swingCurve != null)
			{
				if(swingCurve.dataEnabled == true)
				{
					storeResult.Value = swingCurve.clubAngles.Evaluate(time.Value);
				}
			}
			
			Finish();
		}
	}
}

