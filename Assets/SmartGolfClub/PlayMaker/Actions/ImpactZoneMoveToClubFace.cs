using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("ImpactZone.MoveToClubFace()")]
	public class ImpactZoneMoveToClubFace : FsmStateAction
	{
		[RequiredField]
		public ImpactZone impactZone;
		[RequiredField]
		public ClubGeometry clubGeometry;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(impactZone != null && clubGeometry != null)
				impactZone.MoveToClubFace(clubGeometry);
			
			Finish();
		}
	}
}
