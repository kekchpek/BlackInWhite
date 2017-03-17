using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер экрана с интро
/// </summary>
public class IntroController : IController {

    public static IntroController controller;
    public float time;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    //он просто ждёт time секунд и включает другой экран
	void Update () {
        time -= Time.deltaTime;
        if(time<0)
        {
            MainController.controller.GoToScreen(MenuController.controller, 2f);
        }
	}

}
