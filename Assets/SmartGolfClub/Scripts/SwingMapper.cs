using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using SmartMaker;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingMapper")]
	public class SwingMapper : MonoBehaviour
	{
		public SwingAnimator referenceAnimator;
		public SwingAnimator controlAnimator;
		public ClubGeometry userClub;
		public SwingCurve userCurve;
		public MPU9150 imu;
		public float replaySpeed = 1f;

		public UnityEvent OnReplayStarted;
		public UnityEvent OnReplayStopped;

		private bool _realtimeMapping = false;
		private bool _replaying = false;
		private float _time;
		private float _totalTime;


		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_replaying == true)
			{
				if(_time == _totalTime)
					ReplayStop();
				else
				{
					float t = _time * replaySpeed;
					float rollAngle = userCurve.rollAngles.Evaluate(t);
					if(t < userCurve.topTime)
					{
						referenceAnimator.UpSwingPose(rollAngle);
						controlAnimator.UpSwingPose(rollAngle);
					}
					else
					{
						referenceAnimator.DownSwingPose(rollAngle);
						controlAnimator.DownSwingPose(rollAngle);
					}

					imu.target.rotation = Quaternion.FromToRotation(userClub.clubDirection, -userCurve.GetClubDirection(t)) * imu.target.rotation;

					_time += Time.deltaTime;
					_time = Mathf.Min(_time, _totalTime);
				}
			}
			else
			{
				if(_realtimeMapping == true)
				{
					float rollAngle = userClub.rollAngle;
					referenceAnimator.DownSwingPose(rollAngle);
					controlAnimator.DownSwingPose(rollAngle);
				}
			}		
		}

		public bool realtimeMapping
		{
			get
			{
				return _realtimeMapping;
			}
			set
			{
				if(userClub == null || referenceAnimator == null || controlAnimator == null || imu == null)
					return;

				if(_replaying == true)
					return;

				_realtimeMapping = value;
				imu.enabled = _realtimeMapping;
			}
		}

		public bool isReplaying
		{
			get
			{
				return _replaying;
			}
		}

		public void ReplayStart()
		{
			if(userCurve == null || referenceAnimator == null || controlAnimator == null)
				return;

			if(userCurve.dataEnabled == false)
				return;

			realtimeMapping = false;
			_time = 0f;
			_totalTime = userCurve.totalTime / replaySpeed;
			_replaying = true;
			OnReplayStarted.Invoke();
		}

		public void ReplayStop()
		{
			if(_replaying == false)
				return;

			_replaying = false;

			OnReplayStopped.Invoke();
		}
	}
}
