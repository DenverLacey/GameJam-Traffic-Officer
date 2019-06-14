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
		public Transform crossing;
    }

	[Header("Manager stuff")]
	[Tooltip("Minimum spawn delay")]
	[SerializeField] private float m_minSpawnDelay;

	[Tooltip("Maximum spawn delay")]
	[SerializeField] private float m_maxSpawnDelay;

    [Tooltip("Amout the max spawn delay is decremented each spawn.")]
    [SerializeField] private float m_spawnDelayDecrement;

    [Tooltip("How close the car has to be to the end of the lane to despawn")]
	[SerializeField] private float m_despawnDistance;

    [Tooltip("Chance for two cars to spawn in one go, setting up a collision.")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float m_doubleSpawnChance;

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

	[Tooltip("Car Score effect prefab")]
	[SerializeField] GameObject m_scorePrefab;

    [Header("Lane Stuff")]
    [Tooltip("How many cars per lane object pool")]
    [SerializeField] private int m_carsPerLane;

    [Tooltip("Lane info")]
    [SerializeField] private Lane[] m_lanes;

    Queue<CarActor> m_inactiveCars;
	List<CarActor> m_activeCars;

	Queue<GameObject> m_inactiveScoreAffects;
	List<GameObject> m_activeScoreAffects;

    private float m_currentCarSpeed;
	private float m_currentSpawnDelay;
    private float m_currentMaxSpawnDelay;
	private float m_spawnTimer;

	public bool ManagerRunning { get; set; }

    /// <summary>
	/// Creates object pool for all cars, initialises all variables
	/// </summary>
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

                m_currentCarSpeed = m_carSpeed;

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

		// create pool for all needed score affects
		m_inactiveScoreAffects = new Queue<GameObject>();
		for (int a = 0; a < 8; a++) {
			GameObject affect = Instantiate(m_scorePrefab, transform.position, Quaternion.identity);
			affect.SetActive(false);
			m_inactiveScoreAffects.Enqueue(affect);
		}
		m_activeScoreAffects = new List<GameObject>();

		// set up spawn timers and delays
		m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_maxSpawnDelay);
        m_currentMaxSpawnDelay = m_maxSpawnDelay;
		m_spawnTimer = 0.0f;

        // Set up car speeds.
        m_currentCarSpeed = m_carSpeed;

		ManagerRunning = false;
    }

    /// <summary>
	/// Spwans cars, detects if car has made it the end of its lane
	/// </summary>
    void Update()
    {
		if (!ManagerRunning)
			return;

		// update timer
		m_spawnTimer += Time.deltaTime;

		if (m_spawnTimer >= m_currentSpawnDelay)
        {
            float doubleSpawnVal = Random.Range(0.0f, 100.0f);

            // Pick random lane.
            int randLane = Random.Range(0, m_lanes.Length);

            if (doubleSpawnVal < m_doubleSpawnChance) // Spawn two cars.
            {
                ActivateCarInLane(randLane);

                randLane += 2;

                randLane %= m_lanes.Length;

                ActivateCarInLane(randLane);
            }
            else // Spawn one car.
            {
                ActivateCarInLane(randLane);
            }

            // Set new spawn speed.
            m_currentCarSpeed += m_carSpeedIncrease;

            // Set new max spawn delay.
            m_currentMaxSpawnDelay = Mathf.Clamp(m_currentMaxSpawnDelay - m_spawnDelayDecrement, m_minSpawnDelay, m_maxSpawnDelay);

            // reset timers and delays
            m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_currentMaxSpawnDelay);
			m_spawnTimer = 0.0f;
		}

		// deactivate cars that have made it across intersection
		for (int i = 0; i < m_activeCars.Count;)
        {
            if ((m_lanes[m_activeCars[i].Lane].end.position - m_activeCars[i].transform.position).sqrMagnitude <= m_despawnDistance * m_despawnDistance)
            {
                // Car has reached the end of the lane.
				DeactivateCar(m_activeCars[i]);
			}
            else if(m_activeCars[i].HasCrashed)
            {
                // Car has crashed.
                DeactivateCar(m_activeCars[i]);
                MainMenu.Bonus -= MainMenu.BonusDecrement; // Take from bonus.
            }
			else
            {
				// if car has made it to crossing
				if ((m_lanes[m_activeCars[i].Lane].crossing.position - m_activeCars[i].transform.position).sqrMagnitude <= m_despawnDistance * m_despawnDistance &&
					!m_activeCars[i].HasCrossed) 
				{
					MainMenu.Score += MainMenu.ScoreIncrement;
					m_activeCars[i].HasCrossed = true;
					ActivateScoreAffect(m_activeCars[i]);
				}
				i++;
			}
		}
    }

    /// <summary>
    /// Resets all cars managed by this car manager.
    /// </summary>
    /// <param name="addScores"></param>
    public void Reset(bool addScores = false)
    {
        foreach (CarActor car in m_activeCars)
        {
            m_inactiveCars.Enqueue(car);
            car.gameObject.SetActive(false);
			ActivateScoreAffect(car);
        }

        if (addScores)
            MainMenu.Score += m_activeCars.Count * MainMenu.ScoreIncrement;

        m_activeCars.Clear();

        // Reset speed and spawn delay.
        m_currentCarSpeed = m_carSpeed;
        m_currentMaxSpawnDelay = m_maxSpawnDelay;
    }

	/// <summary>
	/// Activates car from object pool and aligns it with its lane.
	/// </summary>
	/// <param name="laneIndex">
	///	which lane the car
	/// </param>
	void ActivateCarInLane(int laneIndex)
    {
		if (m_inactiveCars.Count == 0)
			return;

		// take car out of inactive pool
		CarActor car = m_inactiveCars.Dequeue();
		
		// set cars lane
		car.Lane = laneIndex;

        // Set car speed.
        car.Speed = m_currentCarSpeed;

		// get lane from array of lanes
		Lane lane = m_lanes[laneIndex];

		// orient car for journey
		car.transform.position = lane.start.position;
		Vector3 dir = (lane.end.position - lane.start.position).normalized;
		car.transform.forward = dir;

		// activate car
		car.gameObject.SetActive(true);
		m_activeCars.Add(car);
	}

	/// <summary>
	/// Deactives car object and moves it to queue of inactive cars
	/// </summary>
	/// <param name="car">
	/// The car that is being deactivated
	/// </param>
	void DeactivateCar(CarActor car)
    {
		// remove car from active list
		m_activeCars.Remove(car);

		// deactivate car
		car.gameObject.SetActive(false);

		// move to inactive queue
		m_inactiveCars.Enqueue(car);
	}

	void ActivateScoreAffect(CarActor car) {
		GameObject affect = m_inactiveScoreAffects.Dequeue();
		affect.SetActive(true);
		affect.transform.position = car.transform.position;
		affect.transform.rotation = Quaternion.LookRotation(car.transform.position.normalized);
		StartCoroutine(DeactivateScoreAffect(affect, 1.1f));
	}

	IEnumerator DeactivateScoreAffect(GameObject obj, float seconds) {
		yield return new WaitForSeconds(seconds);
		obj.SetActive(false);
		obj.transform.parent = null;
		m_activeScoreAffects.Remove(obj);
		m_inactiveScoreAffects.Enqueue(obj);
	}
}
