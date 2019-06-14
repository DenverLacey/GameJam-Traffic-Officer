using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Traffic Lights")]
    [SerializeField] private GameObject m_greenLight;
    [SerializeField] private GameObject m_redLight;

    [Header("Text Objects")]
    [SerializeField] private TextMesh[] m_textObjects;

    [Tooltip("The amount of score gained when a car has made it through the intersection.")]
    [SerializeField] private int m_scorePassIncrement;

    [Tooltip("Amount of bonus lost when a car crashes.")]
    [SerializeField] private int m_bonusDecrement;

    [Tooltip("Amount of time before a wave ends.")]
    [SerializeField] private float m_waveDuration;

    private Material m_greenMat;
    private Material m_redMat;

    private CarManager m_manager;

    private static int m_bonus;

    private float m_waveTime;

    private bool m_greenEmissions;
    private bool m_redEmissions;

    public static int Score { get; set; }
    public static int Bonus
    {
        get { return m_bonus; }
        set {
            m_bonus = Mathf.Clamp(m_bonus, 0, int.MaxValue);
        }
    }

    public static int ScoreIncrement { get; private set; }
    public static int BonusDecrement { get; private set; }
    public static int CurrentWave { get; set; }


	/// <summary>
	/// Gets reference to green light and red light materials.
	/// </summary>
	private void Awake() {
		m_greenMat = m_greenLight.GetComponent<MeshRenderer>().material;
		m_redMat = m_redLight.GetComponent<MeshRenderer>().material;

        ScoreIncrement = m_scorePassIncrement;
        BonusDecrement = m_bonusDecrement;

        m_manager = FindObjectOfType<CarManager>();
	}

    private void Update()
    {
        if (!m_manager.GameRunning)
            return;

        m_waveTime -= Time.deltaTime;

        if(m_waveTime <= 0.0f)
        {
            ++CurrentWave;
            m_waveTime = m_waveDuration;

            // Reset car manager.
            m_manager.Reset();
            

        }
    }

	/// <summary>
	/// Is called when green light is pressed
	/// </summary>
	public void OnGreenPressed() {
		// start game
		m_manager.GameRunning = true;

        // Set wave to zero.
        CurrentWave = 0;
        m_waveTime = m_waveDuration;

        // deactivate text mesh
        m_textObjects[1].gameObject.SetActive(false);
	}

	/// <summary>
	/// Is called when red light is called
	/// </summary>
	public void OnRedPressed() {
		Application.Quit();
	}
}
