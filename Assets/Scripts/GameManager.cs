using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private int playerScore;
    private int playerHighScore;
    public int baseCurrHealth; //Zu Debug Zwecken Public, wird später private
    private int baseMaxHealth;

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
        baseMaxHealth = 20;
        baseCurrHealth = baseMaxHealth;
        
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

    public void TakeBaseDmg(int dmg)
    {
        baseCurrHealth -= dmg;
        baseCurrHealth = Mathf.Clamp(baseCurrHealth, 0, baseMaxHealth); // sorgt dafür, dass currHealth immer zwischen 0 und maxHealth ist

        if (baseCurrHealth <= 0)
        {
            gameOver();
        }
    }

    private void gameOver()
    {
        Debug.Log("Base Destroyed, Game Over!");
        //EndScreen
    }
    
}
