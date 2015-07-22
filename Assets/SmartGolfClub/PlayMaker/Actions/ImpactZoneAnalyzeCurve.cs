using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("ImpactZone.AnalyzeCurve()")]
	public class ImpactZoneAnalyzeCurve : FsmStateAction
	{
		[RequiredField]
		public ImpactZone impactZone;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(impactZone != null)
				impactZone.AnalyzeCurve();
			
			Finish();
		}
	}
}
