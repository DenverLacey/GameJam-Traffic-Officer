using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerMenuInteraction))]
public class PlayerActor : MonoBehaviour {

	[Tooltip("Explosion Prefab")]
	[SerializeField] GameObject m_explosionPrefab;

	[Tooltip("Button to tell a car to stop and go")]
	[SerializeField] private OVRInput.Button m_pointerButton = OVRInput.Button.PrimaryIndexTrigger;

	[Tooltip("MAximum distance of pointer raycast")]
	private float m_pointerDistance = 100f;

	public float PointerDistance { get => m_pointerDistance; }
	public OVRInput.Button PointerButton { get => m_pointerButton; }

	private bool m_deathLaser;

	// explosion pool
	private Queue<GameObject> m_explosionQueue;

	private void Start() {
		m_explosionQueue = new Queue<GameObject>();
		for (int i = 0; i < 20; i++) {
			GameObject exp = Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			exp.SetActive(false);
			m_explosionQueue.Enqueue(exp);
		}
	}

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
					ActivateExplosion(hit.collider.gameObject);

				// poiting at easter egg
				else if (hit.collider.tag == "EasterEgg" && !m_deathLaser) {
					m_deathLaser = true;
					GetComponent<PlayerMenuInteraction>().ActivateDeathLaser();
				}
			}
		}
	}

	void ActivateExplosion(GameObject building) {
		// deactivate building
		building.SetActive(false);

		// explosion effect
		Vector3 expPos = new Vector3(
			building.transform.position.x,
			0,
			building.transform.position.z
		);

		GameObject exp = m_explosionQueue.Dequeue();
		exp.transform.position = expPos;
		exp.SetActive(true);

		// schedule explosion object for destruction
		StartCoroutine(DeactivateExplosion(exp, 3));
	}

	IEnumerator DeactivateExplosion(GameObject explosion, float delay) {
		yield return new WaitForSeconds(delay);
		explosion.SetActive(false);
		m_explosionQueue.Enqueue(explosion);
	}
}
