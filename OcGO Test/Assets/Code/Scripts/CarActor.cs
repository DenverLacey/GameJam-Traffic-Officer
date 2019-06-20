using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
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

    private Collider m_collider;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }

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
        if (Physics.Raycast(transform.position + transform.forward + Vector3.up, transform.forward, out RaycastHit hit, m_collider.bounds.size.z + StopDistance))
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
	/// Tells car to stop or resume based on if it is stopped or not.
	/// </summary>
	public void ChangeCarState() {
		if (m_stopped) {
			m_stopped = false;
		}
		else {
			m_stopped = true;
			m_stopTimer = 0.0f;
		}
	}

	/// <summary>
	/// Handles car crashes.
	/// </summary>
	/// <param name="other">
	/// The collider of the object that has been collided with.
	/// </param>
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
