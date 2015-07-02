using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Utility/SignalController")]
	public class SignalController : AppActionUtil
	{
		public AppAction appAction;
		public bool loop = false;
		public AnimationCurve[] signals;

		public int index = 0;
		public float multiplier = 1f;
		public float speed = 1f;
		public UnityEvent OnStarted;
		public UnityEvent OnCompleted;
		public UnityEvent OnStopped;

		private bool _playing;
		private float _endTime;
		private float _time;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_playing == true)
			{
				if(appAction != null)
					appAction.signalValue = signals[index].Evaluate(_time * speed) * multiplier;

				if(_time == _endTime)
				{
					if(loop == false)
					{
						_playing = false;
						OnCompleted.Invoke();
					}
				}
				else
				{
					_time += Time.deltaTime;
					if(_time > _endTime)
					{
						if(loop == true)
							_time -= _endTime;
						else
							_time = _endTime;
					}
				}
			}
		}

		public bool isPlaying
		{
			get
			{
				return _playing;
			}
		}

		public void Play()
		{
			if(index < 0f || index >= signals.Length)
			{
				Debug.LogError("Invalid index of Signal!");
				return;
			}

			if(speed == 0f)
			{
				Debug.LogError("Speed must be none zero!");
				return;
			}

			int keyNum = signals[index].length;
			if(keyNum < 2)
			{
				Debug.LogError("Signal's key number must be larger than one!");
				return;
			}

			_time = 0f;
			_endTime = signals[index].keys[keyNum - 1].time / speed;
			_playing = true;

			OnStarted.Invoke();
		}

		public void Stop()
		{
			_playing = false;

			if(appAction != null)
				appAction.signalValue = 0f;

			OnStopped.Invoke();
		}
	}
}
