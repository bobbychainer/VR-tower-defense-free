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
    private GameObject highscoreMenu;
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
        highscoreMenu = GameObject.Find("HighscoreMenu");
        if (mainMenu == null || creditsMenu == null || highscoreMenu == null ) Debug.Log("Menus not found");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        highscoreMenu.SetActive(false);
    }

    void DisplayHighScores() {
        List<int> highScores = GameManager.instance.GetLastTenHighScores();
        highScoresText.text = "Highscores:\n";
        for (int i = 0; i < highScores.Count; i++) highScoresText.text += (i + 1) + ": " + highScores[i] + "\n";
        Debug.Log(highScoresText.text);
    }


    public void StartFirstLevel() {
        Debug.Log("StartFirstLevel()");
        GameManager.instance.StartGame();
    }

    public void LoadGameOverScene() {
        Debug.Log("LoadGameOverScene()");
        SceneManager.LoadScene("GameOver");
    }

    
    public void LoadMainMenuScene() {
        Debug.Log("LoadMainMenuScene()");
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        highscoreMenu.SetActive(false);
    }

    public void LoadCreditsScene() {
        Debug.Log("LoadCreditsScene()");
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        highscoreMenu.SetActive(false);
    }
    public void LoadHighscoresScene() {
        Debug.Log("LoadHighscoresScene()");
        DisplayHighScores();
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        highscoreMenu.SetActive(true);
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
