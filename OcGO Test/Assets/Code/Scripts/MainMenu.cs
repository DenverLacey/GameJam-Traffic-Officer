using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Traffic Lights")]
    [SerializeField] private GameObject m_greenLight = null;
    [SerializeField] private GameObject m_redLight = null;

    [Header("Text Objects")]
    [SerializeField] private TextMesh[] m_textObjects = null;

    [Tooltip("The amount of score gained when a car has made it through the intersection.")]
    [SerializeField] private int m_scorePassIncrement = 50;

    [Tooltip("Initital bonus value.")]
    [SerializeField] private int m_initialBonusVal = 1000;

    [Tooltip("Amount of time before a wave ends.")]
    [SerializeField] private float m_waveDuration = 60.0f;

    [Tooltip("Amount of time before a new wave starts after another ends.")]
    [SerializeField] private float m_intermissionDuration = 5.0f;

    private Material m_greenMat;
    private Material m_redMat;

    private CarManager m_manager;

    private static int m_bonus;

    private float m_waveTime;
    private float m_intermissionTime;

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
    public static int CurrentWave { get; set; }

    public enum EGameState
    {
        STATE_MAIN_MENU,
        STATE_PLAYING,
        STATE_INTERMISSION,
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
        Bonus = m_initialBonusVal;

        m_stateUpdateFuncs = new Dictionary<EGameState, StateFunction>();
        m_stateInitFuncs = new Dictionary<EGameState, StateFunction>();
        m_stateExitFuncs = new Dictionary<EGameState, StateFunction>();

        m_stateUpdateFuncs[EGameState.STATE_MAIN_MENU] = MainMenuUpdate;
        m_stateUpdateFuncs[EGameState.STATE_PLAYING] = GameStateUpdate;
        m_stateUpdateFuncs[EGameState.STATE_INTERMISSION] = IntermissionUpdate;
        m_stateUpdateFuncs[EGameState.STATE_GAME_OVER] = GameOverMenuUpdate;

        m_stateInitFuncs[EGameState.STATE_MAIN_MENU] = MainMenuInit;
        m_stateInitFuncs[EGameState.STATE_PLAYING] = GameStateInit;
        m_stateInitFuncs[EGameState.STATE_INTERMISSION] = IntermissionInit;
        m_stateInitFuncs[EGameState.STATE_GAME_OVER] = GameOverMenuInit;

        m_stateExitFuncs[EGameState.STATE_MAIN_MENU] = MainMenuExit;
        m_stateExitFuncs[EGameState.STATE_PLAYING] = GameStateExit;
        m_stateExitFuncs[EGameState.STATE_INTERMISSION] = IntermissionExit;
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

        if (m_waveTime <= 0.0f || Input.GetKeyDown(KeyCode.Return))
        {
            ++CurrentWave;
            m_waveTime = m_waveDuration;

            if(CurrentWave > 2)
            {
                GameState = EGameState.STATE_GAME_OVER;
                return;
            }

            // Enter intermission between waves.
            GameState = EGameState.STATE_INTERMISSION;
            return;
        }

		// update UI elements
		m_textObjects[0].text = "Wave " + (CurrentWave + 1).ToString() + ": " + m_waveTime.ToString("00.0");
		m_textObjects[1].text = "Score: " + Score.ToString();
	}

    private void GameStateExit()
    {
        m_manager.Reset(true);
        m_manager.ManagerRunning = false;
    }

    private void IntermissionInit()
    {
        m_textObjects[0].text = "Next wave in: " + m_intermissionDuration + " seconds!";
        m_textObjects[1].text = "Score: " + Score.ToString();

        m_intermissionTime = m_intermissionDuration;
    }

    private void IntermissionUpdate()
    {
        m_intermissionTime -= Time.deltaTime;

        m_textObjects[0].text = "Next wave in: " + m_intermissionTime + " seconds!";

        if (m_intermissionTime <= 0.0f)
        {
            // Set state without the property, since we don't want to reset the game state.
            m_state = EGameState.STATE_PLAYING;
            IntermissionExit();
        }
    }

    private void IntermissionExit()
    {
        Bonus = m_initialBonusVal;
        m_manager.ManagerRunning = true;
    }

    private void GameOverMenuInit()
    {
		m_textObjects[0].text = "Game Over!";
        m_textObjects[1].text = "Score: " + Score;
    }

    private void GameOverMenuUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameState = EGameState.STATE_MAIN_MENU;
        }
    }

    private void GameOverExit()
    {
		
    }

    private void MainMenuInit()
    {
		m_textObjects[0].text = "Quit";
		m_textObjects[1].text = "Start";
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

	/// <summary>
	/// Is called when light 0 is called
	/// </summary>
	public void OnLight0Pressed() {
		if (GameState != EGameState.STATE_PLAYING) {
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
