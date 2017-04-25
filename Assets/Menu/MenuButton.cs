using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Кнопка в меню игры
/// </summary>
public class MenuButton : InteractableObject {


    public AudioSource audioSource;
    public AudioClip[] downAudios, upAudios;

    public Vector3 posUp, posDown;//позиции в нажатом и отжатом состоянии
    public string function;//функция которую выплняет кнопка
    Material mat;//кэшированый материал
    public Texture textureUp, textureDown;//текстуры в нажатом и отжатом состоянии
    public bool onIt;//нажата ли кнопка пальцем
    public bool pressed;//находит ся ли кнопка в нажатом состоянии
    bool pressFlag;//нажата ли кнопка(не важно пальцем или в нажатом состоянии
    public BoxCollider boxCollider;//коллайдер кнопки
    
    void Start()
    {
        pressFlag = false;
        boxCollider = GetComponent<BoxCollider>();
        downAudios = MainController.controller.buttonAudiosDown;
        upAudios = MainController.controller.buttonAudiosUp;
        audioSource = MainController.controller.audioSources.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.27f;
        if (mat == null)
        {
            //клонируем материал, чттобы мы могли его спокойно изменять во время игры
            mat = new Material(transform.GetChild(0).GetComponent<MeshRenderer>().material);
            transform.GetChild(0).GetComponent<MeshRenderer>().material = mat;
        }
    }

    /// <summary>
    /// Задаёт текстуру в ненажатом положении
    /// </summary>
    /// <param name="t"></param>
    public void SetTextureUp(Texture t)
    {
        if(mat == null)
        {
            mat = new Material(transform.GetChild(0).GetComponent<MeshRenderer>().material);
            transform.GetChild(0).GetComponent<MeshRenderer>().material = mat;
        }
        textureUp = t;
        if(transform.position == posUp)
        {
            mat.SetTexture("_MainTex", textureUp);
        }
    }


    /// <summary>
    /// Вызов функционала кнопки
    /// </summary>
    public override void Interact()
    {
        base.Interact();
        if(!pressed)
            audioSource.clip = downAudios[Random.Range(0, downAudios.Length)];
        else
            audioSource.clip = upAudios[Random.Range(0, upAudios.Length)];
        audioSource.Play();
        if (MenuController.controller.gameObject.activeSelf)
        {
            MenuController.controller.CallFunction(function);
        }
        else
        {
            PostMenuController.controller.CallFunction(function);
        }
    }

    /// <summary>
    /// Нажатие на кнопку вез вызова функционала(спадает на следующий кадр)
    /// </summary>
    public override void PreInteract()
    {
        base.PreInteract();
        onIt = true;
    }

    /// <summary>
    /// "Вдавливание" кнопки
    /// </summary>
    public void Press()
    {
        if (!pressFlag && mat != null)
        {
            pressFlag = true;
            transform.position = posDown;
            boxCollider.center = new Vector3(0, 0, -0.5f);
            mat.SetTexture("_MainTex", textureDown);
        }
    }

    void Update()
    {
        if(!onIt && !pressed)
        {
            Unpress();
        }
        else
        {
            Press();
        }
        onIt = false;
    }

    /// <summary>
    /// "Выдавливание" кнопки
    /// </summary>
    public void Unpress()
    {
        if (mat != null && pressFlag)
        {
            pressFlag = false;
            boxCollider.center = new Vector3(0, 0, 0);
            transform.position = posUp;
            mat.SetTexture("_MainTex", textureUp);
        }
    }
}
