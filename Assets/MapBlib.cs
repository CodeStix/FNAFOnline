using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class MapBlib : MonoBehaviour
{
    public bool hidden = false;
    public bool disabled = false;
    [Space]
    public Color[] disabledColors;
    public Color[] enabledColors;
    public Color[] hoverColors;
    [Space]
    public bool clickable = true;
    public UnityEvent onClick;
    [Space]
    public GameObject[] toEnableIfEnabled;

    public static bool globalDisable = false;

    private new SpriteRenderer renderer;
    private bool hovering = false;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        renderer.enabled = !hidden;

        foreach (GameObject obj in toEnableIfEnabled)
            obj.SetActive(!(hidden || disabled || globalDisable));

        if (hovering)
            return;

        if (disabled || globalDisable)
            renderer.color = disabledColors[Random.Range(0, disabledColors.Length)];
        else
            renderer.color = enabledColors[Random.Range(0, enabledColors.Length)];
    }

    void OnMouseOver()
    {
        if (hidden || disabled)
            return;

        hovering = true;
        renderer.color = hoverColors[Random.Range(0, hoverColors.Length)];
    }

    void OnMouseExit()
    {
        hovering = false;
    }

    void OnMouseDown()
    {
        if (clickable && !disabled && !hidden && !globalDisable)
        {
            onClick.Invoke();
        }
    }

    public void Disable()
    {
        disabled = true;
    }
}