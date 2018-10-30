using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour 
{
    public static GameManager instance = null;

    public enum GameState { MENU, COUNT_DOWN, PAUSE, GAME, LEVEL, GAME_OVER, HIGHSCORES };

    private const float GAME_START_TIME = 3.0f;
	private const int LIVES_MAX = 3;
	
    private bool _waitForGameToStart = false;

	private int _score = 0;
	private int _lives = 0;
	private int _level = 1;

	private float _elapsedSecondsFromLevelStart;

	[SerializeField] Player _player;
	[SerializeField] private EnemiesController _enemiesController;

    public int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
        }
    }

    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        ResetGame();
    }

	void Update()
	{
		if (_waitForGameToStart)
		{
            UpdateCountDown();
        }
		else if (Input.GetKeyDown (KeyCode.Escape) && UIManager.instance.GameScreen.activeSelf && !_waitForGameToStart)
		{
			Config.isGamePaused = !Config.isGamePaused;

            if (Config.isGamePaused)
            {
                SetGameState(GameState.PAUSE);
            }
            else
            {
                SetGameState(GameState.GAME);
            }
            _enemiesController.PauseEnemies (Config.isGamePaused);
		}
	}

	public void SetGameState (GameState state)
	{
        UIManager.instance.DeactivateAllScreens();

		Config.isGamePaused = true;

		switch (state)
		{
			case GameState.MENU:
                UIManager.instance.ActivateScreen(UIManager.Screen.START);
                break;
			case GameState.COUNT_DOWN:
                UIManager.instance.ActivateScreen(UIManager.Screen.GAME);
                UIManager.instance.ActivateScreen(UIManager.Screen.COUNTDOWN);
                _waitForGameToStart = true;
				_elapsedSecondsFromLevelStart = 0;
				break;
			case GameState.PAUSE:
                UIManager.instance.ActivateScreen(UIManager.Screen.GAME);
                UIManager.instance.ActivateScreen(UIManager.Screen.PAUSE);
                break;
			case GameState.GAME:
                UIManager.instance.ActivateScreen(UIManager.Screen.GAME);
                _waitForGameToStart = false;
				Config.isGamePaused = false;
				break;
            case GameState.LEVEL:
                UIManager.instance.ActivateScreen(UIManager.Screen.LEVEL);
                break;
            case GameState.GAME_OVER:
                UIManager.instance.ActivateScreen(UIManager.Screen.GAME_OVER);
                break;
			case GameState.HIGHSCORES:
                UIManager.instance.ActivateScreen(UIManager.Screen.HIGHSCORE);
                break;
		}
	}

	public void ResetGame ()
	{
		_level = 1;
		_score = 0;
		_lives = LIVES_MAX;

		UpdateLevel ();
		UpdateScore ();
		UpdateLives();

        _player.transform.position = Config.PLAYERS_START_POS;
        _player.gameObject.SetActive(false);

        _enemiesController.Reset();
		SetGameState (GameState.MENU);
	}

	public void GameSetup ()
	{
        _player.transform.position = Config.PLAYERS_START_POS;
        _player.gameObject.SetActive(true);
        _enemiesController.Reset();
        _enemiesController.UpdateEnemiesNumber (_level);
        _enemiesController.CreateEnemies ();

        SetGameState(GameState.COUNT_DOWN);
        Invoke("StartNewGame", GAME_START_TIME);
	}

	private void StartNewGame ()
	{
		SetGameState (GameState.GAME);

        _enemiesController.StartShooting ();
        _enemiesController.CallMotherShip();
	}

    private void UpdateCountDown()
    {
        string time = Mathf.Ceil(GAME_START_TIME - _elapsedSecondsFromLevelStart).ToString();
        float alpha = (GAME_START_TIME - _elapsedSecondsFromLevelStart) / GAME_START_TIME;

        UIManager.instance.SetCountDown(time, alpha);

        _elapsedSecondsFromLevelStart += Time.deltaTime;
    }

    public void NextLevel()
	{
		_level++;
		UpdateLevel ();
		GameSetup();
	}

	private void UpdateLevel ()
	{
        UIManager.instance.SetLevel(_level.ToString());
	}

	private void UpdateScore ()
	{
        UIManager.instance.SetScore(_score.ToString());
	}

	private void UpdateLives ()
	{
        UIManager.instance.SetLives(_lives.ToString());
    }

	public void OnEnemyHit (int points)
	{
		Score += points;
		UpdateScore();

		StopCoroutine("UpdateEnemies");
		StartCoroutine("UpdateEnemies");
	}

	IEnumerator UpdateEnemies ()
	{
		yield return new WaitForSeconds(Config.EXPLOSION_TIME_OUT);
        _enemiesController.UpdateBounds();
	}

	public void OnPlayerHit ()
	{
		if (_lives > 1)
		{
			_lives--;
			UpdateLives ();
			_player.gameObject.GetComponent<Player>().Explode();
		}
		else
        {
            EndGame();
        }	
	}

    public void EndLevel ()
    {
        StopAllCoroutines();
        SetGameState(GameState.LEVEL);

        _player.gameObject.SetActive(false);
        _enemiesController.DestroyAllEnemies();

        UIManager.instance.SetNextLevel(_score.ToString());
    }

	public void EndGame ()
	{
		StopAllCoroutines();
		SetGameState(GameState.GAME_OVER);

        _player.gameObject.SetActive(false);
        _enemiesController.DestroyAllEnemies();
	}
}