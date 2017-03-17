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

    /// <summary>
    /// Ловля кубов
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter(Collider coll)
    {
        Cube cube = coll.GetComponent<Cube>();
        if(cube!= null)
        {
            if(isWhite == cube.isWhite)
            {
                GameController.controller.AddPoint();
                cube.Profit();
            }
            else
            {
                GameController.controller.End();
            }
        }
    }

}
