using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityRobot;

[RequireComponent(typeof(Animation))]
public class GolfSwingTrajectory : MonoBehaviour
{
	public Animator avatarAnimator;
	public AnimationClip driveAnimation;
	public Transform animationTarget;
	public Transform trajectoryTarget;
	public imuModule imu;
	public Material lineColor;
	public float speedPlayback = 1f;

	private bool _start = false;
	private bool _recording = false;
	private bool _playing = false;
	private bool _samplingTrajectory = false;
	private AnimationCurve _curveAnimationTargetAngleX;
	private AnimationCurve _curveAnimationTargetAngleY;
	private AnimationCurve _curveAnimationTargetAngleZ;
	private AnimationCurve _curveTrajectoryTargetPosX;
	private AnimationCurve _curveTrajectoryTargetPosY;
	private AnimationCurve _curveTrajectoryTargetPosZ;
	private float _time;
	private float _recordingTime;
	private float _animationTime;
	private Vector3 _samplingValue;

	// Use this for initialization
	void Start ()
	{
		imu.OnStarted += OnStarted;
		imu.OnStopped += OnStopped;
		imu.OnUpdated += OnUpdated;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_start == true)
		{
			if(_recording == true)
			{
				Vector3 value = animationTarget.rotation.eulerAngles;
				if(_samplingValue != value)
				{
					_curveAnimationTargetAngleX.AddKey(new Keyframe(_time, value.x));
					_curveAnimationTargetAngleY.AddKey(new Keyframe(_time, value.y));
					_curveAnimationTargetAngleZ.AddKey(new Keyframe(_time, value.z));
					_samplingValue = value;
				}
				_time += Time.deltaTime;
			}
			else if(_playing == true)
			{
				Vector3 value = Vector3.zero;
				float t = _time * speedPlayback;
				value.x = _curveAnimationTargetAngleX.Evaluate(t);
				value.y = _curveAnimationTargetAngleY.Evaluate(t);
				value.z = _curveAnimationTargetAngleZ.Evaluate(t);
				animationTarget.rotation = Quaternion.Euler(value);

				if(_samplingTrajectory == false)
				{
					value = trajectoryTarget.position;
					if(_samplingValue != value)
					{
						_curveTrajectoryTargetPosX.AddKey(new Keyframe(t, value.x));
						_curveTrajectoryTargetPosY.AddKey(new Keyframe(t, value.y));
						_curveTrajectoryTargetPosZ.AddKey(new Keyframe(t, value.z));
						_samplingValue = value;
					}
				}

				_time += Time.deltaTime;
				if(_time > _animationTime)
				{
					_samplingTrajectory = true;
					_playing = false;
				}
			}
		}
	}

	void OnRenderObject()
	{
		if(_start == true)
		{
			if(_playing == true && _curveTrajectoryTargetPosX.keys.Length > 1)
			{
				lineColor.SetPass(0);
				GL.Begin(GL.LINES);
			//	GL.Color(lineColor.color);
				Vector3 pos = Vector3.zero;
		//		int count = _curveTrajectoryTargetPosX.keys.Length;
		//		for(int i=0; i<count; i++)
		//		{
		//			float t = _curveTrajectoryTargetPosX.keys[i].time;
		//			pos.x = _curveTrajectoryTargetPosX.keys[i].value;
		//			pos.y = _curveTrajectoryTargetPosY.keys[i].value;
		//			pos.z = _curveTrajectoryTargetPosZ.keys[i].value;
		//			if(pos != Vector3.zero)
		//			{
		//				GL.Vertex(pos);
		//				if(t > _time)
		//					break;
		//				if(i > 0 && i < (count - 1))
		//					GL.Vertex(pos);
		//			}
		//		}
				float unit = 0.01f;
				for(float t=0; t<(_time - unit); t+= unit)
				{
					pos.x = _curveTrajectoryTargetPosX.Evaluate(t * speedPlayback);
					pos.y = _curveTrajectoryTargetPosY.Evaluate(t * speedPlayback);
					pos.z = _curveTrajectoryTargetPosZ.Evaluate(t * speedPlayback);
					if(pos != Vector3.zero)
					{
						GL.Vertex(pos);
						if(t > 0 && (t + unit < _time))
							GL.Vertex(pos);
					}
				}
				GL.End();
			}
		}
	}

	public bool isRecording
	{
		get
		{
			return _recording;
		}
	}

	public bool isPlaying
	{
		get
		{
			return _playing;
		}
	}

	public void StartRecording()
	{
		if(_recording == true)
			return;

		_curveAnimationTargetAngleX = new AnimationCurve();
		_curveAnimationTargetAngleY = new AnimationCurve();
		_curveAnimationTargetAngleZ = new AnimationCurve();
		_time = 0f;
		_samplingValue = Vector3.zero;
		_samplingTrajectory = false;

		_recording = true;
	}

	public void EndRecording()
	{
		if(_recording == false)
			return;

		_recording = false;
		_recordingTime = _time;

		NoiseFiltering(_curveAnimationTargetAngleX.keys);
		NoiseFiltering(_curveAnimationTargetAngleY.keys);
		NoiseFiltering(_curveAnimationTargetAngleZ.keys);
	}

	public void PlayRecording()
	{
		if(_playing == true)
			return;

		if(_samplingTrajectory == false)
		{
			_curveTrajectoryTargetPosX = new AnimationCurve();
			_curveTrajectoryTargetPosY = new AnimationCurve();
			_curveTrajectoryTargetPosZ = new AnimationCurve();
			_samplingValue = Vector3.zero;
		}

		avatarAnimator.speed = driveAnimation.length / _recordingTime * speedPlayback;
		_animationTime = _recordingTime / speedPlayback;
		_time = 0f;
		avatarAnimator.SetTrigger("DriveAction");

		_playing = true;
	}

	public void StopRecording()
	{
		_playing = false;
	}

	public void PauseRecording()
	{
		_playing = false;
	}

	public void ResumeRecording()
	{
		_playing = true;
	}

	void OnStarted(object sender, EventArgs e)
	{
		_start = true;
	}
	
	void OnStopped(object sender, EventArgs e)
	{
		_start = false;
		EndRecording();
		StopRecording();
	}
	
	void OnUpdated(object sender, EventArgs e)
	{
	}

	void NoiseFiltering(Keyframe[] keys)
	{
		// Kalman filter's parameter
		float Q = 0.000001f;
		float R = 0.01f;
		float P = 1f;
		float X = 0f;
		float K = 0f;
		for(int i=0; i<keys.Length; i++)
		{
			K = (P + Q) / (P + Q + R);
			P = R * (P + Q) / (R + P + Q);
			keys[i].value = X + (keys[i].value - X) * K;
			X = keys[i].value;
		}
	}
}
