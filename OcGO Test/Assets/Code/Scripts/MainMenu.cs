using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	[Header("Traffic Lights")]
	[SerializeField] private GameObject m_greenLight;
	[SerializeField] private GameObject m_redLight;

	[Header("Text Objects")]
	[SerializeField] private TextMesh[] m_textObjects;

	private Material m_greenMat;
	private Material m_redMat;

	private bool m_greenEmissions;
	private bool m_redEmissions;

	private void Start() {
		m_greenMat = m_greenLight.GetComponent<MeshRenderer>().material;
		m_redMat = m_redLight.GetComponent<MeshRenderer>().material;
	}

	public void ChangeGreenEmissionsState() {
		if (m_greenEmissions) {
			m_greenMat.DisableKeyword("_EMISSION");
			m_greenEmissions = false;
		}
		else {
			m_greenMat.EnableKeyword("_EMISSION");
			m_greenEmissions = true;
		}
	}

	public void ChangeRedEmissionsState() {
		if (m_greenEmissions) {
			m_redMat.DisableKeyword("_EMISSION");
			m_redEmissions = false;
		}
		else {
			m_redMat.EnableKeyword("_EMISSION");
			m_redEmissions = true;
		}
	}

	public void OnGreenPressed() {
		// start game
		Debug.Log("START GAME!");

		// destroy text objects
		foreach (var t in m_textObjects) {
			Destroy(t, Time.deltaTime);
		}
	}

	public void OnRedPressed() {
		Debug.Log("QUIT GAME!");
		Application.Quit();
	}
}
