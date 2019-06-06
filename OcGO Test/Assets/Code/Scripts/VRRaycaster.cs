using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRaycaster : MonoBehaviour
{
    public bool validHit;
    public RaycastHit lastHitObject;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out lastHitObject))
        {
            validHit = true;
        }
        else
        {
            validHit = false;
        }
    }

    public bool GetIsHit()
    {
        return validHit;
    }

    public Vector3 GetHitPosition()
    {
        if (validHit)
            return lastHitObject.point;
        return Vector3.zero;
    }

    public GameObject GetHitObject()
    {
        if (validHit)
            return lastHitObject.collider.gameObject;

        return null;
    }

}
