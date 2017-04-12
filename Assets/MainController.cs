using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

/// <summary>
/// Главный контроллер, который управляет сменой экранов, инпутом и сохранением рекордов
/// </summary>
public class MainController : MonoBehaviour {

    public static MainController controller;
    public GameObject audioSources;
    public AudioSource loopAudio;

    public IController currentScreen, nextScreen;
    public Image fadeImage;
    public int bestPoints;
    public Camera currentCamera;
    public bool inputTime;
    public IController[] controllers;
    InterstitialAd ad;
    public int adTimer;
    int adTime;
    string adHandle = "ca-app-pub-5377701829054453/9318814921";
    float sTime, sTimer, sTimer2;
    public AudioClip[] levelLoopAudios, buttonAudiosDown, buttonAudiosUp;
    public AudioClip endAudio, catchAudio;
    public bool loopMusicOn;
    public bool soundOn { get { return audioSources.active; } set { audioSources.SetActive(value); loopAudio.enabled = value; } }

    public void SetLoopLevelAudio(int lvl)
    {
        loopMusicOn = true;
        if(lvl > levelLoopAudios.Length)
        {
            lvl = levelLoopAudios.Length - 1;
        }
        if (loopAudio.clip != levelLoopAudios[lvl])
        {
            loopAudio.clip = levelLoopAudios[lvl];
            loopAudio.Stop();
            loopAudio.Play();
        }
    }


    public void LoadAd(object sender, EventArgs args)
    {
        ad = new InterstitialAd("ca-app-pub-5377701829054453/9318814921");
        AdRequest request = new AdRequest.Builder().Build();
        ad.LoadAd(request);
    }

    public void ShowAd()
    {
        if (ad.IsLoaded())
        {
            ad.OnAdClosed += LoadAd;
            ad.Show();
        }
    }

    void OnApplicationPause()
    {
        if(currentScreen == GameController.controller && GameController.controller != null)
        {
            if(!GameController.controller.paused)
            {
                GameController.controller.Pause();
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (currentScreen == GameController.controller && GameController.controller != null)
            {
                if (!GameController.controller.paused)
                {
                    GameController.controller.Pause();
                }
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (currentScreen == GameController.controller && GameController.controller != null)
            {
                if (!GameController.controller.paused)
                {
                    GameController.controller.Pause();
                }
            }
        }
    }

    /// <summary>
    /// Функция обрабатывающая инпут
    /// </summary>
    void InputController()
    {
        if (inputTime)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                Ray r = currentCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                var layer = 1 << 8;
                if (Physics.Raycast(r, out hit, Mathf.Infinity, layer))
                {
                    hit.collider.GetComponent<InteractableObject>().Interact();
                }
            }
#endif
            if (Input.touchCount > 0)
            {
                Ray r = currentCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                var layer = 1 << 8;
                if (Physics.Raycast(r, out hit, Mathf.Infinity, layer))
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        hit.collider.GetComponent<InteractableObject>().Interact();
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        if (!hit.collider.GetComponent<InteractableObject>().interacted)
                            hit.collider.GetComponent<InteractableObject>().PreInteract();
                    }
                }
            }
        }
        if (nextScreen == null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(currentScreen == InfoController.controller)
                {
                    InfoController.controller.GoBack();
                }
                if(currentScreen == GameController.controller)
                {
                    if(GameController.controller.paused)
                    {
                        GoToScreen(MenuController.controller, 0.3f);
                    }
                    else
                    {
                        GameController.controller.Pause();
                    }
                }
                if(currentScreen == PostMenuController.controller)
                {
                    if (PostMenuController.controller.facePressed)
                    {
                        PostMenuController.controller.CallFunction("developers");
                    }
                    else
                    {
                        GoToScreen(MenuController.controller, 0.5f);
                    }
                }
                if(currentScreen == MenuController.controller)
                {
                    if (MenuController.controller.facePressed)
                    {
                        MenuController.controller.CallFunction("developers");
                    }
                    else
                    {
                        Application.Quit();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Инициализация игры
    /// </summary>
    void Start () {
        foreach (IController c in controllers)
            c.GameLoadInitialization();
        if (controller == null)
            controller = this;
        GoToScreen(IntroController.controller, 2f);
        LoadRecords();
        LoadAd(null, null);
        adTime = adTimer;
        if (PlayerPrefs.GetInt("sound", 1) == 1)
            soundOn = true;
        else
            soundOn = false;
    }

    /// <summary>
    /// Включение/отключение инпута
    /// </summary>
    public void SetInputTime(bool b)
    {
        StartCoroutine(SetInputTimeC(b));
    }

    IEnumerator SetInputTimeC(bool b)
    {
        yield return null;
        inputTime = b;
    }

    /// <summary>
    /// Переход к указанному экрану с помощью затемнения экрана за t секунд
    /// </summary>
    /// <param name="controller"> контролеер экрана, который хотим включить</param>
    /// <param name="t">время, за которое проиходит переход к экрану</param>
    public void GoToScreen(IController controller, float t, float t2)
    {
        if (nextScreen == null)
        {
            nextScreen = controller;
            if (nextScreen == GameController.controller || nextScreen == InfoController.controller)
            {
                fadeImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                fadeImage.color = new Color(0, 0, 0, 1);
            }
            sTimer = t;
            sTimer2 = t2;
            inputTime = false;
        }
    }

    public void GoToScreen(IController controller, float t)
    {
        if(t == 0)
        {
            nextScreen = controller;
            currentScreen.gameObject.SetActive(false);
            nextScreen.Init();
            nextScreen.gameObject.SetActive(true);
            currentScreen = nextScreen;
            nextScreen = null;
            currentCamera = currentScreen.cam;
            if (currentScreen != InfoController.controller)
                inputTime = true;
            return;
        }
        GoToScreen(controller, t/2f, t/2f);
    }

    void Update()
    {
        if (!loopAudio.isPlaying && loopMusicOn)
            loopAudio.Play();
        //инпут
        InputController();
        //переход к следующему экрану
        if (nextScreen != null)
        {
            sTime += Time.deltaTime;
            float r, g, b, a;
            r = fadeImage.color.r;
            g = fadeImage.color.g;
            b = fadeImage.color.b;
            if (currentScreen != nextScreen)
                a = sTime / sTimer;
            else
                a = 1 - (sTime / sTimer2);
            fadeImage.color = new Color(r, g, b, a);
            if (currentScreen != nextScreen)
            {
                if (sTime > sTimer)
                {
                    sTime -= sTimer;
                    if (currentScreen != null)
                        currentScreen.gameObject.SetActive(false);
                    nextScreen.Init();
                    nextScreen.gameObject.SetActive(true);
                    if (currentScreen == GameController.controller)
                    {
                        if (adTime <= 0)
                        {
                            adTime = adTimer;
                            ShowAd();
                        }
                        else
                        {
                            adTime--;
                        }
                    }
                    currentScreen = nextScreen;
                    currentCamera = currentScreen.cam;

                }
            }
            else
            {
                if (sTime > sTimer2)
                {
                    sTimer = 0;
                    sTime = 0;
                    nextScreen = null;
                    if (currentScreen != InfoController.controller)
                        inputTime = true;
                }
            }
        }
    }

    /// <summary>
    /// Сохранение рекорда
    /// </summary>
    /// <param name="p"> количество очков</param>
    public void SaveRocords(int p)
    {
        bestPoints = p;
        PlayerPrefs.SetInt("record", p);
    }

    /// <summary>
    /// Загрузка рекорда
    /// </summary>
    void LoadRecords()
    {
        bestPoints = PlayerPrefs.GetInt("record");
    }

}
