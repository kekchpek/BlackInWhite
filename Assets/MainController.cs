using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

/// <summary>
/// Главный контроллер, который управляет сменой экранов, инпутом и сохранением рекордов
/// </summary>
public class MainController : MonoBehaviour {

    public static MainController controller;//статическая ссылка на этот контроллер
    public GameObject audioSources;//объект со всеми аудиосоурсами в игре
    public AudioSource loopAudio;//аудиосоурс с фоновой музыкой

    public IController currentScreen, nextScreen;//текущий экран  и экран на который нужно перейдти
    public Image fadeImage;//картинка для затемнения экрана
    public int bestPoints;//регкорд
    public Camera currentCamera;//текущая активная камера
    public bool inputTime;//работает ли инпут
    public IController[] controllers;//массив всех контроллеров в игре
    InterstitialAd ad;//межстраничная реклама
    public int adTimer;//раз в сколько игр показывается реклама
    int adTime;//счётчик определяющий сколько игр осталось до показа рекламы
    string adHandle = "ca-app-pub-5377701829054453/9318814921";//хэндл рекламы
    float sTime, sTimer, sTimer2;//таймеры для перехода от одного экрана к другому
    public AudioClip[] levelLoopAudios, buttonAudiosDown, buttonAudiosUp;//аудиоклипы, которые используются в игре
    public AudioClip endAudio, catchAudio;//--||--
    public bool loopMusicOn;//включена ли фоновая музыка
    public bool soundOn { get { return audioSources.active; } set { audioSources.SetActive(value); loopAudio.enabled = value; } }//включён ли звук

    public void SetLoopLevelAudio(int lvl)//устанавливает фоновую музыку нужного уровня
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


    public void LoadAd(object sender, EventArgs args)//загружает рекламу
    {
        ad = new InterstitialAd("ca-app-pub-5377701829054453/9318814921");
        AdRequest request = new AdRequest.Builder().Build();
        ad.LoadAd(request);
    }

    public void ShowAd()//показывает рекламу
    {
        if (ad.IsLoaded())
        {
            ad.OnAdClosed += LoadAd;//следующая реклама загружается сразу после закрытия рекламы
            ad.Show();
        }
    }

    void OnApplicationFocus(bool hasFocus)//при сворачивании приложения ставит паузу если идёт игра
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

    void OnApplicationPause(bool pauseStatus)//при сворачивании приложения ставит паузу если идёт игра
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
#if UNITY_EDITOR//ввод мышкой
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
        foreach (IController c in controllers)//инициализация всех контроллеров
            c.GameLoadInitialization();
        if (controller == null)
            controller = this;
        GoToScreen(IntroController.controller, 0.2f, 1f);//сразу переходим к экрану с интро
        LoadRecords();//загружаем рекорд
        LoadAd(null, null);//сразу начинаем загружать рекламу
        adTime = adTimer;//обнуляем счётчик игр через которые появится реклама
        if (PlayerPrefs.GetInt("sound", 1) == 1)//включаем/выключаем звук
            soundOn = true;
        else
            soundOn = false;
    }

    /// <summary>
    /// Включение/отключение инпута
    /// </summary>
    public void SetInputTime(bool b)
    {
        StartCoroutine(SetInputTimeC(b));//инпут включается только в следующем кадре
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
    /// <param name="t">время, за которое проиходит фэйд</param>
    /// <param name="t2">время, за которое проиходит анфэйд</param>
    public void GoToScreen(IController controller, float t, float t2)
    {
        if (nextScreen == null)//если в данный момент не происходит свап
        {
            nextScreen = controller;
            if (nextScreen == GameController.controller || nextScreen == InfoController.controller)//некоторые экраны по задумке переключаютс белым фэйдом
            {
                fadeImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                fadeImage.color = new Color(0, 0, 0, 0);
            }
            sTimer = t;
            sTimer2 = t2;
            inputTime = false;
        }
    }

    public void GoToScreen(IController controller, float t)
    {
        if(t == 0)//мнгновенный переход к экрану
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
        if (nextScreen != null)//переход к следующему экрану
        {
            sTime += Time.deltaTime;
            float r, g, b, a;//вычисляем нужный цвер
            r = fadeImage.color.r;
            g = fadeImage.color.g;
            b = fadeImage.color.b;
            if (currentScreen != nextScreen)//фэйд или анфэйд происходит в данный момент
                a = sTime / sTimer;
            else
                a = 1 - (sTime / sTimer2);
            fadeImage.color = new Color(r, g, b, a);
            if (currentScreen != nextScreen)//переход от одного экрана к друггому
            {
                if (sTime > sTimer)//в момент когда экран полностью затемнён
                {
                    sTime -= sTimer;
                    if (currentScreen != null)
                        currentScreen.gameObject.SetActive(false);
                    nextScreen.Init();
                    nextScreen.gameObject.SetActive(true);
                    if (currentScreen == GameController.controller && GameController.controller.ended)//показываем рекламу если это конец игры
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
                if (sTime > sTimer2)//заканчиваем переход
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
