using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Падающий куб
/// </summary>
public class Cube : MonoBehaviour {

    public bool isWhite;
    public float removeDist;
    private float dist;
    private float scale;
    bool profit;
    public Material whiteMaterial, blackMaterial;
    private MeshRenderer meshRenderer;

    /// <summary>
    /// Покрасить в чёрный
    /// </summary>
    public void SetBlack()
    {
        scale = transform.localScale.x;
        isWhite = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(blackMaterial);
    }

    /// <summary>
    /// Покрасить в белый
    /// </summary>
    public void SetWhite()
    {
        scale = transform.localScale.x;
        isWhite = true;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(whiteMaterial);
    }

    /// <summary>
    /// Куб пойман
    /// </summary>
    public void Profit()
    {
        if(!profit)
        {
            dist = 0;
            profit = true;
        }
    }

	void Update () {
        if (!GameController.controller.paused)
        {
            if (profit)
            {
                //удаление после ловли
                transform.position += Vector3.down * 2.5f * Time.deltaTime;
                dist += GameController.controller.speed * Time.deltaTime;
                if (dist < removeDist)
                {
                    gameObject.transform.localScale = Vector3.one * (1 - dist / removeDist) * scale;
                }
                else
                {
                    GameController.controller.RemoveCube(gameObject);
                    Destroy(gameObject);
                }
            }
            else
            {
                //падение
                transform.position += Vector3.down * GameController.controller.speed * Time.deltaTime;
            }
        }
	}
}
