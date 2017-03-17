using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Объект, который реагирует на нажатие
/// </summary>
public class InteractableObject : MonoBehaviour {

    public bool interacted;

    /// <summary>
    /// Нажатие
    /// </summary>
    public virtual void Interact()
    {

    }

    /// <summary>
    /// Наведение
    /// </summary>
    public virtual void PreInteract()
    {

    }
}
