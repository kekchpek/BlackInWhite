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
    public AudioClip endAudio;
    public AudioClip[] catchAudios;

    void Start()
    {
        endAudio = MainController.controller.endAudio;
        catchAudios = MainController.controller.buttonAudiosDown;
        audioSource = MainController.controller.audioSources.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.27f;
    }

    /// <summary>
    /// Ловля кубов
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter(Collider coll)
    {
        Cube cube = coll.GetComponent<Cube>();
        if (cube != null)
            if (!cube.profit)
            {
                if (isWhite == cube.isWhite)
                {
                    GameController.controller.AddPoint();
                    cube.Profit();
                    audioSource.clip = catchAudios[Random.Range(0, catchAudios.Length)];
                    audioSource.Play();
                }
                else
                {
                    cube.Wrong();
                    if(!GameController.controller.ended)
                    {
                        audioSource.clip = endAudio;
                        audioSource.Play();
                    }
                    GameController.controller.End();
                }
            }
    }

}
