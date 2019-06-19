using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarActor : MonoBehaviour
{
    public float Speed { get; set; }
    public int Lane { get; set; }
    public float StopDistance { get; set; }
    public float StopTime { get; set; }
	public bool HasCrashed { get; private set; }
	public bool HasCrossed { get; set; }

    private ParticleSystem m_explosionEffect;

    private float m_stopTimer;
    private bool m_stopped;
	private bool m_waiting;

	private void OnEnable() {
		HasCrashed = false;
		HasCrossed = false;
	}

	/// <summary>
	/// Determines if car should move or not. Moves car if it should
	/// </summary>
	private void Update()
    {
        // if car is stopped, start stop timer
        if (m_stopped)
        {
            m_stopTimer += Time.deltaTime;
            if (m_stopTimer >= StopTime)
            {
                m_stopped = false;
                m_stopTimer = 0.0f;
            }
        }

		if (!m_stopped && !m_waiting) {
			transform.Translate(Vector3.forward * Speed * Time.deltaTime);
		}

        // stop car if about to collide with car in same lane
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, StopDistance))
        {
            if (hit.collider.tag == "Car")
            {
                CarActor other = hit.collider.GetComponent<CarActor>();
                if (other.Lane == Lane)
                    m_waiting = true;
            }
        }
        else
        {
            m_waiting = false;
        }
    }

    /// <summary>
    /// Set explosion effect particlesystem instance.
    /// </summary>
    /// <param name="effect"></param>
    public void SetExplosionEffect(ParticleSystem effect)
    {
        m_explosionEffect = effect;
    }

    /// <summary>
    /// Play the explosion effect.
    /// </summary>
    public void Explode()
    {
        m_explosionEffect.gameObject.SetActive(true);
        m_explosionEffect.transform.position = transform.position;
        m_explosionEffect.Play();
    }

    /// <summary>
    /// Tells car to stop
    /// </summary>
    public void Stop()
    {
        m_stopped = true;
        m_stopTimer = 0.0f;
    }

    /// <summary>
    /// Tells car it can resume
    /// </summary>
    public void Resume()
    {
        m_stopped = false;
    }

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Car") {
			CarActor car = other.GetComponent<CarActor>();
			if (car.Lane != Lane) {
				// collision occured
				car.HasCrashed = true;
				HasCrashed = true;
			}
		}
	}
}
