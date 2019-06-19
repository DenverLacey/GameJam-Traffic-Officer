using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour
{
    public ParticleSystem m_explosionEffect;

    public int LaneIndex { get; set; }
    public int IndexInLane { get; set; }
	public float StopTime { get; set; }
}
