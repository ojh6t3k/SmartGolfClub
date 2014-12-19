using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OrbitControl : MonoBehaviour
{
	public GameObject lookAt;
	public float xAngle;
    public float yAngle;
    public float distanceFrom = 200f;
	public float distanceMinLimit = 10f;
	public float distanceMaxLimit = 2000f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public bool autoInput = false;

	// Use this for initialization
	void Start ()
	{
		Reset();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(lookAt != null)
		{
			if (autoInput == true)
            {
                if (Input.GetMouseButton(0) == true)
                {
                    xAngle += Input.GetAxis("Mouse X") * 5f;
                    yAngle -= Input.GetAxis("Mouse Y") * 5f;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    distanceFrom -= Input.GetAxis("Mouse ScrollWheel") * distanceFrom;
                }
            }

			distanceFrom = Mathf.Clamp(distanceFrom, distanceMinLimit, distanceMaxLimit);
			yAngle = ClampAngle(yAngle, yMinLimit, yMaxLimit);
	        transform.rotation = Quaternion.Euler(yAngle, xAngle, 0f);
	        transform.position = lookAt.transform.position - transform.forward * distanceFrom;
		}
	}
	
	public void Reset()
    {
		if(lookAt != null)
		{
	        transform.LookAt(lookAt.transform.position);
	        xAngle = transform.eulerAngles.y;
	        yAngle = transform.eulerAngles.x;	        
	        distanceFrom = Vector3.Distance(transform.position, lookAt.transform.position);
		}
    }
	
	private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -180f)
            angle = 360f + angle;
        if (angle > 180f)
            angle = angle - 360f;

        return Mathf.Clamp(angle, min, max);
    }
}
