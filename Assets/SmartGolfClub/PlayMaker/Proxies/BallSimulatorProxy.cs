using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/PlayMaker/BallSimulatorProxy")]
	public class BallSimulatorProxy : MonoBehaviour
	{
		public BallSimulator ballSimulator;
		
		public readonly string builtInOnSimulationStarted = "BALL SIMULATOR / ON SIMULATION STARTED";
		public readonly string builtInOnSimulationStopped = "BALL SIMULATOR / ON SIMULATION STOPPED";
		
		public string eventOnSimulationStarted = "BALL SIMULATOR / ON SIMULATION STARTED";
		public string eventOnSimulationStopped = "BALL SIMULATOR / ON SIMULATION STOPPED";
		
		private PlayMakerFSM _fsm;
		private FsmEventTarget _fsmEventTarget;
		
		// Use this for initialization
		void Start ()
		{
			_fsm = FindObjectOfType<PlayMakerFSM>();
			if(_fsm == null)
				_fsm = gameObject.AddComponent<PlayMakerFSM>();
			
			if(ballSimulator != null)
			{
				ballSimulator.OnSimulationStarted.AddListener(OnSimulationStarted);
				ballSimulator.OnSimulationStopped.AddListener(OnSimulationStopped);
			}
			
			_fsmEventTarget = new FsmEventTarget();
			_fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			_fsmEventTarget.excludeSelf = false;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		private void OnSimulationStarted()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnSimulationStarted);
		}
		
		private void OnSimulationStopped()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnSimulationStopped);
		}
	}
}
