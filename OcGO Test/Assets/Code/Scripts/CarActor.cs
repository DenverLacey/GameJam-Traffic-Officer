using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarActor : MonoBehaviour
{
    public float Speed { get; set; }
    public int Lane { get; set; }
    public float StopDistance { get; set; }
    public float StopTime { get; set; }

    private float m_stopTimer;
    private bool m_stopped;

    /// <summary>
    /// Determines if car should move or not. Moves car if it should
    /// </summary>
    private void Update()
    {
        // stop car if about to collide with car in same lane
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, StopDistance))
        {
            if (hit.collider.tag == "Car")
            {
                CarActor other = hit.collider.GetComponent<CarActor>();
                if (other.Lane == Lane)
                    m_stopped = true;
            }
        }
        else
        {
            m_stopped = false;
        }

        // if car is stopped, don't move car
        if (m_stopped)
        {
            m_stopTimer += Time.deltaTime;
            if (m_stopTimer >= StopTime)
            {
                m_stopped = false;
                m_stopTimer = 0.0f;
            }
            return;
        }

        // else, move car
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
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
}
