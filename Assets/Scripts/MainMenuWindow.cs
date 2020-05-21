  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class MainMenuWindow : MonoBehaviour
{
    private void Start()
    {
        transform.parent.Find("playButton").GetComponent<Button_UI>().ClickFunc = () => {Loader.Load(Loader.Scene.GameScene); };

        transform.parent.Find("quitButton").GetComponent<Button_UI>().ClickFunc = () => {Application.Quit(); };

    }
}
 