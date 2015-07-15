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

		public AnimationCurve rollAngles = new AnimationCurve();
		public AnimationCurve yawAngles = new AnimationCurve();
		public AnimationCurve clubAngles = new AnimationCurve();
		public AnimationCurve clubVelocities = new AnimationCurve();

		public UnityEvent OnRecordingStarted;
		public UnityEvent OnRecordingStopped;

		private AnimationCurve _clubPosX;
		private AnimationCurve _clubPosY;
		private AnimationCurve _clubPosZ;
		private AnimationCurve _clubDirX;
		private AnimationCurve _clubDirY;
		private AnimationCurve _clubDirZ;

		private bool _displayLine = false;
		private bool _dataEnable = false;
		private bool _recording = false;
		private float _time;
		private float _totalTime;
		private float _topTime;
		private float _finishTime;
		private float _preRollAngle;
		private float _preRollAngleDiff;
		private Vector3 _characterCenter;
		private Vector3 _characterForward;
		private Vector3 _characterUp;
		private Vector3 _characterRight;

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
					_time += Time.deltaTime;

					if(Mathf.Abs(curRollAngleDiff) > 1f)
					{
						AddCurve(_time);
						if(_preRollAngleDiff <= 0f && curRollAngleDiff > 0f)
						{
							_topTime = _time;
						}
						else if(_preRollAngleDiff >= 0f && curRollAngleDiff < 0f)
						{
							_finishTime = _time;
							RecordingStop();
						}
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
				
				value = xmlNodes[i].Attributes["dir"].Value;
				tokens = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				_clubDirX.AddKey(new Keyframe(time, float.Parse(tokens[0])));
				_clubDirY.AddKey(new Keyframe(time, float.Parse(tokens[1])));
				_clubDirZ.AddKey(new Keyframe(time, float.Parse(tokens[2])));
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
				
				xmlattr = xmldoc.CreateAttribute("dir");
				xmlattr.Value = string.Format("{0:f},{1:f},{2:f}", _clubDirX[i].value, _clubDirY[i].value, _clubDirZ[i].value);
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
			if(_finishTime == 0f)
				_finishTime = _totalTime;

			CompleteCurve();

			OnRecordingStopped.Invoke();
		}

		private void ReadyCurve()
		{
			_dataEnable = false;
			
			rollAngles = new AnimationCurve();
			yawAngles = new AnimationCurve();
			clubAngles = new AnimationCurve();
			
			_clubPosX = new AnimationCurve();
			_clubPosY = new AnimationCurve();
			_clubPosZ = new AnimationCurve();
			_clubDirX = new AnimationCurve();
			_clubDirY = new AnimationCurve();
			_clubDirZ = new AnimationCurve();

			_characterCenter = clubGeometry.characterCenter.position;
			_characterForward = clubGeometry.characterForward.position - _characterCenter;
			_characterForward.Normalize();
			_characterUp = clubGeometry.characterUp.position - _characterCenter;
			_characterUp.Normalize();
			_characterRight = Vector3.Cross(_characterUp, _characterForward);
			_characterRight.Normalize();
		}

		private void AddCurve(float time)
		{
			Vector3 pos = clubGeometry.clubUp.position - _characterCenter;
			_clubPosX.AddKey(new Keyframe(time, ScalarOnVector(pos, _characterRight)));
			_clubPosY.AddKey(new Keyframe(time, ScalarOnVector(pos, _characterUp)));
			_clubPosZ.AddKey(new Keyframe(time, ScalarOnVector(pos, _characterForward)));
			
			Vector3 dir = clubGeometry.clubCenter.position - clubGeometry.clubUp.position;
			dir.Normalize();
			_clubDirX.AddKey(new Keyframe(time, ScalarOnVector(dir, _characterRight)));
			_clubDirY.AddKey(new Keyframe(time, ScalarOnVector(dir, _characterUp)));
			_clubDirZ.AddKey(new Keyframe(time, ScalarOnVector(dir, _characterForward)));
			
			rollAngles.AddKey(new Keyframe(time, clubGeometry.rollAngle));
			yawAngles.AddKey(new Keyframe(time, clubGeometry.yawAngle));
			clubAngles.AddKey(new Keyframe(time, clubGeometry.clubAngle));
		}

		private float ScalarOnVector(Vector3 vector, Vector3 onNormal)
		{
			Vector3 proj = Vector3.Project(vector, onNormal);
			float scalar = proj.magnitude;
			if(Vector3.Dot(proj, onNormal) < 0f)
				scalar = -scalar;

			return scalar;
		}

		private void CompleteCurve()
		{
			MakeSmoothCurve(ref rollAngles);
			MakeSmoothCurve(ref yawAngles);
			MakeSmoothCurve(ref clubAngles);
			MakeSmoothCurve(ref _clubPosX);
			MakeSmoothCurve(ref _clubPosY);
			MakeSmoothCurve(ref _clubPosZ);
			MakeSmoothCurve(ref _clubDirX);
			MakeSmoothCurve(ref _clubDirY);
			MakeSmoothCurve(ref _clubDirZ);

			// Make Velocity
			clubVelocities = new AnimationCurve();
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

			// Make Display Line
			Vector3 offset = clubGeometry.clubUp.position - clubGeometry.clubCenter.position;
			float clubLength = offset.magnitude;
			unitTime = 0.02f;
			int keyCount = (int)(_topTime / unitTime) + 1;
			Vector3 pos, dir;
			if(upSwingLine != null)
			{
				upSwingLine.SetVertexCount(keyCount);
				if(keyCount > 0)
				{
					int i = 0;
					time = 0f;
					while(i < (keyCount - 1))
					{
						pos = _clubPosX.Evaluate(time) * _characterRight + _clubPosY.Evaluate(time) * _characterUp + _clubPosZ.Evaluate(time) * _characterForward;
						dir = _clubDirX.Evaluate(time) * _characterRight + _clubDirY.Evaluate(time) * _characterUp + _clubDirZ.Evaluate(time) * _characterForward;
						dir.Normalize();
						upSwingLine.SetPosition(i, _characterCenter + pos + dir * clubLength);

						time += unitTime;
						time = Math.Min(time, _topTime);
						i++;
					}
				}
				pos = _clubPosX.Evaluate(_topTime) * _characterRight + _clubPosY.Evaluate(_topTime) * _characterUp + _clubPosZ.Evaluate(_topTime) * _characterForward;
				dir = _clubDirX.Evaluate(_topTime) * _characterRight + _clubDirY.Evaluate(_topTime) * _characterUp + _clubDirZ.Evaluate(_topTime) * _characterForward;
				dir.Normalize();
				upSwingLine.SetPosition(keyCount - 1, _characterCenter + pos + dir * clubLength);
			}

			unitTime = 0.005f;
			keyCount = (int)((_finishTime - _topTime) / unitTime) + 1;
			if(downSwingLine != null)
			{
				downSwingLine.SetVertexCount(keyCount);
				if(keyCount > 0)
				{
					int i = 0;
					time = _topTime;
					while(i < (keyCount - 1))
					{
						pos = _clubPosX.Evaluate(time) * _characterRight + _clubPosY.Evaluate(time) * _characterUp + _clubPosZ.Evaluate(time) * _characterForward;
						dir = _clubDirX.Evaluate(time) * _characterRight + _clubDirY.Evaluate(time) * _characterUp + _clubDirZ.Evaluate(time) * _characterForward;
						dir.Normalize();
						downSwingLine.SetPosition(i, _characterCenter + pos + dir * clubLength);
						
						time += unitTime;
						time = Math.Min(time, _finishTime);
						i++;
					}
				}
				pos = _clubPosX.Evaluate(_finishTime) * _characterRight + _clubPosY.Evaluate(_finishTime) * _characterUp + _clubPosZ.Evaluate(_finishTime) * _characterForward;
				dir = _clubDirX.Evaluate(_finishTime) * _characterRight + _clubDirY.Evaluate(_finishTime) * _characterUp + _clubDirZ.Evaluate(_finishTime) * _characterForward;
				dir.Normalize();
				downSwingLine.SetPosition(keyCount - 1, _characterCenter + pos + dir * clubLength);
			}

			_dataEnable = true;
		}

		private void MakeSmoothCurve(ref AnimationCurve curve)
		{
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
				_displayLine = value;
				if(upSwingLine != null)
					upSwingLine.enabled = _displayLine;
				if(downSwingLine != null)
					downSwingLine.enabled = _displayLine;
			}
			get
			{
				return _displayLine;
			}
		}
	}
}
