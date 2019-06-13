using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [System.Serializable]
    struct Lane
    {
       public Transform start;
       public Transform end;
    }

	[Header("Manager stuff")]
	[Tooltip("Minimum spawn delay")]
	[SerializeField] private float m_minSpawnDelay;

	[Tooltip("Maximum spawn delay")]
	[SerializeField] private float m_maxSpawnDelay;

	[Tooltip("How close the car has to be to the end of the lane to despawn")]
	[SerializeField] private float m_despawnDistance;

    [Header("Car Stuff")]
    [Tooltip("The car prefab.")]
    [SerializeField] private GameObject m_carPrefab;

    [Tooltip("Speed of the cars.")]
    [SerializeField] private float m_carSpeed;

    [Tooltip("By how much the car's speed increases")]
    [SerializeField] private float m_carSpeedIncrease;

    [Tooltip("How far away the cars will stop from another car")]
    [SerializeField] private float m_carStopDistance;

    [Tooltip("How long the cars will stay stopped")]
    [SerializeField] private float m_carStopTime;

    [Header("Lane Stuff")]
    [Tooltip("How many cars per lane object pool")]
    [SerializeField] private int m_carsPerLane;

    [Tooltip("Lane info")]
    [SerializeField] private Lane[] m_lanes;

    Queue<CarActor> m_inactiveCars;
	List<CarActor> m_activeCars;

	private float m_currentSpawnDelay;
	private float m_spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        // create object pool for all needed cars
        m_inactiveCars = new Queue<CarActor>();
        for (int lane = 0; lane < m_lanes.Length; lane++)
        {
            for (int c = 0; c < m_carsPerLane; c++)
            {
                // create new car object and deactivate it
                GameObject cobj = Instantiate(m_carPrefab, transform.position, Quaternion.identity);
                cobj.SetActive(false);

                // set up car's CarActor component
                CarActor car = cobj.GetComponent<CarActor>();
                car.Speed = m_carSpeed;
                car.Lane = -1;
                car.StopDistance = m_carStopDistance;
                car.StopTime = m_carStopTime;

                // add car to pool
                m_inactiveCars.Enqueue(car);
            }
        }

		m_activeCars = new List<CarActor>();

		// set up spawn timers and delays
		m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_maxSpawnDelay);
		m_spawnTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
		// update timer
		m_spawnTimer += Time.deltaTime;

		if (m_spawnTimer >= m_currentSpawnDelay) {
			// activate car in random lane
			int randLane = Random.Range(0, m_lanes.Length);
			ActivateCarInLane(randLane);

			// reset timers and delays
			m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_maxSpawnDelay);
			m_spawnTimer = 0.0f;
		}

		// deactivate cars that have made it across intersection
		for (int i = 0; i < m_activeCars.Count;) {
			// if car has reached end of lane or has crashed
			if (Vector3.Distance(m_activeCars[i].transform.position, m_lanes[m_activeCars[i].Lane].end.position) <= m_despawnDistance || m_activeCars[i].HasCrashed) {
				DeactivateCar(m_activeCars[i]);
			}
			else {
				i++;
			}
		}
    }

	/// <summary>
	/// Activates car from object pool and aligns it with its lane.
	/// </summary>
	/// <param name="laneIndex">
	///	which lane the car
	/// </param>
	void ActivateCarInLane(int laneIndex) {
		if (m_inactiveCars.Count == 0)
			return;

		// take car out of inactive pool
		CarActor car = m_inactiveCars.Dequeue();
		
		// set cars lane
		car.Lane = laneIndex;

		// get lane from array of lanes
		Lane lane = m_lanes[laneIndex];

		// orient car for journey
		car.transform.position = lane.start.position;
		Vector3 dir = (lane.end.position - lane.start.position).normalized;
		car.transform.forward = dir;

		// active car
		car.gameObject.SetActive(true);
		m_activeCars.Add(car);
	}

	void DeactivateCar(CarActor car) {
		// remove car from active list
		m_activeCars.Remove(car);

		// deactivate car
		car.gameObject.SetActive(false);

		// move to inactive queue
		m_inactiveCars.Enqueue(car);
	}
}
