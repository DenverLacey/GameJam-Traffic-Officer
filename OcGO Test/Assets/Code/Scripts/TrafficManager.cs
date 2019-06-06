using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    // Public:

    public GameObject carPrefab;

    public Transform[] m_laneSpawnPoints;
    public Vector3[] m_laneDirections;

    [Tooltip("The maximum amount of cars on the road at any given time.")]
    public int m_carCap = 25;

    [Tooltip("The minimum delay before spawning the next car.")]
    public float m_minSpawnDelay = 1.0f;

    [Tooltip("The maximum delay before spawning the next car.")]
    public float m_maxSpawnDelay = 5.0f;

    [Tooltip("Rate in which spawn delay decreases after each car spawn")]
    public float m_spawnDelayDecrement = 0.2f;

    [Tooltip("Initial movement speed of all cars.")]
    public float m_carSpeed = 3.0f;

    [Tooltip("Rate in which traffic speed increases when cars spawn.")]
    public float m_carSpeedIncrease = 0.3f;
    public float m_speedLimit = 40.0f;

    [Tooltip("Distance between cars when they are stopped and stacked.")]
    public float m_carStopGap = 7.0f;

    // Private:

    private Queue<GameObject> m_inactiveCars;
    private List<GameObject> m_lane1Cars;
    private List<GameObject> m_lane2Cars;
    private List<GameObject> m_lane3Cars;
    private List<GameObject> m_lane4Cars;

    private float[] m_laneStopDists; // Offsets from lane spawnpoints where the front-most stopped car is.
    private int[] m_laneStopIndices; // Index of the front-most stopped car.

    private List<GameObject>[] m_laneList;

    private float m_currentSpawnDelay;

    private int m_stopIndex = 0;

    void Awake()
    {
        m_inactiveCars = new Queue<GameObject>();
        m_lane1Cars = new List<GameObject>();
        m_lane2Cars = new List<GameObject>();
        m_lane3Cars = new List<GameObject>();
        m_lane4Cars = new List<GameObject>();

        m_laneList = new List<GameObject>[4];

        m_laneList[0] = m_lane1Cars;
        m_laneList[1] = m_lane2Cars;
        m_laneList[2] = m_lane3Cars;
        m_laneList[3] = m_lane4Cars;

        m_laneStopDists = new float[4];
        m_laneStopIndices = new int[4];

        m_laneStopIndices[0] = int.MaxValue;
        m_laneStopIndices[1] = int.MaxValue;
        m_laneStopIndices[2] = int.MaxValue;
        m_laneStopIndices[3] = int.MaxValue;

        // Create car pool.
        for (int i = 0; i < m_carCap; ++i)
        {
            GameObject carInstance = Instantiate(carPrefab, transform);
            carInstance.AddComponent<CarData>();

            carInstance.SetActive(false);

            m_inactiveCars.Enqueue(carInstance);
        }

        m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_maxSpawnDelay);
    }

    void UpdateCar(GameObject car, int carIndex, int laneIndex)
    {
        Transform carTransform = car.transform;

        // Offset from lane spawn.
        float laneOffset = (m_laneSpawnPoints[laneIndex].position - carTransform.position).magnitude;

        // Index in the stopped car stack.
        int stackPos = (m_laneStopIndices[laneIndex] + 1) - (carIndex + 1);

        // Offset from original stopping point this specific car will stop at.
        float stopOffset = m_laneStopDists[laneIndex] + (stackPos * m_carStopGap);

        // Stops if any car in front of it is stopped and it has reached it's designated stopping point.
        if (carIndex >= m_laneStopIndices[laneIndex] && laneOffset >= stopOffset)
            return;

        // Translate...
        carTransform.Translate(carTransform.forward * m_carSpeed * Time.deltaTime, Space.World);

        if (laneOffset > 100 * 100)
        {
            car.SetActive(false);

            m_inactiveCars.Enqueue(car);
            m_laneList[laneIndex].RemoveAt(carIndex);
        }
    }

    /*
    Description: Stop the provided car. Will also stop all other cars behind it.
    */
    public void StopCar(GameObject car)
    {
        CarData data = car.GetComponent<CarData>();

        // Dont change the stop index if a car ahead has stopped.
        if (data.IndexInLane >= m_laneStopIndices[data.LaneIndex])
            return;

        m_laneStopIndices[data.LaneIndex] = data.IndexInLane;

        GameObject carToStop = m_laneList[m_stopIndex][data.IndexInLane];

        m_laneStopDists[m_stopIndex] = (m_laneSpawnPoints[m_stopIndex].position - carToStop.transform.position).magnitude;
    }

    /*
    Description: Stop the car at the provided indices. Will also stop other cars behind it.
    */
    public void StopCar(int carIndex, int laneIndex)
    {
        // Dont change the stop index if a car ahead has stopped.
        if (carIndex >= m_laneStopIndices[laneIndex])
            return;

        m_laneStopIndices[laneIndex] = carIndex;

        GameObject carToStop = m_laneList[laneIndex][carIndex];

        m_laneStopDists[m_stopIndex] = (m_laneSpawnPoints[m_stopIndex].position - carToStop.transform.position).magnitude;
    }

    /*
    Description: Car provided will resume it's route if it is the front car stopped in the lane.
    */
    public void ResumeCar(GameObject car)
    {
        CarData data = car.GetComponent<CarData>();

        // Resume car only if it is the front-most stopped car.
        if (data.IndexInLane == m_laneStopIndices[data.LaneIndex])
            m_laneStopIndices[data.LaneIndex] = int.MaxValue;
    }

    void Update()
    {
        // Spawning
        m_currentSpawnDelay -= Time.deltaTime;

        if(m_currentSpawnDelay <= 0.0f && m_inactiveCars.Count > 0)
        {
            // Spawn car in a lane.
            int spawnIndex = Random.Range(0, 4);

            Vector3 spawnPos = m_laneSpawnPoints[spawnIndex].position;
            Quaternion spawnRot = m_laneSpawnPoints[spawnIndex].rotation;

            GameObject carToSpawn = m_inactiveCars.Dequeue();

            // Move to spawn point.
            carToSpawn.transform.position = spawnPos;
            carToSpawn.transform.rotation = spawnRot;

            // Set active.
            carToSpawn.SetActive(true);

            // Increase global car speed.
            m_carSpeed += m_carSpeedIncrease;

            if (m_carSpeed > m_speedLimit)
                m_carSpeed = m_speedLimit;

            m_maxSpawnDelay -= m_spawnDelayDecrement;

            if (m_maxSpawnDelay < m_minSpawnDelay)
                m_maxSpawnDelay = m_minSpawnDelay;

            CarData data = carToSpawn.GetComponent<CarData>();

            data.LaneIndex = spawnIndex;
            data.IndexInLane = m_laneList[spawnIndex].Count;

            // Add to lane.
            m_laneList[spawnIndex].Add(carToSpawn);

            m_currentSpawnDelay = Random.Range(m_minSpawnDelay, m_maxSpawnDelay);
        }

        // Update all lanes.
        for (int i = 0; i < m_lane1Cars.Count; ++i)
            UpdateCar(m_lane1Cars[i], i, 0);

        for (int i = 0; i < m_lane2Cars.Count; ++i)
            UpdateCar(m_lane2Cars[i], i, 1);

        for (int i = 0; i < m_lane3Cars.Count; ++i)
            UpdateCar(m_lane3Cars[i], i, 2);

        for (int i = 0; i < m_lane4Cars.Count; ++i)
            UpdateCar(m_lane4Cars[i], i, 3);
    }
}
