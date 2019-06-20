using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
	[System.Serializable]
    public struct Lane
    {
		public Transform start;
		public Transform end;
		public Transform crossing;
        [HideInInspector] public int carCount;
    }

	[Header("Manager stuff")]
	[Tooltip("Minimum spawn delay")]
	[SerializeField] private float m_minSpawnDelay = 1.0f;

	[Tooltip("Maximum spawn delay")]
	[SerializeField] private float m_maxSpawnDelay = 5.0f;

    [Tooltip("Amount the max spawn delay is decremented each spawn.")]
    [SerializeField] private float m_spawnDelayDecrement = 0.1f;

    [Tooltip("Amount spawn delay is reduced each wave.")]
    [SerializeField] private float m_waveSpawnDelayDecrement = 1.0f;

    [Tooltip("How close the car has to be to the end of the lane to despawn")]
	[SerializeField] private float m_despawnDistance = 0.1f;

    [Tooltip("Chance for two cars to spawn in one go, setting up a collision.")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float m_doubleSpawnChance = 40.0f;

    [Header("Car Stuff")]
    [Tooltip("All Car prefabs.")]
    [SerializeField] private GameObject[] m_carPrefabs = null;

    [Tooltip("Speed of the cars.")]
    [SerializeField] private float m_carSpeed = 1.0f;

    [Tooltip("By how much the car's speed increases")]
    [SerializeField] private float m_carSpeedIncrease = 0.1f;

    [Tooltip("By how much the car's base speed increases each wave")]
    [SerializeField] private float m_waveCarSpeedIncrease = 2.0f;

    [Tooltip("How far away the cars will stop from another car")]
    [SerializeField] private float m_carStopDistance = 3.0f;

    [Tooltip("How long the cars will stay stopped")]
    [SerializeField] private float m_carStopTime = 3.0f;

	[Tooltip("Car Score effect prefab")]
	[SerializeField] GameObject m_scorePrefab = null;

    [Tooltip("Car explosion effect prefab")]
    [SerializeField] GameObject m_explosionPrefab = null;

    [Header("Lane Stuff")]
    [Tooltip("How many cars per lane object pool")]
    [SerializeField] private int m_carsPerLane = 3;

    [Tooltip("Lane info")]
    [SerializeField] private Lane[] m_lanes = null;
	public Lane[] Lanes { get => m_lanes; }

    Queue<CarActor> m_inactiveCars;
	List<CarActor> m_activeCars;

	Queue<GameObject> m_inactiveScoreAffects;
	List<GameObject> m_activeScoreAffects;

    private float m_currentCarSpeed;
	private float m_currentSpawnDelay;
    private float m_currentMaxSpawnDelay;
	private float m_spawnTimer;

	public bool ManagerRunning { get; set; }

    private List<int> m_freeLanes = new List<int>();

    /// <summary>
	/// Creates object pool for all cars, initialises all variables
	/// </summary>
    void Start()
    {
        // create object pool for all needed cars
        m_inactiveCars = new Queue<CarActor>();
        for (int lane = 0; lane < m_lanes.Length; lane++)
        {
            m_freeLanes.Add(lane);
            for (int c = 0; c < m_carsPerLane; c++)
            {
                // create new car object and deactivate it
                GameObject cobj = Instantiate(m_carPrefabs[Random.Range(0, m_carPrefabs.Length)], transform.position, Quaternion.identity);
                cobj.SetActive(false);

                m_currentCarSpeed = m_carSpeed;

                // set up car's CarActor component
                CarActor car = cobj.GetComponent<CarActor>();
                car.Speed = m_carSpeed;
                car.Lane = -1;
                car.StopDistance = m_carStopDistance;
                car.StopTime = m_carStopTime;
                car.SetExplosionEffect(Instantiate(m_explosionPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>());

                // add car to pool
                m_inactiveCars.Enqueue(car);
            }
        }
		m_activeCars = new List<CarActor>();

		// create pool for all needed score affects
		m_inactiveScoreAffects = new Queue<GameObject>();
		for (int a = 0; a < m_lanes.Length * m_carsPerLane * 1.5f; a++) {
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
            // int randLane = Random.Range(0, m_lanes.Length);

			if (m_freeLanes.Count > 0 &&
				m_inactiveCars.Count > 0) 
			{
				int randLane = m_freeLanes[Random.Range(0, m_freeLanes.Count)];

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
                m_activeCars[i].Explode();
                DeactivateCar(m_activeCars[i]);
            }
			else
            {
				// calculate how close the car is to crossing
				float carDist = (m_activeCars[i].transform.position - m_lanes[m_activeCars[i].Lane].start.position).sqrMagnitude;
				float laneDist = (m_lanes[m_activeCars[i].Lane].crossing.position - m_lanes[m_activeCars[i].Lane].start.position).sqrMagnitude;

				// if car has made it to crossing
				if (carDist >= laneDist && !m_activeCars[i].HasCrossed) 
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

		for (int i = 0; i < m_lanes.Length; i++) {
			m_lanes[i].carCount = 0;
			if (!m_freeLanes.Contains(i)) {
				m_freeLanes.Add(i);
			}
		}

		if (addScores) {
			MainMenu.Score += m_activeCars.Count * MainMenu.ScoreIncrement;
		}

        m_activeCars.Clear();

        // Reset speed and spawn delay.
        m_currentCarSpeed = m_carSpeed + (m_waveCarSpeedIncrease * MainMenu.CurrentWave);
        m_currentMaxSpawnDelay = m_maxSpawnDelay - (m_waveSpawnDelayDecrement * MainMenu.CurrentWave);
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
		m_lanes[laneIndex].carCount++;

		// remove full lane from free lanes
		if (m_lanes[laneIndex].carCount >= m_carsPerLane) {
			m_freeLanes.Remove(laneIndex);
		}

		// orient car for journey
		car.transform.position = m_lanes[laneIndex].start.position;
		Vector3 dir = (m_lanes[laneIndex].end.position - m_lanes[laneIndex].start.position).normalized;
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
        //decrement car count for car's lane
        m_lanes[car.Lane].carCount--;
		m_lanes[car.Lane].carCount = Mathf.Clamp(m_lanes[car.Lane].carCount, 0, int.MaxValue);

		// add available lane to free lanes
		if (m_lanes[car.Lane].carCount < m_carsPerLane &&
			!m_freeLanes.Contains(car.Lane)) 
		{
			m_freeLanes.Add(car.Lane);
		}

		// remove car from active list
		m_activeCars.Remove(car);

		// deactivate car
		car.gameObject.SetActive(false);

		// move to inactive queue
		m_inactiveCars.Enqueue(car);
	}

	/// <summary>
	/// Activates a score affect for a car that has crossed intersection.
	/// </summary>
	/// <param name="car">
	/// Reference to which car has crossed.
	/// </param>
	void ActivateScoreAffect(CarActor car) {
		GameObject obj = m_inactiveScoreAffects.Dequeue();
		obj.SetActive(true);
		obj.transform.position = car.transform.position;
		obj.transform.rotation = Quaternion.LookRotation(car.transform.position.normalized);

		ScoreFX aff = obj.GetComponent<ScoreFX>();
		aff.EndY = car.transform.position.y + 1.0f;
		aff.EndScale = new Vector3(.1f, .1f, .1f);
		aff.Duration = 1.1f;

		StartCoroutine(DeactivateScoreAffect(obj, aff.Duration));
	}

	/// <summary>
	/// Deactivates a given score affect GameObject after a given amount of delay.
	/// </summary>
	/// <param name="obj">
	/// What object to deactivate.
	/// </param>
	/// <param name="delay">'
	/// The amount of delay, in seconds.
	/// </param>
	/// <returns>
	/// Returns an IEnumerator.
	/// </returns>
	IEnumerator DeactivateScoreAffect(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		obj.SetActive(false);
		obj.transform.parent = null;
		m_activeScoreAffects.Remove(obj);
		m_inactiveScoreAffects.Enqueue(obj);
	}
}
