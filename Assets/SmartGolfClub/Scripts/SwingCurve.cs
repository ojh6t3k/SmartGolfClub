using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingCurve")]
	public class SwingCurve : MonoBehaviour
	{
		public ClubGeometry clubGeometry;
		public TextAsset swingData;
		public float maxRecordingTime = 5f;

		public AnimationCurve rollAngles = new AnimationCurve();
		public AnimationCurve yawAngles = new AnimationCurve();
		public AnimationCurve clubAngles = new AnimationCurve();
		public AnimationCurve clubVelocities = new AnimationCurve();

		private bool _dataEnable = false;
		private bool _recording = false;
		private float _time;
		private float _totalTime;
		private float _topTime;
		private float _finishTime;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_recording == true)
			{
				rollAngles.AddKey(new Keyframe(_time, clubGeometry.rollAngle));
				yawAngles.AddKey(new Keyframe(_time, clubGeometry.yawAngle));
				clubAngles.AddKey(new Keyframe(_time, clubGeometry.clubAngle));

				if(_time > maxRecordingTime)
					RecordingStop();
				else
					_time += Time.deltaTime;
			}
		}

		public void LoadCurve()
		{
			if(swingData == null)
				return;
		}

		public void RecordingStart()
		{
			if(clubGeometry == null || _recording == true)
				return;

			_dataEnable = false;
			
			rollAngles = new AnimationCurve();
			yawAngles = new AnimationCurve();
			clubAngles = new AnimationCurve();
			clubVelocities = new AnimationCurve();
			
			_time = 0f;
			_recording = true;
		}

		public void RecordingStop()
		{
			if(_recording == false)
				return;

			_recording = false;
			_totalTime = _time;

			// Noise Filtering
			bool sign1, sign2;
			int n = 0;
			for(int i=0; i<rollAngles.length; i++)
			{
				if(i > 0 && i < (rollAngles.length - 2))
				{
					if(rollAngles[i].value == rollAngles[i-1].value)
					{
						rollAngles.RemoveKey(i);
						yawAngles.RemoveKey(i);
						clubAngles.RemoveKey(i);
						i--;
						n++;
					}
					else
					{
						if(rollAngles[i].value - rollAngles[i-1].value >= 0f)
							sign1 = true;
						else
							sign1 = false;
						
						if(rollAngles[i+1].value - rollAngles[i].value >= 0f)
							sign2 = true;
						else
							sign2 = false;
						
						if(sign1 == sign2)
						{
							if(Mathf.Abs(rollAngles[i].value - rollAngles[i-1].value) <= 1f)
							{
								rollAngles.RemoveKey(i);
								yawAngles.RemoveKey(i);
								clubAngles.RemoveKey(i);
								i--;
								n++;
							}
						}
					}
				}
			}
			Debug.Log(n);

			// Find top/finish Time
			_topTime = 0f;
			_finishTime = 0f;
			for(int i=0; i<rollAngles.length; i++)
			{
				if(i > 0 && rollAngles[i].time > 0.5f)
				{
					if(_topTime == 0f)
					{
						if(rollAngles[i-1].value < rollAngles[i].value)
							_topTime = rollAngles[i-1].time;
					}
					else if(_finishTime == 0f)
					{
						if(rollAngles[i-1].value > rollAngles[i].value)
							_finishTime = rollAngles[i-1].time;
					}
					else
						break;
				}
			}

			// Find Velocity
			float keyTime, keyValue;
			for(int i=0; i<rollAngles.length; i++)
			{
				if(i > 0)
				{
					keyValue = Mathf.Abs(rollAngles[i].value - rollAngles[i-1].value) / (rollAngles[i].time - rollAngles[i-1].time);
					keyTime = rollAngles[i].time;
					clubVelocities.AddKey(new Keyframe(keyTime, keyValue));
				}
				else
				{
					keyValue = rollAngles[i].value;
					keyTime = rollAngles[i].time;
					clubVelocities.AddKey(new Keyframe(keyTime, keyValue));
				}
			}

			// Smooth Filtering
			for(int i=0; i<rollAngles.length; i++)
				rollAngles.SmoothTangents(i, 0f);			
			for(int i=0; i<yawAngles.length; i++)
				yawAngles.SmoothTangents(i, 0f);			
			for(int i=0; i<clubAngles.length; i++)
				clubAngles.SmoothTangents(i, 0f);
			for(int i=0; i<clubVelocities.length; i++)
				clubVelocities.SmoothTangents(i, 0f);
			
			_dataEnable = true;
		}

		public bool dataEnabled
		{
			get
			{
				return _dataEnable;
			}
		}

		public bool isRecording
		{
			get
			{
				return _recording;
			}
		}

		public float totalTime
		{
			get
			{
				return _totalTime;
			}
		}

		public float topTime
		{
			get
			{
				return _topTime;
			}
		}

		public float finishTime
		{
			get
			{
				return _finishTime;
			}
		}
	}
}
