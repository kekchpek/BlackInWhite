using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Кнопка в экране игры
/// </summary>
public class GameButton : InteractableObject {

    public string function;

    public override void Interact()
    {
        base.Interact();
        GameController.controller.CallFunction(function);
    }

}
