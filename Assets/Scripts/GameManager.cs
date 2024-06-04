using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private Spawner enemySpawner;
    public enum GameState { PREPARATION, ATTACK }
    public GameState currentState;
    public bool isTimerRunning = false; 
    public float timer = 10f;
    private float currentTimer;
    private int playerScore;
    private int playerHighScore;
    private int currentRound;
    private int baseCurrHealth;
    private int baseMaxHealth;
    private int playerCoins; // TODO: implement tower placement with buyable towers from coins 
    private int playerCurrHealth;
    private int playerMaxHealth;

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
        baseMaxHealth = 20;
        baseCurrHealth = baseMaxHealth;
        playerCoins = 100;

        playerMaxHealth = 100;
        playerCurrHealth = playerMaxHealth;

        
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
            baseCurrHealth = baseMaxHealth;
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

    public void TakeBaseDamage(int dmg) {// TODO: Fix Nullref
        Debug.Log("BH: " + baseCurrHealth);
        baseCurrHealth -= dmg;
        UIManager.instance.UpdateBaseHealthText(baseCurrHealth);
        //baseCurrHealth = Mathf.Clamp(baseCurrHealth, 0, baseMaxHealth); // sorgt dafuer, dass currHealth immer zwischen 0 und maxHealth ist
        if (baseCurrHealth <= 0) {
            Debug.Log("Base Health 0");
            MenuController.instance.LoadGameOverScene();
        }
    }

    public void TakePlayerDamage(int dmg)
    {
        playerCurrHealth -= dmg;
        playerCurrHealth = Mathf.Clamp(playerCurrHealth, 0, playerMaxHealth);
        UIManager.instance.UpdatePlayerHealthText(playerCurrHealth);
    }

}
