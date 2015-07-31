using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;


namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/BallSimulator")]
	public class BallSimulator : MonoBehaviour
	{
		public Rigidbody ball;
		public float lengthUnit = 1f;
		public float forceScale = 1f;
		public Transform targetDir;
		public ImpactZone impactZone;
		public LineRenderer desiredLine;
		public LineRenderer ballLine;
		public bool diplayDebug = true;
		public RectTransform uiMiniMap;
		public RectTransform uiDesiredLine;
		public RectTransform uiBallLine;
		public RectTransform uiBall;
		public RectTransform uiStartPos;
		public Text uiDistance;
		public Text uiAngle;
		public float maxDistance = 1000f;

		public UnityEvent OnSimulationStarted;
		public UnityEvent OnSimulationStopped;

		private Vector3 _startPos;
		private Quaternion _startRot;
		private Vector3 _ballPos;
		private Vector3 _desiredPos;
		private bool _playing = false;
		private float _time;

		// Use this for initialization
		void Start ()
		{
			_startPos = ball.transform.position;
			_startRot = ball.transform.rotation;
			Reset();
		}
		
		// Update is called once per frame
		void Update ()
		{
		}

		void FixedUpdate()
		{
			if(_playing == true)
			{
				Vector3 offset = ball.transform.position - _startPos;
				_ballPos = Vector3.ProjectOnPlane(offset, Vector3.up) + _startPos;
				_desiredPos = Vector3.Project(offset, _startRot * Vector3.forward) + _startPos;
				
				if(desiredLine != null)
					desiredLine.SetPosition(1, _desiredPos);
				
				if(ballLine != null)
					ballLine.SetPosition(1, _ballPos);

				if(uiMiniMap != null)
				{
					float d = distance;
					float d2 = desiredDistance;
					float a = angle;
					uiDistance.text = string.Format("{0:f1}", d);
					uiAngle.text = string.Format("{0:f1}", a);
					uiDesiredLine.localScale = new Vector3(1, d2 / maxDistance, 1);
					uiBallLine.localScale = new Vector3(1, d / maxDistance, 1);
					uiBallLine.localEulerAngles = new Vector3(0f, 0f, a);
				}

				if(_time > 1f)
				{
					if(ball.transform.position.y <= _startPos.y)
					{
						ball.isKinematic = true;
						ball.angularVelocity = Vector3.zero;
						ball.velocity = Vector3.zero;
						_playing = false;
						OnSimulationStopped.Invoke();
					}
				}

				_time += Time.fixedDeltaTime;
			}
		}

		void OnDrawGizmos()
		{
			if(targetDir != null && ball != null)
			{
				Gizmos.color = Color.yellow;
				if(Application.isPlaying == false)
					Gizmos.DrawLine(ball.transform.position, targetDir.position);
				else
					Gizmos.DrawLine(_startPos, targetDir.position);
			}

			if(diplayDebug == true)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(_startPos, _ballPos);
				
				Gizmos.color = Color.red;
				Gizmos.DrawLine(_startPos, _desiredPos);
			}
		}

		public void Reset()
		{
			_playing = false;
			ball.isKinematic = true;
			ball.transform.position = _startPos;
			ball.transform.rotation = _startRot;
			_ballPos = _startPos;
			_desiredPos = _startPos;

			if(desiredLine != null)
			{
				desiredLine.SetVertexCount(2);
				desiredLine.SetPosition(0, _startPos);
				desiredLine.SetPosition(1, _desiredPos);
			}
			
			if(ballLine != null)
			{
				ballLine.SetVertexCount(2);
				ballLine.SetPosition(0, _startPos);
				ballLine.SetPosition(1, _ballPos);
			}
		}

		public float distance
		{
			get
			{
				return Vector3.Distance(_startPos, _ballPos) * lengthUnit;
			}
		}

		public float angle
		{
			get
			{
				Vector3 to = _ballPos - _startPos;
				if(to == Vector3.zero)
					return 0f;
				Vector3 from = _desiredPos - _startPos;
				return ClubGeometry.GetAngle(from, to, Vector3.up);
			}
		}

		public float desiredDistance
		{
			get
			{
				float d = distance;
				float a = angle;
				if(a == 0f)
					return d;
				else
					return (d * Mathf.Cos(Mathf.Abs(a) * Mathf.Deg2Rad));
			}
		}

		public bool isPlaying
		{
			get
			{
				return _playing;
			}
		}

		public void Play()
		{
			Reset();

			Vector3 offsetForce = Vector3.zero;

			if(targetDir != null)
			{
				Vector3 offset = targetDir.position - _startPos;
				offsetForce = new Vector3(ClubGeometry.ScalarOnVector(offset, (_startRot * Vector3.right))
				           			     ,ClubGeometry.ScalarOnVector(offset, (_startRot * Vector3.up))
				               		     ,ClubGeometry.ScalarOnVector(offset, (_startRot * Vector3.forward)));
			}
			else if(impactZone != null)
			{
				Vector3 dir = impactZone.hitForce;
				offsetForce = new Vector3(ClubGeometry.ScalarOnVector(dir, impactZone.transform.right)
				                          ,ClubGeometry.ScalarOnVector(dir, impactZone.transform.up)
				                          ,ClubGeometry.ScalarOnVector(dir, impactZone.transform.forward));
			}
			else
				return;

			Vector3 force = ball.transform.right * offsetForce.x
							+ ball.transform.up * offsetForce.y
							+ ball.transform.forward * offsetForce.z;
			ball.isKinematic = false;
			ball.WakeUp();
			ball.AddForce(force * forceScale, ForceMode.Impulse);
			_time = 0f;
			_playing = true;
			OnSimulationStarted.Invoke();

			if(uiMiniMap != null)
			{
				Vector2 pos = uiStartPos.anchoredPosition;
				uiBall.anchoredPosition = pos;

				uiDesiredLine.anchoredPosition = pos;
				Vector3 scale = uiDesiredLine.localScale;
				scale.y = 0;
				uiDesiredLine.localScale = scale;
				uiDesiredLine.gameObject.SetActive(true);

				uiBallLine.anchoredPosition = pos;
				scale = uiBallLine.localScale;
				scale.y = 0;
				uiBallLine.localScale = scale;
				uiBallLine.gameObject.SetActive(true);
			}
		}
	}
}
