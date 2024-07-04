using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    private GameObject buildUI;
    public GameObject playerUI;
    private GameObject gameUI;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text roundText;
    public TMP_Text stateText;
    public TMP_Text timerText;
    public TMP_Text baseHealthText;
    public TMP_Text playerHealthText;
    public TMP_Text playerCoinsText;
    public Button readyButton;	
    private BuildController buildController;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() { 
        playerUI = GameObject.Find("PlayerUI");
        buildUI = GameObject.Find("BuildUI");
        gameUI = GameObject.Find("GameUI");
        if (buildUI == null || playerUI == null || gameUI == null) Debug.Log("UIs not found");
        buildUI.SetActive(true);
        playerUI.SetActive(false);

        //scoreText.text = "Score: " + GameManager.instance.GetPlayerScore().ToString();
        //roundText.text = "Round: " + GameManager.instance.GetRound().ToString();
        //stateText.text = "State: " + GameManager.instance.GetState().ToString();
        //baseHealthText.text = "Base Health: " + GameManager.instance.GetBaseHealth().ToString();
        //playerHealthText.text = "Player Health: " + GameManager.instance.GetPlayerHealth().ToString();
        //playerCoinsText.text = "Player Coins: " + GameManager.instance.GetPlayerCoins().ToString();
		
		buildController = FindObjectOfType<BuildController>();
    }

    // change visibility of ready button
    public void ToggleReadyButton(bool isVisible) { readyButton.gameObject.SetActive(isVisible); }

    // update ui texts
    public void UpdatePlayerHealthText(float playerHealth) { playerHealthText.text = "Player Health: " + playerHealth.ToString(); }
    public void UpdatePlayerCoinsText(float playerCoins) { playerCoinsText.text = "Player Coins: " + playerCoins.ToString(); }
    public void UpdateBaseHealthText(float baseHealth) { baseHealthText.text = "Base Health: " + baseHealth.ToString(); }
    public void UpdatePlayerScoreText(float score) { scoreText.text = "Score: " + score.ToString(); }
    public void UpdatePlayerHighScoreText(float highScore) { highScoreText.text = "Highscore: " + highScore.ToString(); }
    public void UpdateRound(int round) { roundText.text = "Round: " + round.ToString(); }
    public void UpdateGameState(string state) { stateText.text = "State " + state; }
    public void UpdateTimerText(float time) {timerText.text = "Timer: " + Mathf.RoundToInt(time).ToString(); }

    // instatiate small tower
	public void SmallTowerButtonPressed() { 
        Debug.Log("S pressed");
		int towerPrice = GameManager.instance.GetTowerCosts("SMALL", 0);
        if (GameManager.instance.GetPlayerCoins() >= towerPrice) {
            buildController.SpawnSmallTower(); 
        } else {
            Debug.Log("Not enough Coins to Buy");
        }
        
    }

	// instatiate rapid tower
	public void RapidTowerButtonPressed() { 
        Debug.Log("R pressed");
		int towerPrice = GameManager.instance.GetTowerCosts("RAPID", 0);
        if (GameManager.instance.GetPlayerCoins() >= towerPrice) {
            buildController.SpawnRapidTower(); 
        } else {
            Debug.Log("Not enough Coins to Buy");
        }
    }

	// instatiate laser tower
	public void LaserTowerButtonPressed() { 
        Debug.Log("L pressed");
		int towerPrice = GameManager.instance.GetTowerCosts("LASER", 0);
        if (GameManager.instance.GetPlayerCoins() >= towerPrice) {
            buildController.SpawnLaserTower(); 
        } else {
            Debug.Log("Not enough Coins to Buy");
        }
    }
}
