using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер меню после окончания игры
/// </summary>
public class PostMenuController : MenuController {

    public Text points, best;

    public static new PostMenuController controller;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()//добавляется вывод текста с очками и рекордом
    {
        base.Init();
        points.text = GameController.controller.points.ToString();
        if (GameController.controller.points > MainController.controller.bestPoints)
            MainController.controller.SaveRocords(GameController.controller.points);
        best.text = "BEST:  " + MainController.controller.bestPoints.ToString();
    }
}
