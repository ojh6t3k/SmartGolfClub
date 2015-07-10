using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker.PlayMaker
{
	[AddComponentMenu("SmartMaker/PlayMaker/SignalControllerProxy")]
	public class SignalControllerProxy : MonoBehaviour
	{
		public readonly string builtInOnStarted = "SIGNAL CONTROLLER / ON STARTED";
		public readonly string builtInOnStopped = "SIGNAL CONTROLLER / ON STOPPED";
		public readonly string builtInOnCompleted = "SIGNAL CONTROLLER / ON COMPLETED";
		
		public string eventOnStarted = "SIGNAL CONTROLLER / ON STARTED";
		public string eventOnStopped = "SIGNAL CONTROLLER / ON STOPPED";
		public string eventOnCompleted = "SIGNAL CONTROLLER / ON COMPLETED";
		
		private PlayMakerFSM _fsm;
		private SignalController _signalController;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			_signalController = GetComponent<SignalController>();
			if(_signalController != null)
			{
				_signalController.OnStarted.AddListener(OnStarted);
				_signalController.OnStopped.AddListener(OnStopped);
				_signalController.OnCompleted.AddListener(OnCompleted);
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
		
		private void OnCompleted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnCompleted);
		}
		
		private void OnStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnStopped);
		}
	}
}
