using UnityEngine;
using System.Collections;
using UnityRobot;

public class GolfPoseController : MonoBehaviour
{
	public Transform rightHand;
	public Transform leftHand;
	public Transform clubGrabber;

	public string yawVarName;
	public float rightYaw;
	public float leftYaw;

	protected Animator animator;

	private Vector3 _initGrabberUp;
	private Vector3 _oldGrabberUp;
	private float _grabberRoll;


	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
		_initGrabberUp = Vector3.Project(clubGrabber.up, Vector3.up) + Vector3.Project(clubGrabber.up, Vector3.right);
		_initGrabberUp.Normalize();
		Reset();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 newGrabberUp = Vector3.Project(clubGrabber.up, Vector3.up) + Vector3.Project(clubGrabber.up, Vector3.right);
		newGrabberUp.Normalize();
		_grabberRoll += GetAngle(_oldGrabberUp, newGrabberUp, Vector3.forward);
		_oldGrabberUp = newGrabberUp;

		float yaw = 0f;
		if(_grabberRoll > 0f)
			yaw = _grabberRoll / rightYaw;
		else if(_grabberRoll < 0f)
			yaw = _grabberRoll / leftYaw;
		yaw = Mathf.Clamp(yaw, -1f, 1f);
		animator.SetFloat(yawVarName, yaw);

	}
	
	void OnAnimatorIK()
	{
		// Right Hand
		animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);
		animator.SetIKRotation(AvatarIKGoal.RightHand,rightHand.rotation);
		
		// Left Hand
		animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1.0f);
		animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHand.rotation);
		
	}

	public void Reset()
	{
		_oldGrabberUp = _initGrabberUp;
		_grabberRoll = 0f;
	}

	float GetAngle(Vector3 from, Vector3 to, Vector3 up)
	{
		float angle = Vector3.Angle(from, to);
		float dir = Vector3.Dot(Vector3.Cross(from, to), up);		
		if (dir < 0f)
			angle = -angle;

		return angle;
	}
}
