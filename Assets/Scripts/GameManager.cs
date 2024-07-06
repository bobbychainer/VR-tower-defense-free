using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private GenerateCubes generateCubes;
    private PlayerController playerController;
    private GameObject pauseMenu;
    private Spawner enemySpawner;
    public Dictionary<string, List<int>> towerPrices = new Dictionary<string, List<int>>();
	public Dictionary<string, List<float>> damageUpgrades = new Dictionary<string, List<float>>();
	public Dictionary<string, List<float>> attackCooldownUpgrades = new Dictionary<string, List<float>>();
	public Dictionary<string, List<float>> attackRadiusUpgrades = new Dictionary<string, List<float>>();
    public enum GameState { PREPARATION, ATTACK, PAUSED}
    private List<int> lastTenHighScores = new List<int>();
    public GameState currentState;
    public bool isTimerRunning = false; 
    private float timer = 10f;
    private float currentTimer;
    private float playerScore;
    private float playerHighScore;
    private int currentRound;
    private float baseCurrHealth;
    private float baseMaxHealth;
    private float playerCoins; 
    public bool canBuy = true;
    private float playerCurrHealth;
    private float playerMaxHealth;

    private AudioManager audioManager;
    
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
        pauseMenu.SetActive(false);

        // Tower Prices
        towerPrices.Add("SMALL", new List<int> { 100, 100, 200, 300, 400, 500, 600 });
        towerPrices.Add("RAPID", new List<int> { 100, 100, 200, 300, 400, 500 });
        towerPrices.Add("LASER", new List<int> { 100, 100, 200, 300, 400 });

        // Damage Upgrades
        damageUpgrades.Add("SMALL", new List<float> { 0, 1, 0, 0, 3, 0, 0 });
        damageUpgrades.Add("RAPID", new List<float> { 0, 0, 2, 0, 3, 0 });
        damageUpgrades.Add("LASER", new List<float> { 0, 1, 3, 0, 5 });

        // Attack Cooldown Upgrades
        attackCooldownUpgrades.Add("SMALL", new List<float> { 0f, 0f, 0.9f, 0.8f, 0f, 0.7f, 0f });
        attackCooldownUpgrades.Add("RAPID", new List<float> { 0f, 0.8f, 0f, 0.6f, 0f, 0.4f });
        attackCooldownUpgrades.Add("LASER", new List<float> { 0f, 0f, 4f, 3f, 0f });

        // Attack Radius Upgrades
        attackRadiusUpgrades.Add("SMALL", new List<float> { 0f, 0f, 0f, 10f, 0f, 14f, 16f });
        attackRadiusUpgrades.Add("RAPID", new List<float> { 0f, 0f, 0f, 10f, 14f, 0f });
        attackRadiusUpgrades.Add("LASER", new List<float> { 0f, 0f, 0f, 10f, 14f });

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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

    // starts the game after button clicked
    public void StartGame() {
        Debug.Log("StartGame");
        playerController.freezePlayer = false;
        playerController.RespawnPlayer();
        UIManager.instance.playerUI.SetActive(true);
        
        playerScore = 0;
        currentRound = 1;
        currentTimer = timer;
        currentState = GameState.PREPARATION;
        baseMaxHealth = 100;
        baseCurrHealth = baseMaxHealth;
        playerCoins = 200;
        playerMaxHealth = 5;
        playerCurrHealth = playerMaxHealth;
        UIManager.instance.UpdateRound(currentRound);
        UIManager.instance.UpdatePlayerScoreText(playerScore);  
        UIManager.instance.UpdatePlayerCoinsText(playerCoins);  
        UIManager.instance.UpdateBaseHealthText(baseCurrHealth);    
        UIManager.instance.UpdatePlayerHealthText(playerCurrHealth);
        UIManager.instance.UpdateGameState("Preparation");
        pauseMenu.SetActive(false);
        UpdateHighScore();
    }

    // switches between preparation and attack state
    public void ChangeGameState() {
        Debug.Log("ChangeGameState");
        if (currentState == GameState.PREPARATION) { // Next is ATTACK
            audioManager.ChangeBGM(audioManager.attackPhase);
            isTimerRunning = true;
            currentTimer = timer;
            currentState = GameState.ATTACK;
            enemySpawner.StartEnemySpawn();
            UIManager.instance.UpdateGameState("Attack"); 
            UIManager.instance.ToggleReadyButton(false);
        } else if (currentState == GameState.ATTACK) { // Next is PREP
            audioManager.ChangeBGM(audioManager.background);
            enemySpawner.IncreaseSpawnRate(currentRound);
            EnemyController.IncreaseEnemyStats();
            generateCubes.ExtendPath(currentRound + 1);
            playerController.freezePlayer = false;
            playerCoins += 200;
            playerCurrHealth = playerMaxHealth;
            currentState = GameState.PREPARATION;
            isTimerRunning = false;
            enemySpawner.StopEnemySpawn();
            currentRound++; 
            UIManager.instance.UpdatePlayerCoinsText(playerCoins);
            UIManager.instance.UpdatePlayerHealthText(playerCurrHealth);
            UIManager.instance.UpdateRound(currentRound);
            UIManager.instance.UpdateGameState("Preparation"); 
            UIManager.instance.ToggleReadyButton(true);
        }
    }

    // GETTER AND IS FUNCTIONS /////////////////////////////////////////////////////////////////////////
    public float GetPlayerScore() { return playerScore; }
    public float GetPlayerCoins() { return playerCoins; }
    public float GetPlayerHealth() { return playerCurrHealth; }
    public float GetBaseHealth() { return baseCurrHealth; }
    public int GetRound() { return currentRound; }
    public GameState GetState() { return currentState; }
    
	public int GetTowerCosts(string name, int level)  {
		List<int> towerPriceList = towerPrices[name];
		return towerPriceList[level];
	}
	
	public float GetDamageUpgrade(string name, int level) {
		List<float> upgradeList = damageUpgrades[name];
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

	public bool IsPreparationGameState() { return currentState == GameState.PREPARATION; }
	public bool IsAttackGameState() { return currentState == GameState.ATTACK; }

    private void OnEnable() { pauseTriggerAction.action.performed += OnPauseTriggerPressed; }

    private void OnDisable() { pauseTriggerAction.action.performed -= OnPauseTriggerPressed; }


    // PASUE /////////////////////////////////////////////////////////////////////////
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


    // SCORES AND COINS /////////////////////////////////////////////////////////////////////////
    public void AddCoins(float coins) {
        playerCoins += coins;
        UIManager.instance.UpdatePlayerCoinsText(playerCoins);
    }
    void UpdateHighScore() { UIManager.instance.UpdatePlayerHighScoreText(playerHighScore); }
    public void RemoveCoins(int coins) {
        audioManager.PlaySFX(audioManager.coinsSound);
        playerCoins -= coins;
        UIManager.instance.UpdatePlayerCoinsText(playerCoins);
    }

    public void UpdatePlayerScore(float score) {
        playerScore += score;
        UIManager.instance.UpdatePlayerScoreText(playerScore);
        // check if new highscore
        if (playerScore > playerHighScore) {
            playerHighScore = playerScore;
            PlayerPrefs.SetInt("HighScore", (int)playerHighScore); // safe new highscore
            UpdateHighScore();
        }
    }

    void LoadHighScores() {
        lastTenHighScores.Clear();
        int count = PlayerPrefs.GetInt("HighScoreCount", 0);
        for (int i = 0; i < count; i++) lastTenHighScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
    }

    public List<int> GetLastTenHighScores() { return new List<int>(lastTenHighScores); }

    // DAMAGE /////////////////////////////////////////////////////////////////////////
    public void TakeBaseDamage(float dmg) {
        Debug.Log("BH: " + baseCurrHealth);
        baseCurrHealth -= dmg;
        UIManager.instance.UpdateBaseHealthText(baseCurrHealth);
        if (baseCurrHealth <= 0) {
            //Debug.Log("Base Health 0, Load GameOverScene");
            //MenuController.instance.LoadGameOverScene();
            audioManager.PlaySFX(audioManager.gameOverSound);
            audioManager.PlaySFX(audioManager.gameOverVoice);
            StopGame();
        }
    }

    public void StopGame() { // TODO: Fix GameOver behavior
        Debug.Log("StopGame");
        //playerController.freezePlayer = true;
        //playerController.moveProvider.enabled = false;
        playerController.RespawnPlayer();
        MenuController.instance.LoadMainMenuScene();
        currentState = GameState.PREPARATION;
        isTimerRunning = false;
        Time.timeScale = 0f;
    }

    public void TakePlayerDamage(float dmg) {
        playerCurrHealth -= dmg;
        if (playerCurrHealth <= 0) {
            Debug.Log("Player Health 0, Freeze Player");
            playerController.freezePlayer = true; // TODO: check if its really working on VR
            Debug.Log("FROZEN");
        }
        UIManager.instance.UpdatePlayerHealthText(playerCurrHealth);
    }
	
	
}
