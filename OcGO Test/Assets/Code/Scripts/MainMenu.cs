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

	public static int Score { get; set; }
	public static int Bonus {
		get { return Bonus; }
		set {
			Bonus = Mathf.Clamp(value, 0, int.MaxValue);
		}
	}

	/// <summary>
	/// Gets reference to green light and red light materials.
	/// </summary>
	private void Start() {
		m_greenMat = m_greenLight.GetComponent<MeshRenderer>().material;
		m_redMat = m_redLight.GetComponent<MeshRenderer>().material;
	}

	/// <summary>
	/// Turns green light on and off
	/// </summary>
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

	/// <summary>
	/// Turns red light on and off
	/// </summary>
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

	/// <summary>
	/// Is called when green light is pressed
	/// </summary>
	public void OnGreenPressed() {
		// start game
		FindObjectOfType<CarManager>().GameRunning = true;

		// deactivate text mesh
		m_textObjects[1].gameObject.SetActive(false);
	}

	/// <summary>
	/// Is called when red light is called
	/// </summary>
	public void OnRedPressed() {
		Application.Quit();
	}
}
