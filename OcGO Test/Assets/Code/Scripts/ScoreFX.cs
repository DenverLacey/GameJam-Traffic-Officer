using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]

public class ScoreFX : MonoBehaviour
{
	[Tooltip("How long the bonus text affect will last")]
	[SerializeField] private float m_duration;

	[Tooltip("The Y value that the affect will rise to")]
	[SerializeField] private float m_endY;

	[Tooltip("The scale that the affect will expand to")]
	[SerializeField] private Vector3 m_endScale;

	public float Duration { get => m_duration; set => m_duration = value; }
	public float EndY { get => m_endY; set => m_endY = value; }
	public Vector3 EndScale { get => m_endScale; set => m_endScale = value; }

	private float m_timer;

	private TextMesh m_text;

	private void Awake() {
		m_timer = 0.0f;
        m_text = GetComponent<TextMesh>();
		m_text.text = "+" + MainMenu.ScoreIncrement;
	}

	private void OnEnable() {
		transform.localScale = new Vector3(0, 0, 0);
		m_timer = 0.0f;
	}

	private void Update() {
		m_timer += Time.deltaTime;
		if (m_timer <= Duration * 0.75f) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, EndY, transform.position.z), .03f);
			transform.localScale = Vector3.Lerp(transform.localScale, EndScale, .03f);
		}
		else {
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, .07f);
		}
	}

	public void SetText(string text) {
		m_text.text = text;
	}
}
