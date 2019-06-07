using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class TrafficLight : MonoBehaviour {

	[Tooltip("What happens when traffic light is clicked")]
	[SerializeField] UnityEvent m_onClicked;

	private MainMenu m_mainMenu;
	private Material m_mat;
	private bool m_pointedAt;

    /// <summary>
	/// Finds reference to main menu object and
	/// gets objects material.
	/// </summary>
    void Start() {
		m_mainMenu = FindObjectOfType<MainMenu>();
		m_mat = GetComponent<MeshRenderer>().material;
    }

    /// <summary>
	/// Turns emission on and off based on m_pointedAt.
	/// </summary>
    void LateUpdate() {
        if (m_pointedAt) {
			m_mat.EnableKeyword("_EMISSION");
		}
		else {
			m_mat.DisableKeyword("_EMISSION");
		}
		m_pointedAt = false;
    }

	/// <summary>
	/// Sets m_pointedAt to true.
	/// </summary>
	public void OnPointedAt() {
		m_pointedAt = true;
	}

	/// <summary>
	/// Invokes OnClicked event.
	/// </summary>
	public void OnClicked() {
		m_onClicked.Invoke();
	}
}
