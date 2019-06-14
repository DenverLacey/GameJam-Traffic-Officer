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

    [Tooltip("Initital bonus value.")]
    [SerializeField] private int m_initialBonusVal;

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

    public enum EGameState
    {
        STATE_MAIN_MENU,
        STATE_PLAYING,
		STATE_PAUSED,
        STATE_GAME_OVER
    }

    delegate void StateFunction();

    private static Dictionary<EGameState, StateFunction> m_stateUpdateFuncs;
    private static Dictionary<EGameState, StateFunction> m_stateInitFuncs;
    private static Dictionary<EGameState, StateFunction> m_stateExitFuncs;

    private static EGameState m_state;

    public static EGameState GameState
    {
        get { return m_state; }
        set
        {
            m_stateExitFuncs[m_state]();
            m_state = value;
            m_stateInitFuncs[m_state]();
        }
    }

	/// <summary>
	/// Gets reference to green light and red light materials.
	/// </summary>
	private void Awake() {
		m_greenMat = m_greenLight.GetComponent<MeshRenderer>().material;
		m_redMat = m_redLight.GetComponent<MeshRenderer>().material;

        ScoreIncrement = m_scorePassIncrement;
        BonusDecrement = m_bonusDecrement;
        Bonus = m_initialBonusVal;

        m_stateUpdateFuncs = new Dictionary<EGameState, StateFunction>();
        m_stateInitFuncs = new Dictionary<EGameState, StateFunction>();
        m_stateExitFuncs = new Dictionary<EGameState, StateFunction>();

        m_stateUpdateFuncs[EGameState.STATE_MAIN_MENU] = MainMenuUpdate;
        m_stateUpdateFuncs[EGameState.STATE_PLAYING] = GameStateUpdate;
		m_stateUpdateFuncs[EGameState.STATE_PAUSED] = PausedUpdate;
        m_stateUpdateFuncs[EGameState.STATE_GAME_OVER] = GameOverMenuUpdate;

        m_stateInitFuncs[EGameState.STATE_MAIN_MENU] = MainMenuInit;
        m_stateInitFuncs[EGameState.STATE_PLAYING] = GameStateInit;
		m_stateInitFuncs[EGameState.STATE_PAUSED] = PausedInit;
		m_stateInitFuncs[EGameState.STATE_GAME_OVER] = GameOverMenuInit;

        m_stateExitFuncs[EGameState.STATE_MAIN_MENU] = MainMenuExit;
        m_stateExitFuncs[EGameState.STATE_PLAYING] = GameStateExit;
		m_stateExitFuncs[EGameState.STATE_PAUSED] = PausedExit;
		m_stateExitFuncs[EGameState.STATE_GAME_OVER] = GameOverExit;

        GameState = EGameState.STATE_MAIN_MENU;

        m_manager = FindObjectOfType<CarManager>();
	}

    private void Update()
    {
        m_stateUpdateFuncs[m_state]();
    }

    private void GameStateInit()
    {
        m_manager.ManagerRunning = true;

        // Set wave to zero.
        CurrentWave = 0;
        m_waveTime = m_waveDuration;

        // Scoring
        Score = 0;
        Bonus = m_initialBonusVal;
    }

    private void GameStateUpdate()
    {
        m_waveTime -= Time.deltaTime;

        if (m_waveTime <= 0.0f)
        {
            ++CurrentWave;
            m_waveTime = m_waveDuration;

            // Add bonus to score.
            Score += Bonus;
            Bonus = m_initialBonusVal;

            if(CurrentWave > 2)
            {
                GameState = EGameState.STATE_GAME_OVER;
                return;
            }

            // Reset car manager.
            m_manager.Reset(true);

            Debug.Log("New wave!");
        }

		// update UI elements
		m_textObjects[0].text = "Wave " + (CurrentWave + 1).ToString() + ": " + m_waveTime.ToString("00.0");
		m_textObjects[1].text = Score.ToString();

		// poll input
		if (OVRInput.GetDown(OVRInput.Button.Back)) {
			m_state = EGameState.STATE_PAUSED;
			PausedInit();
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			m_state = EGameState.STATE_PAUSED;
			PausedInit();
		}
	}

    private void GameStateExit()
    {
        m_manager.Reset(true);
        m_manager.ManagerRunning = false;
    }

    private void GameOverMenuInit()
    {
        Debug.Log("Game over!");
    }

    private void GameOverMenuUpdate()
    {

    }

    private void GameOverExit()
    {

    }

    private void MainMenuInit()
    {

    }

    private void MainMenuUpdate()
    {
		if (Input.GetKeyDown(KeyCode.Return)) {
			GameState = EGameState.STATE_PLAYING;
		}
    }

    private void MainMenuExit()
    {

    }

	private void PausedInit() 
	{
		m_textObjects[0].text = "Quit";
		m_textObjects[1].text = "Resume";
		Time.timeScale = 0.0001f;
	}

	private void PausedUpdate() 
	{
		if (OVRInput.GetDown(OVRInput.Button.Back)) {
			m_state = EGameState.STATE_PLAYING;
			PausedExit();
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
			m_state = EGameState.STATE_PLAYING;
			PausedExit();
		}
	}

	private void PausedExit() 
	{
		Time.timeScale = 1.0f;
	}

	/// <summary>
	/// Is called when light 0 is called
	/// </summary>
	public void OnLight0Pressed() {
		if (GameState == EGameState.STATE_PAUSED || GameState == EGameState.STATE_MAIN_MENU) {
			Application.Quit();
		}
	}

	/// <summary>
	/// Is called when light 1 is pressed
	/// </summary>
	public void OnLight1Pressed() {
		if (GameState != EGameState.STATE_PLAYING) {
			// start game
			GameState = EGameState.STATE_PLAYING;
		}
	}
}
