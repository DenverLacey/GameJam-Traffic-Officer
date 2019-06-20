using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FlickerLight : MonoBehaviour
{
	[Tooltip("How quick the lights flicker")]
	[SerializeField] private float m_flickerSpeed;

	[Tooltip("Material to use when light flickers on")]
	[SerializeField] private Material m_onMat;

	[Tooltip("Material to use when light flickers off")]
	[SerializeField] private Material m_offMat;

	private MeshRenderer m_renderer;
	private float m_timer;
	private bool m_on;

    // Start is called before the first frame update
    void Start()
    {
		m_renderer = GetComponent<MeshRenderer>();
		m_timer = 0.0f;
		m_on = false;

		m_renderer.materials[0] = m_offMat;
		m_renderer.materials[1] = m_offMat;
    }

    // Update is called once per frame
    void Update()
    {
		m_timer += Time.deltaTime;

		if (m_timer < m_flickerSpeed) return;

		if (m_on) {
			m_renderer.materials[0] = m_offMat;
			m_renderer.materials[1] = m_offMat;

			m_on = false;
			m_timer = 0.0f;
		}
		else {
			m_renderer.materials[0] = m_onMat;
			m_renderer.materials[1] = m_onMat;

			m_on = true;
			m_timer = 0.0f;
		}
    }
}
