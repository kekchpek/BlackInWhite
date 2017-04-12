using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Контроллер, управляющий экраном игры
/// </summary>
public class GameController : IController {
    
    public Transform[] CubeSpawns;
    public GameButton right, left;
    public float speed;
    public float startSpeed;
    public float acceleration;
    public float waitStart;
    public float minDist, maxDist;
    public float startDoubleChance;
    public float accelerationDoubleChance;
    public float maxDoubleChance;
    public GameObject cubePrefab;
    public GameObject pauseObj;
    public bool paused;
    public bool ended;
    public Text pointsText;
    public Text tPoints, tBest;
    private float doubleChance;
    float needDist, dist; 
    float startTime;
    bool started;
    public static GameController controller;
    public int points;
    private List<GameObject> cubes = new List<GameObject>();
    public GameObject leftButton, rightButton;
    bool isLeftPressed = false, isRightPressed = false;
    public float endTime;

    public AudioSource leftBttnAudio, rightBttnAudio;
    public AudioClip[] upAudios, downAudios;

    /// <summary>
    /// Удаление падающего куба куба
    /// </summary>
    /// <param name="obj"> куб</param>
    public void RemoveCube(GameObject obj)
    {
        cubes.Remove(obj);
    }

    void Start()
    {
        downAudios = MainController.controller.buttonAudiosDown;
        upAudios = MainController.controller.buttonAudiosUp;
        leftBttnAudio = MainController.controller.audioSources.AddComponent<AudioSource>();
        rightBttnAudio = MainController.controller.audioSources.AddComponent<AudioSource>();
        leftBttnAudio.volume = rightBttnAudio.volume = 0.27f;
        rightBttnAudio.playOnAwake = leftBttnAudio.playOnAwake = false;
    }

    public override void GameLoadInitialization()
    {
        if (controller == null)
            controller = this;
    }

	public override void Init () {
        //"обнуление" экрана, возвращение его в первозданный вид
        foreach (GameObject obj in cubes)
        {
            Destroy(obj);
        }
        cubes.Clear();
        speed = startSpeed;
        dist = 0;
        startTime = waitStart;
        started = false;
        doubleChance = startDoubleChance;
        points = 0;
        pointsText.text = "0";
        Resume();
        ended = false;
        if (isRightPressed)
            PressRightButton();
        if (isLeftPressed)
            PressLeftButton();
	}
	

	void Update () {
        if (!paused && !ended)
        {
            if (started)
            {
                //генерация падающих кубов
                dist -= speed * Time.deltaTime;
                speed += acceleration * Time.deltaTime;
                if (doubleChance < maxDoubleChance)
                    doubleChance += accelerationDoubleChance * Time.deltaTime;
                if (dist <= 0)
                {
                    dist = Random.Range(minDist, maxDist);
                    if (Random.Range(0f, 1f) > doubleChance)
                    {
                        int rand = Random.Range(0, 2);
                        Vector3 pos = CubeSpawns[rand].position;
                        GameObject obj = (GameObject)Instantiate(cubePrefab, pos, Quaternion.identity);
                        obj.transform.SetParent(transform);
                        cubes.Add(obj);
                        if (Random.Range(0, 2) == 1)
                            obj.GetComponent<Cube>().SetBlack();
                        else
                            obj.GetComponent<Cube>().SetWhite();
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector3 pos = CubeSpawns[i].position;
                            GameObject obj = (GameObject)Instantiate(cubePrefab, pos, Quaternion.identity);
                            obj.transform.SetParent(transform);
                            cubes.Add(obj);
                            if (Random.Range(0, 2) == 1)
                                obj.GetComponent<Cube>().SetBlack();
                            else
                                obj.GetComponent<Cube>().SetWhite();
                        }
                    }
                }
            }
            else
            {
                //задержка перед падением первого куба(не используется)
                startTime -= Time.deltaTime;
                if (startTime <= 0)
                    started = true;
            }
        }
	}

    /// <summary>
    /// Включить паузу
    /// </summary>
    public void Pause()
    {
        MainController.controller.inputTime = false;
        paused = true;
        BuildPause();
        pauseObj.SetActive(true);
    }

    /// <summary>
    /// Выключить паузу
    /// </summary>
    public void Resume()
    {
        paused = false;
        pauseObj.SetActive(false);
        MainController.controller.SetInputTime(true);
    }
    

    /// <summary>
    /// Конец игры
    /// </summary>
    public void End()
    {
        MainController.controller.SetInputTime(false);
        MainController.controller.loopMusicOn = false;
        MainController.controller.loopAudio.Stop();
        ended = true;
        MainController.controller.GoToScreen(PostMenuController.controller, 1.5f, 0.5f);
        foreach(GameObject c in cubes)
        {
            if(c != null)
                c.GetComponent<Cube>().Disapear(0.5f);
        }
    }


    /// <summary>
    /// Добавить одно очко
    /// </summary>
    public void AddPoint()
    {
        points++;
        pointsText.text = points.ToString();
        if (points > 200)
            MainController.controller.SetLoopLevelAudio(3);
        else if (points > 70)
            MainController.controller.SetLoopLevelAudio(2);
        else if (points > 30)
            MainController.controller.SetLoopLevelAudio(1);
    }

    /// <summary>
    /// Построение экрана паузы
    /// </summary>
    void BuildPause()
    {
        tPoints.text = points.ToString();
        tBest.text = "BEST:  " + MainController.controller.bestPoints.ToString();
    }

    /// <summary>
    /// Нажатие на левую кнопку
    /// </summary>
    void PressLeftButton()
    {
        leftButton.transform.position = new Vector3(leftButton.transform.position.x, leftButton.transform.position.y, -leftButton.transform.position.z);
        left.currentPosition = new Vector3(left.currentPosition.x, left.currentPosition.y, -left.currentPosition.z);
        leftBttnAudio.clip = downAudios[Random.Range(0, downAudios.Length)];
        leftBttnAudio.Play();
        isLeftPressed = !isLeftPressed;
    }

    /// <summary>
    /// Нажатие на правую кнопку
    /// </summary>
    void PressRightButton()
    {
        rightButton.transform.position = new Vector3(rightButton.transform.position.x, rightButton.transform.position.y, -rightButton.transform.position.z);
        right.currentPosition = new Vector3(right.currentPosition.x, right.currentPosition.y, -right.currentPosition.z);
        rightBttnAudio.clip = upAudios[Random.Range(0, upAudios.Length)];
        rightBttnAudio.Play();
        isRightPressed = !isRightPressed;
    }


    /// <summary>
    /// Вызов определённой функции(для кнопок)
    /// </summary>
    /// <param name="s"> название функции</param>
    public void CallFunction(string s)
    {
        if (s == "PressLeftButton")
        {
            PressLeftButton();
        }
        if (s == "PressRightButton")
        {
            PressRightButton();
        }
        if (s == "Pause")
        {
            Pause();
        }
    }
}
