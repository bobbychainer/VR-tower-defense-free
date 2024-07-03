using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private GenerateCubes generateCubes;
    private PlayerController playerController;
    private GameObject pauseMenu;
    private Spawner enemySpawner;
    public Dictionary<string, List<int>> towerPrices = new Dictionary<string, List<int>>();
	public Dictionary<string, List<int>> damageUpgrades = new Dictionary<string, List<int>>();
	public Dictionary<string, List<float>> attackCooldownUpgrades = new Dictionary<string, List<float>>();
	public Dictionary<string, List<float>> attackRadiusUpgrades = new Dictionary<string, List<float>>();
    public enum GameState { PREPARATION, ATTACK, PAUSED}
    private List<int> lastTenHighScores = new List<int>();
    //public GameState currentStatadd_e;
    public GameState currentState;
    public bool isTimerRunning = false; 
    public float timer = 60f;
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
    
    public InputActionProperty pauseTriggerAction;

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
        generateCubes = FindObjectOfType<GenerateCubes>();
        pauseMenu = GameObject.Find("PauseMenu");
        if (enemySpawner == null || playerController == null || generateCubes == null) Debug.Log("Scripts in GM not found.");
        
        // initialize game variables
        playerHighScore = PlayerPrefs.GetInt("HighScore", 0);
        LoadHighScores(); 
        UpdateHighScore();

        // Tower Prices
        towerPrices.Add("SMALL", new List<int> { 10, 10, 20, 30, 40, 50, 60 });
        towerPrices.Add("RAPID", new List<int> { 10, 10, 20, 30, 40, 50 });
        towerPrices.Add("LASER", new List<int> { 10, 10, 20, 30, 40 });

        // Damage Upgrades
        damageUpgrades.Add("SMALL", new List<int> { 0, 2, 0, 0, 4, 0, 0 });
        damageUpgrades.Add("RAPID", new List<int> { 0, 0, 2, 0, 3, 0 });
        damageUpgrades.Add("LASER", new List<int> { 0, 2, 4, 0, 6 });

        // Attack Cooldown Upgrades
        attackCooldownUpgrades.Add("SMALL", new List<float> { 0f, 0f, 0.9f, 0.8f, 0f, 0.7f, 0f });
        attackCooldownUpgrades.Add("RAPID", new List<float> { 0f, 0.6f, 0f, 0.5f, 0f, 0.3f });
        attackCooldownUpgrades.Add("LASER", new List<float> { 0f, 0f, 4f, 3f, 0f });

        // Attack Radius Upgrades
        attackRadiusUpgrades.Add("SMALL", new List<float> { 0f, 0f, 0f, 14f, 0f, 18f, 22f });
        attackRadiusUpgrades.Add("RAPID", new List<float> { 0f, 0f, 0f, 12f, 16f, 0f });
        attackRadiusUpgrades.Add("LASER", new List<float> { 0f, 0f, 0f, 14f, 18f });
    }

    // runs the timer
    private void Update() {
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

    private void OnEnable() {
        pauseTriggerAction.action.performed += OnPauseTriggerPressed;
    }

    private void OnDisable() {
        pauseTriggerAction.action.performed -= OnPauseTriggerPressed;
    }

    private void OnPauseTriggerPressed(InputAction.CallbackContext context) {
            if (currentState == GameState.ATTACK) {
                PauseGame();
            } else if (currentState == GameState.PAUSED) {
                ResumeGame();
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
        UIManager.instance.UpdateRound(currentRound);
        playerScore = 0;
        currentRound = 1;
        currentTimer = timer;
        currentState = GameState.PREPARATION;
        baseMaxHealth = 100;
        baseCurrHealth = baseMaxHealth;
        playerCoins = 500;

        playerMaxHealth = 10;
        playerCurrHealth = playerMaxHealth;
        
        UpdateHighScore();

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
            generateCubes.ExtendPath(currentRound);
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
	
	
	public int GetTowerCosts(string name, int level)  {
		List<int> towerPriceList = towerPrices[name];
		return towerPriceList[level];
	}
	
	public int GetDamageUpgrade(string name, int level) {
		List<int> upgradeList = damageUpgrades[name];
		return upgradeList[level];
	}
	
	public float GetAttackCooldownUpgrade(string name, int level) {
		List<float> upgradeList = attackCooldownUpgrades[name];
		return upgradeList[level];
	}
	
	public float GetAttackRadiusUpgrade(string name, int level) {
		List<float> upgradeList = attackRadiusUpgrades[name];
		return upgradeList[level];
	}
}
