using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Котроллер экрана и обучением игре
/// </summary>
public class InfoController : IController {

    public static InfoController controller;

    IController backPoint;

    public GameObject[] pages;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public override void Init()
    {
        //запоминает в какой экран нужно вернуться
        base.Init();
        backPoint = MainController.controller.currentScreen;
    }

    /// <summary>
    /// Включает экран из которого сам был включён
    /// </summary>
    public void GoBack()
    {
        foreach(GameObject g in pages)
        {
            g.SetActive(false);
        }
        pages[0].SetActive(true);
        MainController.controller.GoToScreen(backPoint, 0);
    }
}
