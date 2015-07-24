using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("ImpactZone.MoveTo()")]
	public class ImpactZoneMoveTo : FsmStateAction
	{
		[RequiredField]
		public ImpactZone impactZone;
		[RequiredField]
		public FsmVector3 position;
		[RequiredField]
		public FsmVector3 direction;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(impactZone != null)
				impactZone.MoveTo(position.Value, direction.Value);
			
			Finish();
		}
	}
}
