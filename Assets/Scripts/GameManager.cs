using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private PlayerController playerController;
    private Spawner enemySpawner;
    public enum GameState { PREPARATION, ATTACK }
    public Dictionary<string, List<int>> towerPrices = new Dictionary<string, List<int>>();
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
        if (enemySpawner == null || playerController == null) Debug.Log("Scripts in GM not found.");
        // initialize game variables
        playerScore = 0;
        playerHighScore = PlayerPrefs.GetInt("HighScore", 0);
        currentRound = 1;
        currentTimer = timer;
        currentState = GameState.PREPARATION;
        baseMaxHealth = 100;
        baseCurrHealth = baseMaxHealth;
        playerCoins = 500;

        playerMaxHealth = 10;
        playerCurrHealth = playerMaxHealth;
        
        UpdateHighScore();

        // DICT
        towerPrices.Add("SMALL", new List<int> {100, 100, 200, 300});
        towerPrices.Add("RAPID", new List<int> {100, 100, 200, 300});
        towerPrices.Add("LASER", new List<int> {100, 100, 200, 300});
    }

    // runs the timer
    private void Update() {
        if (isTimerRunning) {
            currentTimer -= Time.deltaTime;
            UIManager.instance.UpdateTimerText(currentTimer);
        }
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
            UpdateHighScore();
        }
    }

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
	
	
	public int GetTowerCosts(string name, int level)  {
		List<int> towerPriceList = towerPrices[name];
		return towerPriceList[level];
	}

}
