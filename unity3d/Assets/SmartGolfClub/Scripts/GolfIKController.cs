using UnityEngine;
using System.Collections;
using UnityRobot;


public class GolfIKController : MonoBehaviour
{
	protected Animator animator;

	public Transform rightHand;
	public Transform leftHand;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void OnAnimatorIK()
	{
		// Right Hand
	//	animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
		animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);
	//	animator.SetIKPosition(AvatarIKGoal.RightHand,rightHand.position);
		animator.SetIKRotation(AvatarIKGoal.RightHand,rightHand.rotation);
		
		// Left Hand
	//	animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1.0f);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1.0f);
	//	animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHand.position);
		animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHand.rotation);

	}
}
