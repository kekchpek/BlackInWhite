using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Котроллер экрана и обучением игре
/// </summary>
public class InfoController : IController {

    public static InfoController controller;
    public bool swapping, swapped;
    private int currentPage;
    public const float fadeTime = 0.15f;
    private float currentFadeTime;
    public Image fadeImage;

    IController backPoint;

    public GameObject[] pages;

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

    public void Next()
    {
        if (!swapping)
        {
            currentPage++;
            swapping = true;
            currentFadeTime = 0f;
        }
    }

    void Update()
    {
        if(swapping)
        {
            currentFadeTime += Time.deltaTime;
            float r, g, b, a;
            r = fadeImage.color.r;
            g = fadeImage.color.g;
            b = fadeImage.color.b;
            if (swapped)
                a = 1 - currentFadeTime / fadeTime;
            else
                a = currentFadeTime / fadeTime;
            fadeImage.color = new Color(r, g, b, a);
            if (currentFadeTime > fadeTime)
            {
                if (swapped)
                {
                    swapping = false;
                    swapped = false;
                }
                else
                {
                    pages[currentPage].SetActive(true);
                    pages[currentPage - 1].SetActive(false);
                    swapped = true;
                    currentFadeTime = 0f;
                }
            }
        }
    }

    public override void Init()
    {
        //запоминает в какой экран нужно вернуться
        base.Init();
        backPoint = MainController.controller.currentScreen;
        currentPage = 0;
        foreach (GameObject g in pages)
        {
            g.SetActive(false);
        }
        pages[0].SetActive(true);
    }

    /// <summary>
    /// Включает экран из которого сам был включён
    /// </summary>
    public void GoBack()
    {
        MainController.controller.GoToScreen(backPoint, 0.3f);
    }
}
