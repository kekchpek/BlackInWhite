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
    

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        points.text = GameController.controller.points.ToString();
        if (GameController.controller.points > MainController.controller.bestPoints)
            MainController.controller.SaveRocords(GameController.controller.points);
        best.text = "BEST:  " + MainController.controller.bestPoints.ToString();
        infoButton.Unpress();
        startButton.Unpress();
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
            MainController.controller.GoToScreen(InfoController.controller, 0);
        }
    }


}
