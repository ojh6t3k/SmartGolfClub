using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker.PlayMaker
{
	[AddComponentMenu("SmartMaker/PlayMaker/DigitalInputProxy")]
	public class DigitalInputProxy : MonoBehaviour
	{
		public readonly string builtInOnStarted = "DIGITAL INPUT / ON STARTED";
		public readonly string builtInOnStopped = "DIGITAL INPUT / ON STOPPED";
		public readonly string builtInOnChangedValue = "DIGITAL INPUT / ON CHANGED VALUE";
		
		public string eventOnStarted = "DIGITAL INPUT / ON STARTED";
		public string eventOnStopped = "DIGITAL INPUT / ON STOPPED";
		public string eventOnChangedValue = "DIGITAL INPUT / ON CHANGED VALUE";
		
		private PlayMakerFSM _fsm;
		private DigitalInput _digitalInput;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			_digitalInput = GetComponent<DigitalInput>();
			if(_digitalInput != null)
			{
				_digitalInput.OnStarted.AddListener(OnStarted);
				_digitalInput.OnStopped.AddListener(OnStopped);
				_digitalInput.OnChangedValue.AddListener(OnChangedValue);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnStarted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnStarted);
		}
		
		private void OnChangedValue()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnChangedValue);
		}
		
		private void OnStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnStopped);
		}
	}
}
