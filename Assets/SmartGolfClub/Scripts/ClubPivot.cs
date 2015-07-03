using UnityEngine;
using System.Collections;

namespace SmartGolf
{
	[AddComponentMenu("SmartGolf/ClubPivot")]
	public class ClubPivot : MonoBehaviour
	{
		public Transform root;
		public Transform toPivot;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(root != null && toPivot != null)
			{
				root.position += (toPivot.position - transform.position);
			}		
		}
	}
}