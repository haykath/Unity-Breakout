using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{

    static int blocksPerRegion = 24;
    public static bool locked = false;

    const int startingLives = 3;
    const int lifeUpScore = 10000;
    public const float speedUpFactor = 0.01f;
    static int lives = startingLives;
    static int score = 0;
    static int lastLifeWin = 0;
    static int level = 1;

    public BlockRegion topLeft;
    public BlockRegion bottomLeft;
    public BlockRegion topRight;
    public BlockRegion bottomRight;

    public Text livesTxt;
    public Text scoreTxt;
    public Text leveltext;
    public Text screenText;
    public Text highScore;

    static int remainingBlocks = 0;
    bool begin;

    void Awake()
    {
        remainingBlocks = 0;
        begin = false;

        if (!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }

    }

    // Use this for initialization
    void Start()
    {
        locked = false;
        topLeft.regionType = topRight.regionType = bottomLeft.regionType = bottomRight.regionType = BlockRegion.BlockRegionMode.BLOCK_LIST;

        if (topLeft.blockList == null)
        {
            topLeft.blockCount = blocksPerRegion;
            topLeft.RecalculateSingleBlock();
        }

        foreach (GameObject o in topLeft.blockList)
        {
            Block b = o.GetComponent<Block>();
            if (!b) continue;

            int i = topLeft.blockList.IndexOf(o);
            int n = (i / topLeft.lineCount + 1) * (i % topLeft.columnCount + 1);

            if ((n & level) != 0)
            {
                b.life = ((n + level*i) % 3) + 1;
            }
            else
                b.life = 0;

            b.value = b.life;
            b.Update();

            topRight.blockList.Add(Instantiate(o));
            bottomLeft.blockList.Add(Instantiate(o));
            bottomRight.blockList.Add(Instantiate(o));
        }

        topLeft.Recalculate();
        topRight.Recalculate();
        bottomLeft.Recalculate();
        bottomRight.Recalculate();

        screenText.text = "LEVEL " + level.ToString();
        Invoke("FadeText", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!begin)
        {
            if (level == 1)
            {
                lives = 3;
                score = 0;
                lastLifeWin = 0;
            }
            Time.timeScale = 1;
            if (remainingBlocks == 0)
                Debug.Log("Something went wrong while generating the blocks");
            begin = true;
        }

        if (remainingBlocks == 0 && !locked)
        {
            StartCoroutine("LevelPassed");
        }
        else if (lives == 0)
        {
            StartCoroutine("GameOver");
        }

        int lifeWin = score / lifeUpScore;
        if (lifeWin > lastLifeWin)
        {
            lastLifeWin = lifeWin;
            lives++;
        }

        leveltext.text = "LEVEL: " + level.ToString();
        scoreTxt.text = "SCORE: " + score.ToString();
        livesTxt.text = "LIVES: " + lives.ToString();
        highScore.text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore").ToString();
    }

    public static void AddBlock()
    {
        remainingBlocks++;
    }

    public static void RemoveBlock(int scoreValue)
    {
        score += scoreValue * 100;

        if (remainingBlocks > 0)
            remainingBlocks--;
    }

    void FadeText()
    {
        screenText.text = "";
    }

    IEnumerator LevelPassed()
    {
        locked = true;
        screenText.text = "LEVEL CLEARED!";
        yield return new WaitForSeconds(2f);
        level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GameOver()
    {
        locked = true;
        screenText.text = "GAME OVER";
        yield return new WaitForSeconds(2f);
        level = 1;
        lives = startingLives;

        if (score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", score);

        score = 0;
        lastLifeWin = 0;
        SceneManager.LoadScene(0);
    }

    public static void BallDeath()
    {
        lives--;
    }
}
