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

    // Start is called before the first frame update
    void Start() {
		m_mainMenu = FindObjectOfType<MainMenu>();
		m_mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void LateUpdate() {
        if (m_pointedAt) {
			m_mat.EnableKeyword("_EMISSION");
		}
		else {
			m_mat.DisableKeyword("_EMISSION");
		}
		m_pointedAt = false;
    }

	public void OnPointedAt() {
		m_pointedAt = true;
	}

	public void OnClicked() {
		m_onClicked.Invoke();
	}
}
