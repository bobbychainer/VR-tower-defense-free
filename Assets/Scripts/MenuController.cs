using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour {
    public static MenuController instance;
    private PlayerController playerController;
    private GameObject mainMenu;
    private GameObject creditsMenu;
    private GameObject gameOverMenu;
    private GameObject settingsMenu;
    public TMP_Text highScoresText;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        mainMenu = GameObject.Find("MainMenu");
        creditsMenu = GameObject.Find("CreditsMenu");
        gameOverMenu = GameObject.Find("GameOverMenu");
        settingsMenu = GameObject.Find("SettingsMenu");
        if (mainMenu == null || creditsMenu == null || gameOverMenu == null|| settingsMenu == null ) Debug.Log("Menus not found");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void StartFirstLevel() {
        Debug.Log("StartFirstLevel()");
        GameManager.instance.StartGame();
    }

    public void LoadGameOverScene() {
        Debug.Log("LoadGameOverScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void LoadMainMenuScene() {
        Debug.Log("LoadMainMenuScene()");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void LoadCreditsScene() {
        Debug.Log("LoadCreditsScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
    
    public void LoadSettingsScene() {
        Debug.Log("LoadCreditsScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        gameOverMenu.SetActive(false);
    }

    public void Quit() {
        Debug.Log("Quit()");
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
