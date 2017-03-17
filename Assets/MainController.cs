using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Главный контроллер, который управляет сменой экранов, инпутом и сохранением рекордов
/// </summary>
public class MainController : MonoBehaviour {

    public static MainController controller;

    public IController currentScreen, nextScreen;
    public Image shader;
    public int bestPoints;
    public Camera currentCamera;
    public bool inputTime;
    public IController[] controllers;

    float sTime, sTimer;

    /// <summary>
    /// Функция обрабатывающая инпут
    /// </summary>
    void InputController()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonUp(0))
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
                    if(!hit.collider.GetComponent<InteractableObject>().interacted)
                        hit.collider.GetComponent<InteractableObject>().PreInteract();
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
    public void GoToScreen(IController controller, float t)
    {
        if (nextScreen == null)
        {
            nextScreen = controller;
            sTimer = t;
            inputTime = false;
        }
    }

	void Update () {
        //инпут
        if (inputTime)
        {
            InputController();
        }
        //переход к следующему экрану
        if (nextScreen!=null)
        {
            sTime += Time.deltaTime;
            float r, g, b, a;
            r = shader.color.r;
            g = shader.color.g;
            b = shader.color.b;
            if (currentScreen != nextScreen)
                a = sTime * 2 / sTimer;
            else
                a = 1 - (sTime * 2 / sTimer);
            shader.color = new Color(r, g, b, a);
            if(sTime * 2 > sTimer)
            {
                if (currentScreen != nextScreen)
                {
                    sTime -= sTimer;
                    if (currentScreen != null)
                        currentScreen.gameObject.SetActive(false);
                    nextScreen.Init();
                    nextScreen.gameObject.SetActive(true);
                    currentScreen = nextScreen;
                    currentCamera = currentScreen.cam;
                }
                else
                {
                    sTimer = 0;
                    sTime = 0;
                    nextScreen = null;
                    if(currentScreen != InfoController.controller)
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/records.ko");
        bf.Serialize(file, bestPoints);
        file.Close();
    }

    /// <summary>
    /// Загрузка рекорда
    /// </summary>
    void LoadRecords()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/records.ko"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/records.ko", FileMode.Open);
            bestPoints = (int)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            SaveRocords(0);
        }
    }

}
