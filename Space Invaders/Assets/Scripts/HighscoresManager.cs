using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighscoresManager : MonoBehaviour
{
    public static HighscoresManager instance = null;

    private const int MAX_HIGHSCORES = 10;

    private string _gameDataFileName = "data.json";
    private PlayersData _playersData;

    [Serializable]
    public class PlayersData
    {
        public List<PlayerData> playerDataList;
    }

    [Serializable]
    public class PlayerData
    {
        public string name;
        public string score;
    }

    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        LoadGameData();
    }

    private void LoadGameData()
    {
        string filePath = Application.dataPath + _gameDataFileName;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            _playersData = JsonUtility.FromJson<PlayersData>(dataAsJson);
        }
        else
        {
            _playersData = new PlayersData
            {
                playerDataList = new List<PlayerData>()
            };
        }
    }

    public void SaveGameData()
    {

        string dataAsJson = JsonUtility.ToJson(_playersData);

        string filePath = Application.dataPath + _gameDataFileName;
        File.WriteAllText(filePath, dataAsJson);

    }


    public void Highscores(Text HighScoresListNames, Text HighScoresListScores)
    {
        UIManager.instance.ShowHighscoresScreen();
    }

    private void GetHighscoresList()
    {
        
    }

    public void ShowHighscoresList(Text HighScoresListNames, Text HighScoresListScores)
    {
   
        HighScoresListNames.text = "";
        HighScoresListNames.alignment = TextAnchor.MiddleLeft;

        _playersData.playerDataList.Sort((p1, p2) => Convert.ToInt32(p1.score).CompareTo(Convert.ToInt32(p2.score)));
        _playersData.playerDataList.Reverse();

        for (int i = 0; i < _playersData.playerDataList.Count && i < MAX_HIGHSCORES; i++)
        {
            HighScoresListNames.text += (i + 1) + ".\t" + _playersData.playerDataList[i].name + '\n';
            HighScoresListScores.text += _playersData.playerDataList[i].score + '\n';
        }
 
    }

    public void AddNewHighscore(string playerName, string playerScore)
    {

        PlayerData newPlayerData = new PlayerData
        {
            name = playerName,
            score = playerScore
        };
        _playersData.playerDataList.Add(newPlayerData);
        SaveGameData();
        GameManager.instance.SetGameState(GameManager.GameState.HIGHSCORES);
        //ShowHighscoresScreen();
    }
}
