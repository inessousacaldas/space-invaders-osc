using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance = null;

    public enum Screen { START, COUNTDOWN, PAUSE, GAME, LEVEL, GAME_OVER, HIGHSCORE };

    private const string LEVEL_TEXT = "LEVEL: ";
    private const string SCORE_TEXT = "SCORE: ";
    private const string LIVES_TEXT = "LIVES: ";
    private const string WIN_TEXT = "YOU WON!\n YOUR SCORE IS: ";
    private const string GAME_OVER_TEXT = "GAME OVER";

    [Header("Screens")]
    [SerializeField] private GameObject _menuScreen;
    [SerializeField] private GameObject _countDownScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _gameScreen;
    [SerializeField] private GameObject _nextLevelScreen;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _highScoresScreen;

    [Header("Text fields")]
    [SerializeField] private Text _countDown;
    [SerializeField] private Text _level;
    [SerializeField] private Text _score;
    [SerializeField] private Text _lives;
    [SerializeField] private Text _nextLevelMessage;
    [SerializeField] private Text _gameOver;
    [SerializeField] private Text _highscoresListNames;
    [SerializeField] private Text _highscoresListScores;
    [SerializeField] private InputField _playerName;

    public GameObject GameScreen
    {
        get
        {
            return _gameScreen;
        }

        set
        {
            _gameScreen = value;
        }
    }

    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    
    public void DeactivateAllScreens()
    {
        _menuScreen.SetActive(false);
        _countDownScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _gameScreen.SetActive(false);
        _nextLevelScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _highScoresScreen.SetActive(false);
    }

    public void ActivateScreen (Screen screen)
    {
        switch(screen)
        {
            case Screen.START:
                _menuScreen.SetActive(true);
                break;
            case Screen.COUNTDOWN:
                _countDownScreen.SetActive(true);
                break;
            case Screen.PAUSE:
                _pauseScreen.SetActive(true);
                break;
            case Screen.GAME:
                GameScreen.SetActive(true);
                break;
            case Screen.LEVEL:
                _nextLevelScreen.SetActive(true);
                break;
            case Screen.GAME_OVER:
                _gameOverScreen.SetActive(true);
                break;
            case Screen.HIGHSCORE:
                _highScoresScreen.SetActive(true);
                ShowHighscoresScreen();
                break;
        }
    }

    public void OnBtnStart()
    {
        GameManager.instance.GameSetup();
    }

    public void OnBtnExit()
    {
        HighscoresManager.instance.SaveGameData();
        Application.Quit();
    }

    public void OnBtnGoToMainMenu()
    {
        GameManager.instance.ResetGame();
    }

    public void OnBtnNextLevel()
    {
        GameManager.instance.NextLevel();
    }

    public void OnBtnHighscores()
    {
        GameManager.instance.SetGameState(GameManager.GameState.HIGHSCORES);
    }

    public void OnBtnAddNewHighscore()
    {
        HighscoresManager.instance.AddNewHighscore(_playerName.text, GameManager.instance.Score.ToString());
        GameManager.instance.SetGameState(GameManager.GameState.HIGHSCORES);
    }

    public void SetLevel(string level)
    {
        _level.text = LEVEL_TEXT + level;
    }

    public void SetScore(string score)
    {
        _score.text = SCORE_TEXT + score;
    }

    public void SetLives(string lives)
    {
        _lives.text = LIVES_TEXT + lives;
    }

    public void SetCountDown(string time, float alpha)
    {
        _countDown.text = time;
        _countDown.color = new Color(_countDown.color.r, _countDown.color.g, _countDown.color.b, alpha);
    }

    public void SetNextLevel(string score)
    {
        _nextLevelMessage.text = WIN_TEXT + score;
    }

    public void ShowHighscoresScreen()
    {
        
        //GameManager.instance.SetGameState(GameManager.GameState.HIGHSCORES);

        _highscoresListNames.text = "Loading...";
        _highscoresListNames.alignment = TextAnchor.MiddleRight;
        _highscoresListScores.text = "";

        HighscoresManager.instance.ShowHighscoresList(_highscoresListNames, _highscoresListScores);


    }
}
