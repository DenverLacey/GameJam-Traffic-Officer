using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuInteraction : MonoBehaviour {

	float m_pointerDistance;
	LineRenderer m_laserPointer;

	OVRInput.Button m_button;

    // Start is called before the first frame update
    void Start() {
		m_pointerDistance = GetComponent<PlayerActor>().PointerDistance;
		m_button = GetComponent<PlayerActor>().StopButton;
		m_laserPointer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
		// line pointer effect
		Ray pointer = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(pointer, out hit, m_pointerDistance)) {
			m_laserPointer.SetPositions(new Vector3[] { transform.position, hit.point });

			// check if pointing at traffic light
			TrafficLight tl = hit.collider.GetComponent<TrafficLight>();
			if (tl != null) {
				tl.OnPointedAt();

				if (OVRInput.GetDown(m_button)) {
					tl.OnClicked();
				}
			}
		}
		else {
			Vector3 laserEnd = transform.position + transform.forward * m_pointerDistance;
			m_laserPointer.SetPositions(new Vector3[] { transform.position, laserEnd });
		}
	}
}
