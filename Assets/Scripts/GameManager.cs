using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private int playerScore;
    private int playerHighScore;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {

        // initialize game variables
        playerScore = 0;
        playerHighScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateHighScore();
    }
    public int GetPlayerScore() { return playerScore; }
    void UpdateHighScore() { UIManager.instance.UpdatePlayerHighScoreText(playerHighScore); }

    public void UpdatePlayerScore(int score) {
        playerScore += score;
        UIManager.instance.UpdatePlayerScoreText(playerScore);
        // check if new highscore
        if (playerScore > playerHighScore) {
            playerHighScore = playerScore;
            PlayerPrefs.SetInt("HighScore", playerHighScore); // safe new highscore
            UpdateHighScore();
        }
    }
    
}
