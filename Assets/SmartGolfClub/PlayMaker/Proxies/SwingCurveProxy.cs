using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/PlayMaker/SwingCurveProxy")]
	public class SwingCurveProxy : MonoBehaviour
	{
		public SwingCurve swingCurve;
		
		public readonly string builtInOnRecordingStarted = "SWING CURVE / ON RECORDING STARTED";
		public readonly string builtInOnRecordingStopped = "SWING CURVE / ON RECORDING STOPPED";

		public string eventOnRecordingStarted = "SWING CURVE / ON RECORDING STARTED";
		public string eventOnRecordingStopped = "SWING CURVE / ON RECORDING STOPPED";

		private PlayMakerFSM _fsm;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			if(swingCurve != null)
			{
				swingCurve.OnRecordingStarted.AddListener(OnRecordingStarted);
				swingCurve.OnRecordingStopped.AddListener(OnRecordingStopped);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnRecordingStarted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnRecordingStarted);
		}
		
		private void OnRecordingStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnRecordingStopped);
		}
	}
}
