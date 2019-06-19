using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerMenuInteraction))]
public class PlayerActor : MonoBehaviour {

	[Tooltip("Button to tell a car to stop")]
	[SerializeField] private OVRInput.Button m_pointerButton = OVRInput.Button.PrimaryIndexTrigger;

	[Tooltip("Button to tell a car to go")]
	[SerializeField] private OVRInput.Button m_goButton = OVRInput.Button.PrimaryTouchpad;

	[Tooltip("MAximum distance of pointer raycast")]
	private float m_pointerDistance = 100f;

	public float PointerDistance { get => m_pointerDistance; }
	public OVRInput.Button PointerButton { get => m_pointerButton; }

	/// <summary>
	/// Responds to player input and updates line renderer effect.
	/// </summary>
	void Update() {
		// Button inputs
		if (OVRInput.GetDown(m_pointerButton)) {
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_pointerDistance)) {
				hit.collider.GetComponent<CarActor>().ChangeCarState();
			}
		}
			
	}
}
