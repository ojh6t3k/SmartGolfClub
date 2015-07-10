using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("ClubGeometry.Initialize()")]
	public class ClubGeometryInitialize : FsmStateAction
	{
		[RequiredField]
		public ClubGeometry clubGeometry;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(clubGeometry != null)
				clubGeometry.Initialize();
			
			Finish();
		}
	}
}
