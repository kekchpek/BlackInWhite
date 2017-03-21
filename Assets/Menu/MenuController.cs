using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер главного меню
/// </summary>
public class MenuController : IController {

    public static MenuController controller;
    public MenuButton startBttn, soundBttn, infoBttn, rateBttn;
    public bool rated;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        startBttn.Unpress();
        infoBttn.Unpress();
        rateBttn.Unpress();
        rated = false;
        rateBttn.pressed = false;
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
            MainController.controller.GoToScreen(InfoController.controller, 0f);
        }
        if(s == "rate")
        {
            if (!rated)
            {
                Application.OpenURL("market://details?id=com.gameloft.android.ANMP.GloftPOHM");
                rated = true;
                rateBttn.pressed = true;
            }
        }
    }

    
}
