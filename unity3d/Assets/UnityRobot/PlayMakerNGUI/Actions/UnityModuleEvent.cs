using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Listen Event from UnityModule")]
	public class UnityModuleEvent : FsmStateAction
	{
		[Tooltip("Started Event")]
		public FsmEvent startedEvent;
		[Tooltip("Stopped Event")]
		public FsmEvent stoppedEvent;
		[Tooltip("Updated Event")]
		public FsmEvent updatedEvent;

		[Tooltip("Event Target FSM")]
		public PlayMakerFSM targetFSM;
		
		private UnityModule _unityModule;
		
		public override void Awake ()
		{
			base.Awake ();
			
			_unityModule = Owner.GetComponent<UnityModule>();
			if(_unityModule == null)
				Debug.LogWarning("There exist no UnityModule!");
			else
			{
				_unityModule.OnStarted += OnStarted;
				_unityModule.OnStopped += OnStopped;
				_unityModule.OnUpdated += OnUpdated;
			}
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
		}
		
		public override void OnExit ()
		{
			base.OnExit ();
		}
		
		void SendEvent(FsmEvent fsmEvent)
		{
			if(this.Entered == true)
			{
				if(targetFSM == null)
					Fsm.BroadcastEvent(fsmEvent);
				else
					targetFSM.Fsm.Event(fsmEvent);
				
				Fsm.Event(fsmEvent);
			}
		}
		
		void OnStarted(object sender, EventArgs e)
		{
			SendEvent(startedEvent);
		}
		
		void OnStopped(object sender, EventArgs e)
		{
			SendEvent(stoppedEvent);
		}
		
		void OnUpdated(object sender, EventArgs e)
		{
			SendEvent(updatedEvent);
		}		
	}
}
