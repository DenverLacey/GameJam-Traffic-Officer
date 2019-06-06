using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerActor : MonoBehaviour {

	[Tooltip("Button to tell a car to stop")]
	[SerializeField] private OVRInput.Button m_stopButton = OVRInput.Button.PrimaryIndexTrigger;

	[Tooltip("Button to tell a car to go")]
	[SerializeField] private OVRInput.Button m_goButton = OVRInput.Button.PrimaryTouchpad;

	[Tooltip("MAximum distance of pointer raycast")]
	[SerializeField] private float m_pointerDistance = 100f;

	LineRenderer m_laserPointer;

	TrafficManager m_trafficManager;

	public int Score { get; set; }

	/// <summary>
	/// Gets LineRenderer component and reference to traffic manager.
	/// </summary>
	private void Start() {
		m_laserPointer = GetComponent<LineRenderer>();
		m_trafficManager = FindObjectOfType<TrafficManager>();
	}

	/// <summary>
	/// Responds to player input and updates line renderer effect.
	/// </summary>
	void Update() {
		// Button inputs
		if (OVRInput.GetDown(m_stopButton))
			StopCar();
		else if (OVRInput.GetDown(m_goButton))
			ResumeCar();

		// line pointer effect
		Ray pointer = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(pointer, out hit, m_pointerDistance)) {
			m_laserPointer.SetPositions(new Vector3[] { transform.position, hit.point });
		}
		else {
			Vector3 laserEnd = transform.position + transform.forward * m_pointerDistance;
			m_laserPointer.SetPositions(new Vector3[] { transform.position, laserEnd });
		}
	}

	/// <summary>
	/// Stops car that player is pointing at. If player isn't pointing at a car
	/// it does nothing.
	/// </summary>
	void StopCar() {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, m_pointerDistance)) {
			if (hit.collider.tag == "Car")
            {
				m_trafficManager.StopCar(hit.collider.gameObject);
            }
		}
	}

	/// <summary>
	/// Resumes car that player is pointing at. If player isn't pointing at a car
	/// it does nothing.
	/// </summary>
	void ResumeCar() {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, m_pointerDistance)) {
			if (hit.collider.tag == "Car")
            {
				m_trafficManager.ResumeCar(hit.collider.gameObject);
            }
		}
	}
}
