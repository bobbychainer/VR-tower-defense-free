using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private PlayerController playerController;
    private GameObject pauseMenu;
    private Spawner enemySpawner;
    public enum GameState { PREPARATION, ATTACK, PAUSED}
    public Dictionary<string, int> towerPrices = new Dictionary<string, int>();
    private List<int> lastTenHighScores = new List<int>();
    public GameState currentState;
    public bool isTimerRunning = false; 
    public float timer = 10f;
    private float currentTimer;
    private int playerScore;
    private int playerHighScore;
    private int currentRound;
    private int baseCurrHealth;
    private int baseMaxHealth;
    private int playerCoins; 
    public bool canBuy = true;
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
        playerController = FindObjectOfType<PlayerController>();
        pauseMenu = GameObject.Find("PauseMenu");
        if (enemySpawner == null || playerController == null) Debug.Log("Scripts in GM not found.");
        
        // initialize game variables
        playerHighScore = PlayerPrefs.GetInt("HighScore", 0);
        LoadHighScores(); 
        UpdateHighScore();

        // DICT
        towerPrices.Add("SMALL", 100);
        towerPrices.Add("RAPID", 100);
        towerPrices.Add("LASER", 200);
        pauseMenu.SetActive(false);
    }

    // runs the timer
    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) { // TODO: Khalid change button
            if (currentState == GameState.ATTACK) {
                PauseGame();
            } else if (currentState == GameState.PAUSED) {
                ResumeGame();
            }
        }

        if (currentState == GameState.ATTACK && isTimerRunning) {
            currentTimer -= Time.deltaTime;
            UIManager.instance.UpdateTimerText(currentTimer);
            if (currentTimer <= 0) {
                currentTimer = 0;
                UIManager.instance.UpdateTimerText(currentTimer);
                isTimerRunning = false;
                ChangeGameState();
            }
        }
    }

    private void PauseGame() {
        currentState = GameState.PAUSED;
        isTimerRunning = false;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        UIManager.instance.UpdateGameState("Paused");
    }

    private void ResumeGame() {
        currentState = GameState.ATTACK;
        isTimerRunning = true;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        UIManager.instance.UpdateGameState("Attack");
    }

    // starts the game after button clicked
    public void StartGame() {
        Debug.Log("StartGame");
        playerController.freezePlayer = false;
        playerController.RespawnPlayer(playerController.initialPoint);
        UIManager.instance.playerUI.SetActive(true);
        playerScore = 0;
        currentRound = 1;
        currentTimer = timer;
        currentState = GameState.PREPARATION;
        baseMaxHealth = 100;
        baseCurrHealth = baseMaxHealth;
        playerCoins = 500;

        playerMaxHealth = 10;
        playerCurrHealth = playerMaxHealth;
    }

    public void AddCoins(int coins) {
        playerCoins += coins;
        UIManager.instance.UpdatePlayerCoinsText(playerCoins);
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
        } else if (currentState == GameState.ATTACK) { // Next is PREP
            playerController.freezePlayer = false;
            playerCoins += 200;
            baseCurrHealth = baseMaxHealth;
            currentState = GameState.PREPARATION;
            isTimerRunning = false;
            enemySpawner.StopEnemySpawn();
            currentRound++; 
            UIManager.instance.UpdateRound(currentRound);
            UIManager.instance.UpdateGameState("Preparation"); 
            UIManager.instance.ToggleReadyButton(true);
        }
    }

    public int GetPlayerScore() { return playerScore; }
    public int GetPlayerCoins() { return playerCoins; }
    public int GetPlayerHealth() { return playerCurrHealth; }
	public bool IsPreparationGameState() { return currentState == GameState.PREPARATION; }
	public bool IsAttackGameState() { return currentState == GameState.ATTACK; }

    public void RemoveCoins(int coins) {
        playerCoins -= coins;
        UIManager.instance.UpdatePlayerCoinsText(playerCoins);
    }

    void UpdateHighScore() { UIManager.instance.UpdatePlayerHighScoreText(playerHighScore); }

    public void UpdatePlayerScore(int score) {
        playerScore += score;
        UIManager.instance.UpdatePlayerScoreText(playerScore);
        // check if new highscore
        if (playerScore > playerHighScore) {
            playerHighScore = playerScore;
            PlayerPrefs.SetInt("HighScore", playerHighScore); // safe new highscore
            AddHighScore(playerScore); // add the new highscore to the list
            UpdateHighScore();
        }
    }
    void AddHighScore(int score) {
        lastTenHighScores.Add(score);
        if (lastTenHighScores.Count > 10) lastTenHighScores.RemoveAt(0);  
        SaveHighScores();
    }
    void SaveHighScores() {
        for (int i = 0; i < lastTenHighScores.Count; i++) PlayerPrefs.SetInt("HighScore" + i, lastTenHighScores[i]);
        PlayerPrefs.SetInt("HighScoreCount", lastTenHighScores.Count);
    }

    void LoadHighScores() {
        lastTenHighScores.Clear();
        int count = PlayerPrefs.GetInt("HighScoreCount", 0);
        for (int i = 0; i < count; i++) lastTenHighScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
    }

    public List<int> GetLastTenHighScores() { return new List<int>(lastTenHighScores); }

    public void TakeBaseDamage(int dmg) {
        Debug.Log("BH: " + baseCurrHealth);
        baseCurrHealth -= dmg;
        UIManager.instance.UpdateBaseHealthText(baseCurrHealth);
        if (baseCurrHealth <= 0) {
            Debug.Log("Base Health 0, Load GameOverScene");
            MenuController.instance.LoadGameOverScene();
        }
    }

    public void TakePlayerDamage(int dmg) {
        playerCurrHealth -= dmg;
        if (playerCurrHealth <= 0) {
            Debug.Log("Player Health 0, Freeze Player");
            playerController.freezePlayer = true;
            Debug.Log("FROZEN");
        }
        UIManager.instance.UpdatePlayerHealthText(playerCurrHealth);
    }
	

}
