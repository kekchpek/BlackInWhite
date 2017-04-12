using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер главного меню
/// </summary>
public class MenuController : IController {

    public static MenuController controller;
    public MenuButton startBttn, soundBttn, infoBttn, rateBttn, faceButton;
    public bool rated;
    public Texture sadFace, smileFace;
    public bool facePressed;
    public GameObject[] menuObjGroup, developersObjGroup;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        MainController.controller.SetLoopLevelAudio(0);
        MainController.controller.loopMusicOn = true;
        startBttn.pressed = false;
        infoBttn.pressed = false;
        rateBttn.pressed = false;
        faceButton.pressed = false;
        faceButton.SetTextureUp(sadFace);
        rated = false;
        rateBttn.pressed = false;
        if (MainController.controller.soundOn)
        {
            soundBttn.pressed = false;
        }
        else
        {
            soundBttn.pressed = true;
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
        if(s == "rate")
        {
            if (!rated)
            {
                Application.OpenURL("market://details?id=com.gameloft.android.ANMP.GloftPOHM");
                rated = true;
                rateBttn.pressed = true;
                faceButton.SetTextureUp(smileFace);
            }
        }
        if (s == "developers")
        {
            if (facePressed)
            {
                facePressed = false;
                faceButton.pressed = false;
                foreach (GameObject obj in menuObjGroup)
                    obj.SetActive(true);
                foreach (GameObject obj in developersObjGroup)
                    obj.SetActive(false);
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
        if(s == "sound")
        {
            if (MainController.controller.soundOn)
            {
                soundBttn.pressed = true;
                MainController.controller.soundOn = false;
                PlayerPrefs.SetInt("sound", 0);
            }
            else
            {
                soundBttn.pressed = false;
                MainController.controller.soundOn = true;
                PlayerPrefs.SetInt("sound", 1);
            }
        }
    }

    
}
