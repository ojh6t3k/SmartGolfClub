using UnityEngine;
using System;
using System.Collections;
using UnityRobot;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UnityRobot")]
	[Tooltip("Listen Event from AnalogPinDrag")]
	public class AnalogPinDragEvent : FsmStateAction
	{
		[Tooltip("Drag Start Event")]
		public FsmEvent dragStartEvent;
		[Tooltip("Drag Move Event")]
		public FsmEvent dragMoveEvent;
		[Tooltip("Drag End Event")]
		public FsmEvent dragEndEvent;
				
		[Tooltip("Event Target FSM")]
		public PlayMakerFSM targetFSM;
		
		private AnalogPinDrag _analogPinDrag;
		
		public override void Awake ()
		{
			base.Awake ();
			
			_analogPinDrag = Owner.GetComponent<AnalogPinDrag>();
			if(_analogPinDrag == null)
				Debug.LogWarning("There exist no AnalogPinDrag!");
			else
			{
				_analogPinDrag.OnDragStart += OnDragStart;
				_analogPinDrag.OnDragMove += OnDragMove;
				_analogPinDrag.OnDragEnd += OnDragEnd;
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
		
		void OnDragStart(object sender, EventArgs e)
		{
			SendEvent(dragStartEvent);
		}
		
		void OnDragMove(object sender, EventArgs e)
		{
			SendEvent(dragMoveEvent);
		}
		
		void OnDragEnd(object sender, EventArgs e)
		{
			SendEvent(dragEndEvent);
		}		
	}
}
