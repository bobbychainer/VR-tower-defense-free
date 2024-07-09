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
    private GameObject tutorialMenu;
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
        tutorialMenu = GameObject.Find("TutorialMenu");
        if (mainMenu == null || creditsMenu == null || gameOverMenu == null|| settingsMenu == null || tutorialMenu == null) Debug.Log("One of the Menus not found");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void ReloadScene() {
        SceneManager.LoadScene("Level01");
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
        tutorialMenu.SetActive(false);
    }

    public void LoadMainMenuScene() {
        Debug.Log("LoadMainMenuScene()");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void LoadCreditsScene() {
        Debug.Log("LoadCreditsScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }
    
    public void LoadSettingsScene() {
        Debug.Log("LoadSettingsscene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }
    public void LoadTutorialScene() {
        Debug.Log("LoadTutorialScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        tutorialMenu.SetActive(true);
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
