using UnityEngine;
using System.Collections;
using System;
using SmartGolf;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Smart Golf")]
	[Tooltip("SwingMapper.ReplayStop()")]
	public class SwingMapperReplayStop : FsmStateAction
	{
		[RequiredField]
		public SwingMapper swingMapper;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(swingMapper != null)
				swingMapper.ReplayStop();
			
			Finish();
		}
	}
}

