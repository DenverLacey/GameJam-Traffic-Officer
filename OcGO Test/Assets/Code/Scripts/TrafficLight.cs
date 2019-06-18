using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class TrafficLight : MonoBehaviour
{
	[System.Serializable]
	public struct LightMat
    {
		public Material onMat;
		public Material offMat;
	};

	[Tooltip("On and Off materials")]
	[SerializeField] private LightMat m_mats = new LightMat();

	[Tooltip("What happens when traffic light is clicked")]
	[SerializeField] UnityEvent m_onClicked = null;

	private MainMenu m_mainMenu;
	private bool m_pointedAt;

	private MeshRenderer m_renderer;

    /// <summary>
	/// Finds reference to main menu object and
	/// gets objects material.
	/// </summary>
    void Start() {
		m_mainMenu = FindObjectOfType<MainMenu>();
		m_renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
	/// Turns emission on and off based on m_pointedAt.
	/// </summary>
    void LateUpdate() {
        if (m_pointedAt) {
			m_renderer.material = m_mats.onMat;
		}
		else {
			m_renderer.material = m_mats.offMat;
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
