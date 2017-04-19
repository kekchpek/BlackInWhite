using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сущность, которая ловит кубы
/// </summary>
public class Catcher : MonoBehaviour {

    /// <summary>
    /// Цвет Catсher'a
    /// </summary>
    public bool isWhite;


    public AudioSource audioSource;
    public AudioClip endAudio;//звук при конце игры
    public AudioClip[] catchAudios;//звук при правильно пойманом кубике

    void Start()
    {
        //настраиваем звук
        #region soundSet
        endAudio = MainController.controller.endAudio;
        catchAudios = MainController.controller.buttonAudiosDown;
        audioSource = MainController.controller.audioSources.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        #endregion soundSet
    }

    /// <summary>
    /// Ловля кубов
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter(Collider coll)
    {
        Cube cube = coll.GetComponent<Cube>();
        if (cube != null)
            if (!cube.profit)//если куб ещё не пойман
            {
                if (isWhite == cube.isWhite)//если цвет совпа
                {
                    GameController.controller.AddPoint();
                    cube.Profit();//отметить куб как пойманый
                    #region playSound
                    audioSource.volume = 0.27f;
                    audioSource.clip = catchAudios[Random.Range(0, catchAudios.Length)];
                    audioSource.Play();
                    #endregion playSound
                }
                else
                {
                    cube.Wrong();//отметить куб как неправильно пойманый
                    #region playSound
                    if(!GameController.controller.ended)
                    {
                        audioSource.volume = 0.5f;
                        audioSource.clip = endAudio;
                        audioSource.Play();
                    }
                    #endregion playSound
                    GameController.controller.End();
                }
            }
    }

}
