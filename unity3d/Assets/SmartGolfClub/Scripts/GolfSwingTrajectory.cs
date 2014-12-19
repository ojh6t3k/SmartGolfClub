using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityRobot;

public class GolfSwingTrajectory : MonoBehaviour
{
	public GolfPoseController glofPoseController;
	public Transform animationTarget;
	public Transform trajectoryTarget;
	public imuModule imu;
	public GolfPoseController golfPose;
	public Material lineColor;
	public float speedPlayback = 1f;
	public bool recordingFile = false;
	public WMG_Axis_Graph graph;
	public WMG_Series graphLine;
	public WMG_Series preGraphLine;
	public WMG_Grid indexGraphLine;

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
	private AnimationCurve _curveTrajectoryVelocity;
	private float _time;
	private float _recordingTime;
	private float _animationTime;
	private Vector3 _animationStartAngle;
	private Quaternion _samplingRot;
	private Quaternion _animationTargetInitRot;
	private Vector3 _samplingPos;
	private Vector3 _samplingPosDiff;
	private StreamWriter _file;
	private bool _displayTrajectory = false;
	private float _downswingTime = 0f;
	private int _index;

	// Use this for initialization
	void Start ()
	{
		_animationTargetInitRot = animationTarget.rotation;
		imu.OnStarted += OnStarted;
		imu.OnStopped += OnStopped;
		imu.OnUpdated += OnUpdated;
		imu.OnCalibrated += OnCalibrated;
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

				if(_downswingTime == 0f)
				{
					Vector3 pos = trajectoryTarget.position;
					if(_samplingPos != pos)
					{
						Vector3 posDiff = pos - _samplingPos;
						if(_samplingPosDiff != Vector3.zero)
						{
							float angle = Vector3.Angle(_samplingPosDiff, posDiff);
							if(angle > 90f)
								_downswingTime = _time;
						}
						_samplingPosDiff = posDiff;
						_samplingPos = pos;
					}
				}
						
				_time += Time.deltaTime;
			}
			else if(_playing == true)
			{
				if(_downswingTime > 0f)
					glofPoseController.DriveAction = _time / _downswingTime;

				if(_samplingTrajectory == false)
				{
					if(_waitFrameCount > 11)
					{
						Vector3 pos = trajectoryTarget.position;
						float time = _curveAnimationTargetRotX.keys[_index].time;
						_curveTrajectoryTargetPosX.AddKey(new Keyframe(time, pos.x));
						_curveTrajectoryTargetPosY.AddKey(new Keyframe(time, pos.y));
						_curveTrajectoryTargetPosZ.AddKey(new Keyframe(time, pos.z));
							
						float velocity = 0f;
						if(_curveTrajectoryTargetPosX.length > 1)
						{
							Vector3 posDiff = pos - _samplingPos;
							velocity = posDiff.magnitude;
						}
						_curveTrajectoryVelocity.AddKey(new Keyframe(time, velocity));
						_samplingPos = pos;

						_index++;
						_waitFrameCount--;
						if(_index >= _curveAnimationTargetRotX.length)
						{
							_samplingTrajectory = true;
							_playing = false;
							_waitFrameCount--;
						}
					}
						
					if(_waitFrameCount > 10)
					{
						if(_time >= _curveAnimationTargetRotX.keys[_index].time)
						{
							animationTarget.rotation = new Quaternion(_curveAnimationTargetRotX.keys[_index].value
							                                          ,_curveAnimationTargetRotY.keys[_index].value
							                                          ,_curveAnimationTargetRotZ.keys[_index].value
							                                          ,_curveAnimationTargetRotW.keys[_index].value);
						}
						else
							_waitFrameCount--;

						_time += Time.deltaTime;
					}
				}
				else
				{
					float time = _time * speedPlayback;

					if(_waitFrameCount > 10)
					{
						animationTarget.rotation = new Quaternion(_curveAnimationTargetRotX.Evaluate(time)
						                                          ,_curveAnimationTargetRotY.Evaluate(time)
						                                          ,_curveAnimationTargetRotZ.Evaluate(time)
						                                          ,_curveAnimationTargetRotW.Evaluate(time));
					}

					_waitFrameCount++;

					_time += Time.deltaTime;
					if(_time > _animationTime)
						_playing = false;
				}

				_waitFrameCount++;
			}
		}
	}

	void OnRenderObject()
	{
		if(_start == true)
		{
			if(_displayTrajectory == true)
			{
				if(_curveTrajectoryTargetPosX.keys.Length > 1)
				{
					lineColor.SetPass(0);
					GL.Begin(GL.LINES);
					Vector3 pos = Vector3.zero;
					float unit = 0.01f;
					float time = 0f;
					float speed = 1f;
					if(_playing == true && _samplingTrajectory == true)
						speed = speedPlayback;
					while(true)
					{
						pos.x = _curveTrajectoryTargetPosX.Evaluate(time * speed);
						pos.y = _curveTrajectoryTargetPosY.Evaluate(time * speed);
						pos.z = _curveTrajectoryTargetPosZ.Evaluate(time * speed);
						if(pos != Vector3.zero)
						{
							GL.Vertex(pos);
							if(time > 0 && time < _time)
								GL.Vertex(pos);
						}

						if(time < _time)
							time = Mathf.Min(time + unit, _time);
						else
							break;
					}
					GL.End();
				}
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
		_downswingTime = 0f;
		_animationStartAngle = animationTarget.rotation.eulerAngles;
		_samplingRot = Quaternion.identity;
		_samplingPos = Vector3.zero;
		_samplingPosDiff = Vector3.zero;
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
			_curveTrajectoryVelocity = new AnimationCurve();
			_samplingPos = Vector3.zero;
		}

		_animationTime = _recordingTime / speedPlayback;
		_time = 0f;
		_waitFrameCount = 0;
		_index = 0;
		imu.enabled = false;
		animationTarget.rotation = _animationTargetInitRot;
		golfPose.Reset();
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

	public void DrawGraph()
	{
		if(graphLine == null || _samplingTrajectory == false)
			return;

		List<Vector2> dataList = new List<Vector2>();
		for(float t=0f; t<_recordingTime; t+= 0.05f)
			dataList.Add(new Vector2(t, _curveTrajectoryVelocity.Evaluate(t)));

		preGraphLine.pointValues = graphLine.pointValues;
		graphLine.pointValues = dataList;
	}

	public bool DisplayTrajectory
	{
		get
		{
			return _displayTrajectory;
		}
		set
		{
			_displayTrajectory = value;
		}
	}

	public float AnimationIndex
	{
		get
		{
			return _time / _recordingTime;
		}
		set
		{
			if(value < 0f)
				glofPoseController.Reset();
			else
			{
				float var = Mathf.Clamp(value, 0f, 1f);
				_time = _recordingTime * var;
				animationTarget.rotation = new Quaternion(_curveAnimationTargetRotX.Evaluate(_time)
				                                          ,_curveAnimationTargetRotY.Evaluate(_time)
				                                          ,_curveAnimationTargetRotZ.Evaluate(_time)
				                                          ,_curveAnimationTargetRotW.Evaluate(_time));

				if(_downswingTime > 0f)
					glofPoseController.DriveAction = _time / _downswingTime;

				indexGraphLine.gridLinkLengthX = graph.xAxisLength * var;
			}
		}
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

	void OnCalibrated(object sender, EventArgs e)
	{
		golfPose.Reset();
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
