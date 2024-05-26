using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private Spawner enemySpawner;
    private int playerScore;
    private int playerHighScore;
    public enum GameState { PREPARATION, ATTACK }
    public GameState currentState;
    public float timer = 10f;
    private float currentTimer;
    public bool isTimerRunning = false; 
    public int currentRound;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        enemySpawner = FindObjectOfType<Spawner>();
        // initialize game variables
        playerScore = 0;
        playerHighScore = PlayerPrefs.GetInt("HighScore", 0);
        currentRound = 1;
        currentTimer = timer;
        currentState = GameState.PREPARATION;
        UpdateHighScore();
    }

    // runs the timer
    private void Update() {
        if (isTimerRunning) {
            currentTimer -= Time.deltaTime;
            UIManager.instance.UpdateTimerText(currentTimer);
        }
    }

    // switches between preparation and attack state
    public void ChangeGameState() {
        Debug.Log("ChangeGameState");
        if (currentState == GameState.PREPARATION) { // Next is ATTACK
            isTimerRunning = true;
            currentTimer = timer;
            currentState = GameState.ATTACK;
            enemySpawner.StartEnemySpawn();
            UIManager.instance.UpdateGameState("Attack"); 
            UIManager.instance.ToggleReadyButton(false);
            // unfreeze player
            // switch camera
        } else if (currentState == GameState.ATTACK) { // Next is PREP
            currentState = GameState.PREPARATION;
            isTimerRunning = false;
            enemySpawner.StopEnemySpawn();
            currentRound++; 
            UIManager.instance.UpdateRound(currentRound);
            UIManager.instance.UpdateGameState("Preparation"); 
            UIManager.instance.ToggleReadyButton(true);
            // switch camera
            // freeze player
        }
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
