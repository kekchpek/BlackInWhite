using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Падающий куб
/// </summary>
public class Cube : MonoBehaviour {

    public bool isWhite;//логический чвет
    bool inited = false;//проинициализирован
    private float scale;//размер
    bool isRed = false;//покраснел ли(при этом логический цвет не меняется)
    public bool profit;//отмечен как пойманый
    bool wrong;//отмечен как неправильно пойманый
    public Material whiteMaterial, blackMaterial, redMaterial;//кэшируем материалы
    private MeshRenderer meshRenderer;//кэшируем меш
    public float fadeTime;//время за которое исчезает(фэйдом)
    float curretnFadeTime;//время которое прошло с момента начала исчезновения(фэйдом)
    public float removeDist;//дистанция пройдя которую кубик исчезнет(уменьшением)
    float dist;//дистанция которую кубик прошол с начала момента исчезновения(уменьшением)
    bool disapeared;//исчезает ли в данный момент(фэйдом)

    /// <summary>
    /// Покрасить в чёрный
    /// </summary>
    public void SetBlack()
    {
        scale = transform.localScale.x;
        isWhite = false;
        isRed = false;
        if (meshRenderer == null)//кэшируем меш
            meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(blackMaterial);
        if (!inited)//если не инициализирован то задать начальный цвет полностью прозрачным
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
        if (meshRenderer == null)//кэшируем меш
            meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(whiteMaterial);
        if (!inited)//если не инициализирован то задать начальный цвет полностью прозрачным
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0);
            inited = true;
        }
    }

    void SetRed()
    {
        isRed = true;
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
        }
    }

    /// <summary>
    /// Исчезновение(фэйдом)
    /// </summary>
    /// <param name="t">время исчезновения</param>
    public void Disapear(float t)
    {
        if (!disapeared)
        {
            curretnFadeTime = 0;
            disapeared = true;
            fadeTime = t;
        }
    }

    /// <summary>
    /// Отметить как неправильно пойманый
    /// </summary>
    public void Wrong()
    {
        if(!wrong)
        {
            wrong = true;
            //исчезает на 0.5сек больше чем остальные
            Disapear(1f);
            SetRed();//становится крсным
        }
    }

	void Update () {
        if (!GameController.controller.paused)//если игран не на паузе
        {
            if (!disapeared)//если не исчезает(фэйдом)
            {
                if (profit)//если пойман
                {
                    //исчезновение(уменьшением)
                    transform.position += Vector3.down * 2.5f * Time.deltaTime;//при этом скорость падения фиксировано мала
                    dist += Time.deltaTime * 2.5f;
                    if (dist < removeDist)
                        transform.localScale = Vector3.one * scale * (1 - dist / removeDist);
                    else
                        Destroy(gameObject);
                }
                else
                {
                    //появление(фэйдом)
                    #region fadeBecome
                    if (fadeTime > curretnFadeTime)//изначально currentFadeTime равен нулю и если он не исчезает то появляется
                    {
                        curretnFadeTime += Time.deltaTime;
                        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, curretnFadeTime / fadeTime);
                    }
                    else
                    {
                        curretnFadeTime = fadeTime;
                        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, curretnFadeTime / fadeTime);
                    }
                    #endregion
                    transform.position += Vector3.down * GameController.controller.speed * Time.deltaTime;//падение
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
