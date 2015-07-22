using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Text;
using System.Xml;
using System.IO;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/SwingCurve")]
	public class SwingCurve : MonoBehaviour
	{
		public string dataName = "Swing Curve";
		public ClubGeometry clubGeometry;
		public TextAsset swingData;
		public float maxRecordingTime = 5f;
		public LineRenderer upSwingLine;
		public LineRenderer downSwingLine;

		[SerializeField]
		public AnimationCurve rollAngles = new AnimationCurve();
		[SerializeField]
		public AnimationCurve yawAngles = new AnimationCurve();
		[SerializeField]
		public AnimationCurve clubAngles = new AnimationCurve();
		[SerializeField]
		public AnimationCurve clubVelocities = new AnimationCurve();
		[SerializeField]
		public AnimationCurve upswingMap = new AnimationCurve();
		[SerializeField]
		public AnimationCurve downswingMap = new AnimationCurve();

		public UnityEvent OnRecordingStarted;
		public UnityEvent OnRecordingStopped;

		[SerializeField]
		private AnimationCurve _clubPosX;
		[SerializeField]
		private AnimationCurve _clubPosY;
		[SerializeField]
		private AnimationCurve _clubPosZ;
		[SerializeField]
		private AnimationCurve _clubUpX;
		[SerializeField]
		private AnimationCurve _clubUpY;
		[SerializeField]
		private AnimationCurve _clubUpZ;
		[SerializeField]
		private AnimationCurve _clubForwardX;
		[SerializeField]
		private AnimationCurve _clubForwardY;
		[SerializeField]
		private AnimationCurve _clubForwardZ;

		private bool _displayLine = false;
		[SerializeField]
		private bool _dataEnable = false;
		private bool _recording = false;
		private float _time;
		private float _preTime;
		[SerializeField]
		private float _totalTime;
		[SerializeField]
		private float _topTime;
		[SerializeField]
		private float _finishTime;
		private float _preRollAngle;
		private float _preRollAngleDiff;

		// Use this for initialization
		void Start ()
		{
			displayLine = _displayLine;		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_recording == true)
			{
				float curRollAngle = clubGeometry.rollAngle;
				float curRollAngleDiff = curRollAngle - _preRollAngle;
				if(_time == 0f)
				{
					if(Mathf.Abs(curRollAngleDiff) > 5f)
					{
						_time += Time.deltaTime;
						AddCurve(_time);
						_preRollAngle = curRollAngle;
						_preRollAngleDiff = curRollAngleDiff;
					}
				}
				else
				{
					_preTime = _time;
					_time += Time.deltaTime;

					if(Mathf.Abs(curRollAngleDiff) > 1f)
					{
						if(_preRollAngleDiff <= 0f && curRollAngleDiff > 0f)
						{
							_topTime = _preTime;
						}
						else if(_preRollAngleDiff >= 0f && curRollAngleDiff < 0f)
						{
							_time = _preTime;
							RecordingStop();
							return;
						}

						AddCurve(_time);
						_preRollAngle = curRollAngle;
						_preRollAngleDiff = curRollAngleDiff;
					}
				}

				if(_time > maxRecordingTime)
					RecordingStop();
			}
		}

		public void LoadCurve()
		{
			if(swingData == null)
				return;
			
			try
			{
				FromXML(swingData.text);
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to load SwingData!");
				Debug.LogError(e.Message);
				return;
			}
		}

		public void FromXML(string xml)
		{
			XmlDocument xmldoc = new XmlDocument();

			xmldoc.LoadXml(xml);

			ReadyCurve();
			
			XmlElement xmlEle;
			XmlNodeList xmlNodes;
			
			xmlEle = (XmlElement)xmldoc.SelectSingleNode("/SwingCurveData");
			dataName = xmlEle.Attributes["name"].Value;
			
			xmlEle = (XmlElement)xmldoc.SelectSingleNode("/SwingCurveData/SwingCurve");
			_topTime = float.Parse(xmlEle.Attributes["topTime"].Value);
			_finishTime = float.Parse(xmlEle.Attributes["finishTime"].Value);
			_totalTime = float.Parse(xmlEle.Attributes["totalTime"].Value);
			
			xmlNodes = xmlEle.SelectNodes("/SwingCurveData/SwingCurve/KeyFrame");
			for(int i=0; i<xmlNodes.Count; i++)
			{
				float time = float.Parse(xmlNodes[i].Attributes["time"].Value);

				string value = xmlNodes[i].Attributes["angle"].Value;
				string[] tokens = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				rollAngles.AddKey(new Keyframe(time, float.Parse(tokens[0])));
				yawAngles.AddKey(new Keyframe(time, float.Parse(tokens[1])));
				clubAngles.AddKey(new Keyframe(time, float.Parse(tokens[2])));
				
				value = xmlNodes[i].Attributes["pos"].Value;
				tokens = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				_clubPosX.AddKey(new Keyframe(time, float.Parse(tokens[0])));
				_clubPosY.AddKey(new Keyframe(time, float.Parse(tokens[1])));
				_clubPosZ.AddKey(new Keyframe(time, float.Parse(tokens[2])));
				
				value = xmlNodes[i].Attributes["up"].Value;
				tokens = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				_clubUpX.AddKey(new Keyframe(time, float.Parse(tokens[0])));
				_clubUpY.AddKey(new Keyframe(time, float.Parse(tokens[1])));
				_clubUpZ.AddKey(new Keyframe(time, float.Parse(tokens[2])));

				value = xmlNodes[i].Attributes["forward"].Value;
				tokens = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				_clubForwardX.AddKey(new Keyframe(time, float.Parse(tokens[0])));
				_clubForwardY.AddKey(new Keyframe(time, float.Parse(tokens[1])));
				_clubForwardZ.AddKey(new Keyframe(time, float.Parse(tokens[2])));
			}

			CompleteCurve();
		}

		public void SaveCurve(string file)
		{
			if(_dataEnable == false)
				return;
			if(file == null)
				return;
			if(file.Length == 0)
				return;

			XmlDocument xmldoc = ToXML();

			using(TextWriter writer = new StreamWriter(file, false, Encoding.UTF8))
			{
				xmldoc.Save(writer);
			}
		}

		public XmlDocument ToXML()
		{
			XmlDocument xmldoc = new XmlDocument();

			//Add the XML declaration section
			XmlDeclaration xmldecl;
			xmldecl = xmldoc.CreateXmlDeclaration("1.0", null, null);
			xmldecl.Encoding = "UTF-8";
			xmldecl.Standalone = "yes";
			xmldoc.AppendChild(xmldecl);
			
			XmlAttribute xmlattr;
			XmlElement xmlSwingData = xmldoc.CreateElement("SwingCurveData");
			xmlattr = xmldoc.CreateAttribute("name");
			xmlattr.Value = dataName;
			xmlSwingData.Attributes.Append(xmlattr);
			
			XmlElement xmlCurve = xmldoc.CreateElement("SwingCurve");
			xmlattr = xmldoc.CreateAttribute("topTime");
			xmlattr.Value = _topTime.ToString();
			xmlCurve.Attributes.Append(xmlattr);
			
			xmlattr = xmldoc.CreateAttribute("finishTime");
			xmlattr.Value = _finishTime.ToString();
			xmlCurve.Attributes.Append(xmlattr);
			
			xmlattr = xmldoc.CreateAttribute("totalTime");
			xmlattr.Value = _totalTime.ToString();
			xmlCurve.Attributes.Append(xmlattr);
			
			for(int i=0; i<rollAngles.length; i++)
			{
				XmlElement xmlKeyFrame = xmldoc.CreateElement("KeyFrame");
				
				xmlattr = xmldoc.CreateAttribute("time");
				xmlattr.Value = rollAngles[i].time.ToString();
				xmlKeyFrame.Attributes.Append(xmlattr);

				xmlattr = xmldoc.CreateAttribute("angle");
				xmlattr.Value = string.Format("{0:f},{1:f},{2:f}", rollAngles[i].value, yawAngles[i].value, clubAngles[i].value);
				xmlKeyFrame.Attributes.Append(xmlattr);
				
				xmlattr = xmldoc.CreateAttribute("pos");
				xmlattr.Value = string.Format("{0:f},{1:f},{2:f}", _clubPosX[i].value, _clubPosY[i].value, _clubPosZ[i].value);
				xmlKeyFrame.Attributes.Append(xmlattr);
				
				xmlattr = xmldoc.CreateAttribute("up");
				xmlattr.Value = string.Format("{0:f},{1:f},{2:f}", _clubUpX[i].value, _clubUpY[i].value, _clubUpZ[i].value);
				xmlKeyFrame.Attributes.Append(xmlattr);

				xmlattr = xmldoc.CreateAttribute("forward");
				xmlattr.Value = string.Format("{0:f},{1:f},{2:f}", _clubForwardX[i].value, _clubForwardY[i].value, _clubForwardZ[i].value);
				xmlKeyFrame.Attributes.Append(xmlattr);

				xmlCurve.AppendChild(xmlKeyFrame);
			}
			xmlSwingData.AppendChild(xmlCurve);
			
			xmldoc.AppendChild(xmlSwingData);

			return xmldoc;
		}

		public void RecordingStart()
		{
			if(clubGeometry == null || _recording == true)
				return;

			ReadyCurve();
			
			_time = 0f;
			_topTime = 0f;
			_finishTime = 0f;
			AddCurve(0f);
			_preRollAngle = clubGeometry.rollAngle;
			_preRollAngleDiff = 0f;
			_recording = true;

			OnRecordingStarted.Invoke();
		}

		public void RecordingStop()
		{
			if(_recording == false)
				return;

			_recording = false;
			_totalTime = _time;
			_finishTime = _totalTime;

			CompleteCurve();

			OnRecordingStopped.Invoke();
		}

		public void ClearCurve()
		{
			_dataEnable = false;
			
			rollAngles = new AnimationCurve();
			yawAngles = new AnimationCurve();
			clubAngles = new AnimationCurve();
			clubVelocities = new AnimationCurve();
			upswingMap = new AnimationCurve();
			downswingMap = new AnimationCurve();
			
			_clubPosX = new AnimationCurve();
			_clubPosY = new AnimationCurve();
			_clubPosZ = new AnimationCurve();
			_clubUpX = new AnimationCurve();
			_clubUpY = new AnimationCurve();
			_clubUpZ = new AnimationCurve();
			_clubForwardX = new AnimationCurve();
			_clubForwardY = new AnimationCurve();
			_clubForwardZ = new AnimationCurve();
		}

		private void ReadyCurve()
		{
			ClearCurve();
		}

		private void AddCurve(float time)
		{
			Vector3 pos = clubGeometry.ToRelativePosition(clubGeometry.clubUp.position);
			_clubPosX.AddKey(new Keyframe(time, pos.x));
			_clubPosY.AddKey(new Keyframe(time, pos.y));
			_clubPosZ.AddKey(new Keyframe(time, pos.z));
			
			Vector3 dir = clubGeometry.ToRelativeDirection(clubGeometry.clubDirUp);
			_clubUpX.AddKey(new Keyframe(time, dir.x));
			_clubUpY.AddKey(new Keyframe(time, dir.y));
			_clubUpZ.AddKey(new Keyframe(time, dir.z));

			dir = clubGeometry.ToRelativeDirection(clubGeometry.clubDirForward);
			_clubForwardX.AddKey(new Keyframe(time, dir.x));
			_clubForwardY.AddKey(new Keyframe(time, dir.y));
			_clubForwardZ.AddKey(new Keyframe(time, dir.z));
			
			rollAngles.AddKey(new Keyframe(time, clubGeometry.rollAngle));
			yawAngles.AddKey(new Keyframe(time, clubGeometry.yawAngle));
			clubAngles.AddKey(new Keyframe(time, clubGeometry.clubAngle));
		}

		private void CompleteCurve()
		{
			MakeSmoothCurve(ref rollAngles);
			MakeSmoothCurve(ref yawAngles);
			MakeSmoothCurve(ref clubAngles);
			MakeSmoothCurve(ref _clubPosX);
			MakeSmoothCurve(ref _clubPosY);
			MakeSmoothCurve(ref _clubPosZ);
			MakeSmoothCurve(ref _clubUpX);
			MakeSmoothCurve(ref _clubUpY);
			MakeSmoothCurve(ref _clubUpZ);
			MakeSmoothCurve(ref _clubForwardX);
			MakeSmoothCurve(ref _clubForwardY);
			MakeSmoothCurve(ref _clubForwardZ);

			// Make Velocity
			float time = 0f;
			float unitTime = 0.01f;
			float velocity;
			float curRollAngle;
			_preRollAngle = 0f;
			while(time < _totalTime)
			{
				velocity = 0f;
				if(time > 0f)
				{
					curRollAngle = rollAngles.Evaluate(time);
					velocity = Mathf.Abs((curRollAngle - _preRollAngle) / unitTime);
					_preRollAngle = curRollAngle;
				}

				clubVelocities.AddKey(new Keyframe(time, velocity));
				time += unitTime;
			}
			curRollAngle = rollAngles.Evaluate(_totalTime);
			velocity = Mathf.Abs((curRollAngle - _preRollAngle) / (_totalTime - (time - unitTime)));
			clubVelocities.AddKey(new Keyframe(_totalTime, velocity));

			MakeSmoothCurve(ref clubVelocities);

			// Make Swing Map
			unitTime = 0.02f;
			time = 0f;
			while(true)
			{
				upswingMap.AddKey(new Keyframe(rollAngles.Evaluate(time), time));
				if(time == _topTime)
					break;
				else
				{
					time += unitTime;
					time = Mathf.Min(time, _topTime);
				}
			}
			MakeSmoothCurve(ref upswingMap);

			time = _topTime;
			while(true)
			{
				downswingMap.AddKey(new Keyframe(rollAngles.Evaluate(time), time));
				if(time == _finishTime)
					break;
				else
				{
					time += unitTime;
					time = Mathf.Min(time, _finishTime);
				}
			}
			MakeSmoothCurve(ref downswingMap);

			_dataEnable = true;
		}

		public Vector3 EvaluateClubFacePosition(float time)
		{
			Vector3 upPos = EvaluateClubUpPosition(time);
			Vector3 dirUp = EvaluateClubUpDirection(time);
			Vector3 dirForward = EvaluateClubForwardDirection(time);

			return clubGeometry.EvaluateClubPosition(clubGeometry.clubFace.position, upPos, dirUp, dirForward);
		}

		public Vector3 EvaluateClubUpPosition(float time)
		{
			return clubGeometry.ToWorldPosition(new Vector3(_clubPosX.Evaluate(time), _clubPosY.Evaluate(time), _clubPosZ.Evaluate(time)));
		}

		public Vector3 EvaluateClubUpDirection(float time)
		{
			return clubGeometry.ToWorldDirection(new Vector3(_clubUpX.Evaluate(time), _clubUpY.Evaluate(time), _clubUpZ.Evaluate(time)));
		}

		public Vector3 EvaluateClubForwardDirection(float time)
		{
			return clubGeometry.ToWorldDirection(new Vector3(_clubForwardX.Evaluate(time), _clubForwardY.Evaluate(time), _clubForwardZ.Evaluate(time)));
		}

		private void MakeSmoothCurve(ref AnimationCurve curve)
		{
			if(curve.length <= 1)
				return;

			AnimationCurve newCurve = new AnimationCurve();

			float x, y, tangent;
			float time, value, inTangent, outTangent;
			for(int i=0; i<curve.length; i++)
			{
				if(i == 0)
				{
					x = curve[i+1].time - curve[i].time;
					y = curve[i+1].value - curve[i].value;
					tangent = y / x;
					inTangent = 0f;
					outTangent = tangent;
				}
				else if(i == (curve.length - 1))
				{
					x = curve[i].time - curve[i-1].time;
					y = curve[i].value - curve[i-1].value;
					tangent = y / x;
					inTangent = tangent;
					outTangent = 0f;
				}
				else
				{
					if(Mathf.Sign(curve[i+1].value - curve[i].value) == Mathf.Sign(curve[i].value - curve[i-1].value))
					{
						x = curve[i+1].time - curve[i-1].time;
						y = curve[i+1].value - curve[i-1].value;
						tangent = y / x;
						inTangent = tangent;
						outTangent = tangent;
					}
					else
					{
						inTangent = 0f;
						outTangent = 0f;
					}
				}

				time = curve[i].time;
				value = curve[i].value;
				newCurve.AddKey(new Keyframe(time, value, inTangent, outTangent));
			}

			curve = newCurve;
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

		public bool displayLine
		{
			set
			{
				if(_displayLine == value)
					return;

				_displayLine = value;
				if(upSwingLine != null)
				{
					if(_displayLine == true && _dataEnable == true)
					{
						float unitTime = 0.02f;
						float time = 0f;
						int keyCount = (int)(_topTime / unitTime) + 1;
						if(upSwingLine != null)
						{
							upSwingLine.SetVertexCount(keyCount);
							if(keyCount > 0)
							{
								int i = 0;
								time = 0f;
								while(i < (keyCount - 1))
								{
									upSwingLine.SetPosition(i, EvaluateClubFacePosition(time));
									
									time += unitTime;
									time = Math.Min(time, _topTime);
									i++;
								}
							}
							upSwingLine.SetPosition(keyCount - 1, EvaluateClubFacePosition(_topTime));
						}
					}
					upSwingLine.enabled = _displayLine;
				}
				if(downSwingLine != null)
				{
					if(_displayLine == true && _dataEnable == true)
					{
						float unitTime = 0.005f;
						float time = 0f;
						int keyCount = (int)((_finishTime - _topTime) / unitTime) + 1;
						if(downSwingLine != null)
						{
							downSwingLine.SetVertexCount(keyCount);
							if(keyCount > 0)
							{
								int i = 0;
								time = _topTime;
								while(i < (keyCount - 1))
								{
									downSwingLine.SetPosition(i, EvaluateClubFacePosition(time));
									
									time += unitTime;
									time = Math.Min(time, _finishTime);
									i++;
								}
							}
							downSwingLine.SetPosition(keyCount - 1, EvaluateClubFacePosition(_finishTime));
						}
					}
					downSwingLine.enabled = _displayLine;
				}
			}
			get
			{
				return _displayLine;
			}
		}
	}
}
