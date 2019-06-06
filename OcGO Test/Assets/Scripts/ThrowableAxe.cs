using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ThrowableAxe : MonoBehaviour {

    public Rigidbody axe;
    public float throwForce = 50;
    public Transform target, curve_point;
    private Vector3 old_pos;
    private bool isReturning = false;
    private bool hasTheWeapon = true;
    private float time = 0.0f;

    public float rotationSpeed;

    public Quaternion targetRotation;

	// Use this for initialization
	void Start ()
    {
        //targetRotation = target.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        targetRotation = target.rotation;
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            ThrowAxe();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            ReturnAxe();
        }

        if(isReturning)
        {
            if (time < 1.0)
            {
                axe.position = getBQCPoint(time, old_pos, curve_point.position, target.position);
                axe.rotation = Quaternion.Slerp(axe.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                time += Time.deltaTime;
            }
            else
            {
                ResetAxe();
            }
        }
    }

    void ThrowAxe()
    {
        if (hasTheWeapon == true)
        {
            isReturning = false;
            axe.transform.parent = null;
            axe.isKinematic = false; 
            axe.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * throwForce, ForceMode.Impulse);
            axe.AddTorque(axe.transform.TransformDirection(Vector3.forward) * rotationSpeed, ForceMode.Impulse);
            hasTheWeapon = false;

        }
        
    }

    void ReturnAxe()
    {
        time = 0.0f;
        old_pos = axe.position;
        isReturning = true;
        axe.velocity = Vector3.zero;

        axe.isKinematic = true;
    }

    void ResetAxe()
    {
        isReturning = false;
        axe.transform.parent = transform;
        axe.position = target.position;
        axe.rotation = targetRotation;
        hasTheWeapon = true;
    }

    Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }
}
