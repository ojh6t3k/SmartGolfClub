using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("SwingCurve.GetClubSpeed()")]
	public class SwingCurveGetClubSpeed : FsmStateAction
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
					float speed = swingCurve.clubVelocities.Evaluate(time.Value);
					storeResult.Value = speed / 1000f;
				}
			}
			
			Finish();
		}
	}
}

