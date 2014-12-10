using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityRobot;

[RequireComponent(typeof(Animation))]
public class GolfSwingTrajectory : MonoBehaviour
{
	public Animator avatarAnimator;
	public Transform animationTarget;
	public Transform trajectoryTarget;
	public imuModule imu;
	public GolfPoseController golfPose;
	public Material lineColor;
	public float speedPlayback = 1f;
	public bool recordingFile = false;

	private bool _start = false;
	private bool _recording = false;
	private bool _playing = false;
	private int _waitFrameCount;
	private bool _samplingTrajectory = false;
	private AnimationCurve _curveAnimationTargetRotX;
	private AnimationCurve _curveAnimationTargetRotY;
	private AnimationCurve _curveAnimationTargetRotZ;
	private AnimationCurve _curveAnimationTargetRotW;
	private AnimationCurve _curveTrajectoryTargetPosX;
	private AnimationCurve _curveTrajectoryTargetPosY;
	private AnimationCurve _curveTrajectoryTargetPosZ;
	private float _time;
	private float _recordingTime;
	private float _animationTime;
	private Vector3 _animationStartAngle;
	private Quaternion _samplingRot;
	private Vector3 _samplingPos;
	private StreamWriter _file;

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
				Quaternion rot = animationTarget.rotation;
				if(_samplingRot != rot)
				{
			//		value.x = NormalizeAngle(value.x);
			//		value.y = NormalizeAngle(value.y);
			//		value.z = NormalizeAngle(value.z);

					_curveAnimationTargetRotX.AddKey(new Keyframe(_time, rot.x));
					_curveAnimationTargetRotY.AddKey(new Keyframe(_time, rot.y));
					_curveAnimationTargetRotZ.AddKey(new Keyframe(_time, rot.z));
					_curveAnimationTargetRotW.AddKey(new Keyframe(_time, rot.w));
					_samplingRot = rot;
				//	if(_file != null)
				//		_file.WriteLine(string.Format("{0:F},{1:F},{2:F},{3:F}", value.x, value.y, value.z, _time));
				}
				_time += Time.deltaTime;
			}
			else if(_playing == true)
			{
				float t = _time * speedPlayback;
				animationTarget.rotation = new Quaternion(_curveAnimationTargetRotX.Evaluate(t)
				                                          ,_curveAnimationTargetRotY.Evaluate(t)
				                                          ,_curveAnimationTargetRotZ.Evaluate(t)
				                                          ,_curveAnimationTargetRotW.Evaluate(t));

				if(_waitFrameCount > 10)
				{
					if(_samplingTrajectory == false)
					{
						Vector3 pos = trajectoryTarget.position;
						if(_samplingPos != pos)
						{
							_curveTrajectoryTargetPosX.AddKey(new Keyframe(t, pos.x));
							_curveTrajectoryTargetPosY.AddKey(new Keyframe(t, pos.y));
							_curveTrajectoryTargetPosZ.AddKey(new Keyframe(t, pos.z));
							_samplingPos = pos;
						}
					}

					_time += Time.deltaTime;
					if(_time > _animationTime)
					{
						_samplingTrajectory = true;
						avatarAnimator.SetBool("Drive", false);
						golfPose.Reset();
						_playing = false;
					}
				}
				else
					_waitFrameCount++;
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

	float NormalizeAngle(float angle)
	{
		while(angle > 180f)
			angle -= 360f;
		while(angle < -180f)
			angle += 360f;

		return angle;
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

		_curveAnimationTargetRotX = new AnimationCurve();
		_curveAnimationTargetRotY = new AnimationCurve();
		_curveAnimationTargetRotZ = new AnimationCurve();
		_curveAnimationTargetRotW = new AnimationCurve();
		_time = 0f;
		_animationStartAngle = animationTarget.rotation.eulerAngles;
		_samplingRot = Quaternion.identity;
		_samplingTrajectory = false;

		if(recordingFile == true)
			_file = new StreamWriter("RecordingLog.txt");
		else
			_file = null;

		_recording = true;
	}

	public void EndRecording()
	{
		if(_recording == false)
			return;

		if(_file != null)
			_file.Close();

		_recording = false;
		_recordingTime = _time;

	//	NoiseFiltering(_curveAnimationTargetAngleX.keys);
	//	NoiseFiltering(_curveAnimationTargetAngleY.keys);
	//	NoiseFiltering(_curveAnimationTargetAngleZ.keys);
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
			_samplingPos = Vector3.zero;
		}

		_animationTime = _recordingTime / speedPlayback;
		_time = 0f;
		avatarAnimator.SetBool("Drive", true);
		_waitFrameCount = 0;
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
