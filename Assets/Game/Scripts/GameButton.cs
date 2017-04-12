using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Кнопка в экране игры
/// </summary>
public class GameButton : InteractableObject {

    public string function;
    public Vector3 currentPosition;
    public float speed;

    void Start()
    {
        currentPosition = transform.position;
    }

    void Update()
    {
        if(currentPosition != transform.position)
        {
            if (Vector3.Distance(transform.position, currentPosition) < speed)
                transform.position = currentPosition;
            else
                transform.position += (currentPosition - transform.position).normalized * speed;
        }
    }

    public override void Interact()
    {
        base.Interact();
        GameController.controller.CallFunction(function);
    }

}
