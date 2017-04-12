using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Падающий куб
/// </summary>
public class Cube : MonoBehaviour {

    public bool isWhite;
    bool inited = false;
    private float scale;
    bool isRed = false;
    public bool profit;
    bool wrong;
    public Material whiteMaterial, blackMaterial, redMaterial;
    private MeshRenderer meshRenderer;
    public float fadeTime;
    float curretnFadeTime;
    public float removeDist;
    float dist;
    bool disapeared;

    /// <summary>
    /// Покрасить в чёрный
    /// </summary>
    public void SetBlack()
    {
        scale = transform.localScale.x;
        isWhite = false;
        isRed = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(blackMaterial);
        if (!inited)
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0);
            inited = true;
        }
    }

    /// <summary>
    /// Покрасить в белый
    /// </summary>
    public void SetWhite()
    {
        scale = transform.localScale.x;
        isWhite = true;
        isRed = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(whiteMaterial);
        if (!inited)
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0);
            inited = true;
        }
    }

    void SetRed()
    {
        isRed = true;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(redMaterial);
    }

    /// <summary>
    /// Куб пойман
    /// </summary>
    public void Profit()
    {
        if(!profit)
        {
            profit = true;
            curretnFadeTime = 0;
        }
    }

    public void Disapear(float t)
    {
        if (!disapeared)
        {
            curretnFadeTime = 0;
            disapeared = true;
            fadeTime = t;
        }
    }

    public void Wrong()
    {
        if(!wrong)
        {
            wrong = true;
            Disapear(1f);
            SetRed();
        }
    }

	void Update () {
        if (!GameController.controller.paused)
        {
            if (!disapeared)
            {
                if (profit)
                {
                    //удаление после ловли
                    transform.position += Vector3.down * 2.5f * Time.deltaTime;
                    dist += Time.deltaTime * 2.5f;
                    if (dist < removeDist)
                        transform.localScale = Vector3.one * scale * (1 - dist / removeDist);
                    else
                        Destroy(gameObject);
                }
                else
                {
                    //падение
                    if (fadeTime > curretnFadeTime)
                    {
                        curretnFadeTime += Time.deltaTime;
                        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, curretnFadeTime / fadeTime);
                    }
                    else
                    {
                        curretnFadeTime = fadeTime;
                        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, curretnFadeTime / fadeTime);
                    }
                    transform.position += Vector3.down * GameController.controller.speed * Time.deltaTime;
                }
            }
            else
            {
                curretnFadeTime += Time.deltaTime;
                if (curretnFadeTime < fadeTime)
                {
                    meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 1 - curretnFadeTime / fadeTime);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
