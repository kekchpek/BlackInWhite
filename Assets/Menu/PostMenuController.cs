using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер меню после окончания игры
/// </summary>
public class PostMenuController : IController {

    public static PostMenuController controller;

    public Text points, best;
    public MenuButton startButton, infoButton, rateButton, soundButton, faceButton;
    public bool rated, facePressed;
    public Texture sadFace, smileFace;
    public GameObject[] menuObjGroup, developersObjGroup;
    

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        MainController.controller.SetLoopLevelAudio(0);
        points.text = GameController.controller.points.ToString();
        if (GameController.controller.points > MainController.controller.bestPoints)
            MainController.controller.SaveRocords(GameController.controller.points);
        best.text = "BEST:  " + MainController.controller.bestPoints.ToString();
        infoButton.pressed = false;
        startButton.pressed = false;
        faceButton.SetTextureUp(sadFace);
        rated = false;
        rateButton.pressed = false;
        if (MainController.controller.soundOn)
        {
            soundButton.pressed = false;
        }
        else
        {
            soundButton.pressed = true;
        }
    }

    /// <summary>
    /// Вызов определённой функции(для кнопок)
    /// </summary>
    /// <param name="s"> название функции</param>
    public void CallFunction(string s)
    {
        if (s == "Start")
        {
            MainController.controller.GoToScreen(GameController.controller, 0.3f);
        }
        if (s == "Info")
        {
            MainController.controller.GoToScreen(InfoController.controller, 0.3f);
        }
        if (s == "rate")
        {
            if (!rated)
            {
                Application.OpenURL("market://details?id=com.gameloft.android.ANMP.GloftPOHM");
                rated = true;
                rateButton.pressed = true;
                faceButton.SetTextureUp(smileFace);
            }
        }
        if(s == "developers")
        {
            if(facePressed)
            {
                facePressed = false;
                faceButton.pressed = false;
                foreach (GameObject obj in menuObjGroup)
                    obj.SetActive(true);
                foreach (GameObject obj in developersObjGroup)
                    obj.SetActive(false);
                MainController.controller.GoToScreen(MenuController.controller, 0f);
            }
            else
            {
                facePressed = true;
                faceButton.pressed = true;
                foreach (GameObject obj in menuObjGroup)
                    obj.SetActive(false);
                foreach (GameObject obj in developersObjGroup)
                    obj.SetActive(true);
            }
        }
        if (s == "sound")
        {
            if (MainController.controller.soundOn)
            {
                soundButton.pressed = true;
                MainController.controller.soundOn = false;
                PlayerPrefs.SetInt("sound", 0);
            }
            else
            {
                soundButton.pressed = false;
                MainController.controller.soundOn = true;
                PlayerPrefs.SetInt("sound", 1);
            }
        }
    }


}
