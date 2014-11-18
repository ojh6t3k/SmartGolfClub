using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Listen Event from DigitalPin")]
	public class DigitalPinEvent : FsmStateAction
	{
		[Tooltip("Started Event")]
		public FsmEvent startedEvent;
		[Tooltip("Stopped Event")]
		public FsmEvent stoppedEvent;
		[Tooltip("Updated Event")]
		public FsmEvent updatedEvent;
		[Tooltip("Changed Mode Event")]
		public FsmEvent changedModeEvent;
		[Tooltip("Changed Digital Input Event")]
		public FsmEvent changedInputEvent;

		[Tooltip("Event Target FSM")]
		public PlayMakerFSM targetFSM;
		
		private DigitalPin _digitalPin;

		public override void Awake ()
		{
			base.Awake ();

			_digitalPin = Owner.GetComponent<DigitalPin>();
			if(_digitalPin == null)
				Debug.LogWarning("There exist no DigitalPin!");
			else
			{
				_digitalPin.OnStarted += OnStarted;
				_digitalPin.OnStopped += OnStopped;
				_digitalPin.OnUpdated += OnUpdated;
				_digitalPin.OnChangedMode += OnChangedMode;
				_digitalPin.OnChangedDigitalInput += OnChangedDigitalInput;
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
		
		void OnChangedMode(object sender, EventArgs e)
		{
			SendEvent(changedModeEvent);
		}
		
		void OnChangedDigitalInput(object sender, EventArgs e)
		{
			SendEvent(changedInputEvent);
		}
	}
}
