using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingAnimator")]
	public class SwingAnimator : MonoBehaviour
	{
		public Animator animator;
		public string swingState = "Swing";
		public int layer = 0;
		public float speed = 1f;
		public AnimationClip swingClip;
		public SwingCurve swingCurve;

		private float _normalizedTime;
		private bool _creatingCurve = false;
		private float _time;

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_creatingCurve == true)
			{
				if(_time >= swingClip.length)
				{
					_creatingCurve = false;
					swingCurve.RecordingStop();
				}
				else
					_time += Time.deltaTime;
			}
		}

		public void Swing()
		{
			if(animator != null)
			{
				animator.speed = speed;
				animator.Play(swingState, layer, 0f);
			}
		}

		public void UpSwingPose(float rollAngle)
		{
			if(animator == null || swingCurve == null)
				return;

			if(swingCurve.dataEnabled == false)
				return;

			normalizedTime = swingCurve.upswingMap.Evaluate(rollAngle) / swingClip.length;
		}

		public void DownSwingPose(float rollAngle)
		{
			if(animator == null || swingCurve == null)
				return;

			if(swingCurve.dataEnabled == false)
				return;

			normalizedTime = swingCurve.downswingMap.Evaluate(rollAngle) / swingClip.length;
		}

		public float normalizedTime
		{
			set
			{
				if(_normalizedTime != value)
				{
					_normalizedTime = value;
					if(animator != null)
					{
						animator.speed = 0f;
						animator.Play(swingState, layer, _normalizedTime);
						animator.
					}
				}
			}
			get
			{
				return _normalizedTime;
			}
		}

		public bool isCreatingCurve
		{
			get
			{
				return _creatingCurve;
			}
		}

		public void CreateCurve()
		{
			if(swingCurve == null)
				return;

			_time = 0f;
			_creatingCurve = true;
			swingCurve.RecordingStart();
			Swing();
		}
	}
}
