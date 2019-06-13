using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [System.Serializable]
    struct Lane
    {
        Vector3 m_startPos;
        Vector3 m_endPos;
    }

    [Header("Manager stuff")]
    // [Tooltip()]

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

    List<CarActor> m_carObjectPool;

    // Start is called before the first frame update
    void Start()
    {
        // create object pool for all needed cars
        m_carObjectPool = new List<CarActor>();
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
                car.Lane = lane;
                car.StopDistance = m_carStopDistance;
                car.StopTime = m_carStopTime;

                // add car to pool
                m_carObjectPool.Add(car);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
