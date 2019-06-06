using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {

    public bool activated;

    public float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (activated)
        {
            GetComponent<Rigidbody>().AddTorque(Vector3.right * rotationSpeed, ForceMode.Impulse);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
