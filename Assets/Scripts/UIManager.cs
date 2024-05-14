using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    //public TMP_Text baseHealthText;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        scoreText.text = "Score: " + "0".ToString();
    }   

    public void UpdatePlayerScoreText(int score) { scoreText.text = "Score: " + score.ToString(); }
    public void UpdatePlayerHighScoreText(int highScore) { highScoreText.text = "Highscore: " + highScore.ToString(); }
      
    
}
