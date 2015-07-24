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

		public bool displayDebug = true;

		public UnityEvent OnReplayStarted;
		public UnityEvent OnReplayStopped;

		private bool _realtimeMapping = false;
		private bool _replaying = false;
		private float _replaySpeed;
		private float _time;
		private float _totalTime;


		// Use this for initialization
		void Start ()
		{
			_time = 0f;
			_replaySpeed = replaySpeed;
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
					SetClub();

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

		public float time
		{
			get
			{
				return _time * _replaySpeed;
			}
			set
			{
				if(userCurve == null || referenceAnimator == null || controlAnimator == null)
					return;
				
				if(userCurve.dataEnabled == false)
					return;
				
				if(_replaying == true)
					return;

				realtimeMapping = false;

				_time = Mathf.Clamp(value, 0f, userCurve.totalTime) / _replaySpeed;
				SetClub();
			}
		}

		Vector3 a;
		Vector3 b;
		Vector3 c;

		void OnDrawGizmos()
		{
			if(displayDebug == false)
				return;

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(userClub.clubCenter.position, userClub.clubCenter.position + a * userClub.clubLength);

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(userClub.clubCenter.position, userClub.clubCenter.position + b * userClub.clubLength);

			Gizmos.color = Color.yellow;
			Vector3 offset = userClub.clubForward.position - userClub.clubCenter.position;
			Gizmos.DrawSphere(c, offset.magnitude * 0.2f);
		}

		private void SetClub()
		{
			float t = _time * _replaySpeed;
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

			Vector3 from = userClub.clubDirUp;
			Vector3 to = userCurve.EvaluateClubUpDirection(t);
			imu.target.rotation = Quaternion.FromToRotation(from, to) * imu.target.rotation;
			
			from = userClub.clubDirForward;
			Vector3 axis = to;
			to = userCurve.EvaluateClubForwardDirection(t);
			float angle = ClubGeometry.GetAngle(from, to, axis);
			imu.target.rotation = Quaternion.AngleAxis(-angle, axis) * imu.target.rotation;

			if(displayDebug == true)
			{
				a = userCurve.EvaluateClubUpDirection(t);
				b = userCurve.EvaluateClubForwardDirection(t);
				c = userCurve.EvaluateClubUpPosition(t);
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
			_replaySpeed = replaySpeed;
			_totalTime = userCurve.totalTime / _replaySpeed;
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
