using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();

        transform.Find("retryButton").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene);  };
        transform.Find("retryButton").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("mainMenuButton").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.MainMenu); };
        transform.Find("mainMenuButton").GetComponent<Button_UI>().AddButtonSounds();


    }

    private void Start()
    {
        Fish.GetInstance().OnDied += Fish_OnDied;
        Hide();
    }

    private void Fish_OnDied(object sender, System.EventArgs e)
    {
        scoreText.text = Level.GetInstance().getPipesPassedCount().ToString();
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
 