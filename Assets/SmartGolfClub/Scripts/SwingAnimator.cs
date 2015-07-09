﻿using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingAnimator")]
	public class SwingAnimator : MonoBehaviour
	{
		public Animator animator;
		public string swingState = "Swing";
		public int layer = 0;
		public float swingLength;
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
				if(_time >= swingLength)
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
				animator.speed = 1f;
				animator.Play(swingState, layer, 0f);
			}
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
