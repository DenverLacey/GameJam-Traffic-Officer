using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerMenuInteraction))]
public class PlayerActor : MonoBehaviour {

	[Tooltip("Explosion Prefab")]
	[SerializeField] GameObject m_explosionPrefab;

	[Tooltip("Button to tell a car to stop")]
	[SerializeField] private OVRInput.Button m_pointerButton = OVRInput.Button.PrimaryIndexTrigger;

	[Tooltip("Button to tell a car to go")]
	[SerializeField] private OVRInput.Button m_goButton = OVRInput.Button.PrimaryTouchpad;

	[Tooltip("MAximum distance of pointer raycast")]
	private float m_pointerDistance = 100f;

	public float PointerDistance { get => m_pointerDistance; }
	public OVRInput.Button PointerButton { get => m_pointerButton; }

	private bool m_deathLaser;

	/// <summary>
	/// Responds to player input and updates line renderer effect.
	/// </summary>
	void Update() {
		// Button inputs
		if (OVRInput.GetDown(m_pointerButton)) {
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_pointerDistance)) {
				// pointing at car
				if (hit.collider.tag == "Car")
					hit.collider.GetComponent<CarActor>().ChangeCarState();
				
				// poiting at building
				else if (hit.collider.tag == "Building" && m_deathLaser)
					BlowUpBuilding(hit.collider.gameObject);

				// poiting at easter egg
				else if (hit.collider.tag == "EasterEgg" && !m_deathLaser) {
					m_deathLaser = true;
					GetComponent<PlayerMenuInteraction>().ActivateDeathLaser();
				}
			}
		}
	}

	void BlowUpBuilding(GameObject building) {
		// deactivate building
		building.SetActive(false);

		// explosion effect
		Vector3 expPos = new Vector3(
			building.transform.position.x,
			0,
			building.transform.position.z
		);
		GameObject exp = Instantiate(m_explosionPrefab, expPos, Quaternion.identity);

		// schedule explosion object for destruction
		Destroy(exp, 5);
	}
}
