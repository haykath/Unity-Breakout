using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Text highScoreText;
    public int newGameLevel = 1;

    int highScore = 0;

	// Use this for initialization
	void Start () {
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = string.Format("HIGH SCORE: {0}", highScore);
	}
	
    public void NewGame()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
