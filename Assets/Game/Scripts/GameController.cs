using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Контроллер, управляющий экраном игры
/// </summary>
public class GameController : IController {
    
    public Transform[] CubeSpawns;//спауны кубов
    public GameButton right, left;//кнопки
    public float speed;//скорость с которой падают кубы
    public float startSpeed;//начальная скорость падения кубов
    public float acceleration;//ускорения кубов
    public float minDist, maxDist;//минимальное и максимаьное значение дистанции после прохождения которой начинает падать следующий куб
                                  //(расстояние между падающими кубами)
    public float startDoubleChance;//начальный шанс генерации сразу вух кубов
    private float doubleChance;//шанс генерации сразу двух кубов
    public float accelerationDoubleChance;//увеличение шанса подения двух кубов со временем
    public float maxDoubleChance;//максимальный шанс падения двух кубов
    public GameObject cubePrefab;//префаб куба
    public GameObject pauseObj;//объект-пауза
    public bool paused;//на паузе ли игра
    public bool ended;//закончилась ли игра(пойман неправильный куб)
    public Text pointsText;//текст со счётом
    public Text tPoints, tBest;//текст со счётом и с рекордом соответственно на паузе
    float needDist;//неободимое расстояние пройденное последним созданым кубом для создание следующего
    float dist;//расстояние которое прошёл последний созданый куб
    public float waitStart;//сколько секунд проходит перед тем как упадёт первый куб
    float startTime;//время которое прошло с начала игры
    bool started;//начали ли падать кубы
    public int points;//кол-во очков
    private List<GameObject> cubes = new List<GameObject>();//все кубы, которые есть в игре на данный момент
    public GameObject leftButton, rightButton;//Catcher'ы кнопок
    bool isLeftPressed = false, isRightPressed = false;//флаги "нажатости" кнопок

    public AudioSource leftBttnAudio, rightBttnAudio;//аудиосоурсы кнопок
    public AudioClip[] leftAudios, rightAudios;//аудиоклипы кнопок

    public static GameController controller;//статическая переменная для доступа к контроллеру


    /// <summary>
    /// Удаление падающего куба из списка кубов в игре
    /// </summary>
    /// <param name="obj"> куб</param>
    public void RemoveCube(GameObject obj)
    {
        cubes.Remove(obj);
    }

    void Start()
    {
        //настройка аудио
        #region
        rightAudios = MainController.controller.buttonAudiosDown;
        leftAudios = MainController.controller.buttonAudiosUp;
        leftBttnAudio = MainController.controller.audioSources.AddComponent<AudioSource>();
        rightBttnAudio = MainController.controller.audioSources.AddComponent<AudioSource>();
        leftBttnAudio.volume = 0.27f;
        rightBttnAudio.volume = 0.19f;
        rightBttnAudio.playOnAwake = leftBttnAudio.playOnAwake = false;
        #endregion
    }

    public override void GameLoadInitialization()
    {
        //обеспечиваем доступ к контроллеру через статическую переменную
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
        leftBttnAudio.clip = leftAudios[Random.Range(0, rightAudios.Length)];
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
        rightBttnAudio.clip = rightAudios[Random.Range(0, leftAudios.Length)];
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
