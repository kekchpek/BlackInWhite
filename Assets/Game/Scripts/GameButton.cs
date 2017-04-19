using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Кнопка в экране игры
/// </summary>
public class GameButton : InteractableObject {

    public string function;//строка, которая говорит контроллеру, какую функцию выполняет кнопка
    public Vector3 currentPosition;//позиция на которой должна находится кнопка в текущем положении(нажата/не нажата)
    public float speed;//скорость с которой наживается/отжимается кнопка

    void Start()
    {
        currentPosition = transform.position;//инициируем позицию
    }

    void Update()
    {
        if(currentPosition != transform.position)//если позиция кнопки не соответствует нужной в этом состоянии
        {
            //движение кнопки к нужной позиции
            #region
            if (Vector3.Distance(transform.position, currentPosition) < speed)
                transform.position = currentPosition;
            else
                transform.position += (currentPosition - transform.position).normalized * speed;
            #endregion
        }
    }

    public override void Interact()
    {
        base.Interact();
        GameController.controller.CallFunction(function);//вызываем функцию в контроллере
    }

}
