using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер главного меню
/// </summary>
public class MenuController : IController {

    public static MenuController controller;

    public MenuButton startBttn, soundBttn, infoBttn, rateBttn, faceButton;//кнопки меню
    public bool rated;//была ли нажата кнока rate
    public Texture sadFace, smileFace;//текстуры кнопки с лицом
    public bool facePressed;//нажата ли кнопка с лицом
    public GameObject[] menuObjGroup, developersObjGroup;//объекты когда нажата кнопка с лицом и когда отжата

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        //включается музыка
        MainController.controller.SetLoopLevelAudio(0);
        MainController.controller.loopMusicOn = true;
        //отжимаются кнопки
        startBttn.pressed = false;
        infoBttn.pressed = false;
        rateBttn.pressed = false;
        faceButton.pressed = false;
        faceButton.SetTextureUp(sadFace);
        rated = false;
        rateBttn.pressed = false;
        if (MainController.controller.soundOn)//звук отживается только если он включен
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
        if (s == "Start")//старт игрыц
        {
            MainController.controller.GoToScreen(GameController.controller, 0.3f);
        }
        if (s == "Info")//обучение
        {
            MainController.controller.GoToScreen(InfoController.controller, 0.3f);
        }
        if(s == "rate")
        {
            if (!rated)//отправляем на страницу приложения и делаем лицо улыбающимся
            {
                Application.OpenURL("market://details?id=com.gameloft.android.ANMP.GloftPOHM");
                rated = true;
                rateBttn.pressed = true;
                faceButton.SetTextureUp(smileFace);
            }
        }
        if (s == "developers")
        {
            if (facePressed)//если кнопка лица нажата то делаем активными объекты меню и возвращаемся в главное меню если необходимо
            {
                facePressed = false;
                faceButton.pressed = false;
                foreach (GameObject obj in menuObjGroup)
                    obj.SetActive(true);
                foreach (GameObject obj in developersObjGroup)
                    obj.SetActive(false);
                if (MainController.controller.currentScreen != MenuController.controller)
                    MainController.controller.GoToScreen(MenuController.controller, 0f);
            }
            else//еслим отжата, то делаем активными объекты экрана с инфой о разарабах
            {
                facePressed = true;
                faceButton.pressed = true;
                foreach (GameObject obj in menuObjGroup)
                    obj.SetActive(false);
                foreach (GameObject obj in developersObjGroup)
                    obj.SetActive(true);
            }
        }
        if(s == "sound")//включение/отключение звука
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
