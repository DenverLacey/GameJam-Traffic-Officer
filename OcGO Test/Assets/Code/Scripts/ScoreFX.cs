using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]

public class ScoreFX : MonoBehaviour
{
	public float EndY { get; set; }
	private Vector3 m_scale;

	private float m_timer;

	private void Awake() {
		EndY = 2f;
		m_timer = 0.0f;
		m_scale = new Vector3(.1f, .1f, .1f);

        GetComponent<TextMesh>().text = "+" + MainMenu.ScoreIncrement;
	}

	private void OnEnable() {
		transform.localScale = new Vector3(0, 0, 0);
	}

	private void OnDisable() {
		m_timer = 0.0f;
	}

	private void Update() {
		m_timer += Time.deltaTime;
		if (m_timer <= 0.7f) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, EndY, transform.position.z), .03f);
			transform.localScale = Vector3.Lerp(transform.localScale, m_scale, .03f);
		}
		else {
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, .07f);
		}
	}
}
