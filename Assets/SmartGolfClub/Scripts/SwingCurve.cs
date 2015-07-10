using UnityEngine;
using System.Collections;
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

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_recording == true)
			{
				Vector3 pos = clubGeometry.clubUp.position - clubGeometry.characterCenter.position;
				_clubPosX.AddKey(new Keyframe(_time, pos.x));
				_clubPosY.AddKey(new Keyframe(_time, pos.y));
				_clubPosZ.AddKey(new Keyframe(_time, pos.z));

				Vector3 dir = clubGeometry.clubCenter.position - clubGeometry.clubUp.position;
				dir.Normalize();
				_clubDirX.AddKey(new Keyframe(_time, dir.x));
				_clubDirY.AddKey(new Keyframe(_time, dir.y));
				_clubDirZ.AddKey(new Keyframe(_time, dir.z));

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
			for(int i=0; i<rollAngles.length; i++)
			{
				if(i > 0 && i < (rollAngles.length - 2))
				{
					if(rollAngles[i].value == rollAngles[i-1].value)
					{
						rollAngles.RemoveKey(i);
						yawAngles.RemoveKey(i);
						clubAngles.RemoveKey(i);
						_clubPosX.RemoveKey(i);
						_clubPosY.RemoveKey(i);
						_clubPosZ.RemoveKey(i);
						_clubDirX.RemoveKey(i);
						_clubDirY.RemoveKey(i);
						_clubDirZ.RemoveKey(i);
						i--;
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
								_clubPosX.RemoveKey(i);
								_clubPosY.RemoveKey(i);
								_clubPosZ.RemoveKey(i);
								_clubDirX.RemoveKey(i);
								_clubDirY.RemoveKey(i);
								_clubDirZ.RemoveKey(i);
								i--;
							}
						}
					}
				}
			}

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

			CompleteCurve();
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
			float time, value;
			for(int i=0; i<rollAngles.length; i++)
			{
				if(i > 0)
					value = Mathf.Abs(rollAngles[i].value - rollAngles[i-1].value) / (rollAngles[i].time - rollAngles[i-1].time);
				else
					value = rollAngles[i].value;

				time = rollAngles[i].time;
				clubVelocities.AddKey(new Keyframe(time, value));
			}
			MakeSmoothCurve(ref clubVelocities);

			// Make Line
			Vector3 offset = clubGeometry.clubUp.position - clubGeometry.clubCenter.position;
			float clubLength = offset.magnitude;
			
			int upSwingKeyCount = 0;
			for(int i=0; i<rollAngles.length; i++)
			{
				if(rollAngles[i].time >= _topTime)
				{
					upSwingKeyCount = i + 1;
					break;
				}
			}
			if(upSwingLine != null)
			{
				upSwingLine.SetVertexCount(upSwingKeyCount);
				for(int i=0; i<upSwingKeyCount; i++)
				{
					Vector3 pos = new Vector3(_clubPosX[i].value
					                          ,_clubPosY[i].value
					                          ,_clubPosZ[i].value);
					Vector3 dir = new Vector3(_clubDirX[i].value
					                          ,_clubDirY[i].value
					                          ,_clubDirZ[i].value);
					dir.Normalize();
					pos += clubGeometry.characterCenter.position;
					upSwingLine.SetPosition(i, pos + dir * clubLength);
				}
			}
			
			int downSwingCount = 0;
			for(int i=upSwingKeyCount; i<rollAngles.length; i++)
			{
				if(rollAngles[i].time >= _finishTime)
				{
					downSwingCount = (i + 1) - upSwingKeyCount;
					break;
				}
			}
			if(downSwingLine != null)
			{
				downSwingLine.SetVertexCount(downSwingCount);
				for(int i=0; i<downSwingCount; i++)
				{
					Vector3 pos = new Vector3(_clubPosX[i+upSwingKeyCount].value
					                          ,_clubPosY[i+upSwingKeyCount].value
					                          ,_clubPosZ[i+upSwingKeyCount].value);
					Vector3 dir = new Vector3(_clubDirX[i+upSwingKeyCount].value
					                          ,_clubDirY[i+upSwingKeyCount].value
					                          ,_clubDirZ[i+upSwingKeyCount].value);
					dir.Normalize();
					pos += clubGeometry.characterCenter.position;
					downSwingLine.SetPosition(i, pos + dir * clubLength);
				}
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
