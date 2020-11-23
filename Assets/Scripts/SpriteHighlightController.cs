using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHighlightController : MonoBehaviour
{
    [Header("Specify in Editor")]

    public Color defaultColor = Color.white;
    public Color highlightColor = Color.white;
    public Color highlightColor2 = Color.white;

    [Header("Don't Modify")]

    public SpriteRenderer cachedSpriteRenderer;
    public bool highlighted;
    public float highlightEndTime;

    void Start()
    {
        cachedSpriteRenderer = this.GetComponent<SpriteRenderer>();
        cachedSpriteRenderer.color = defaultColor;
    }

    void Update()
    {
        if (highlighted && Time.time >= highlightEndTime)
        {
            cachedSpriteRenderer.color = defaultColor;
            highlighted = false;
        }
    }

    public void highlight(float duration)
    {
        cachedSpriteRenderer.color = highlightColor;
        highlighted = true;
        highlightEndTime = Mathf.Max(highlightEndTime, Time.time + duration);
    }

    public void highlight2(float duration)
    {
        cachedSpriteRenderer.color = highlightColor2;
        highlighted = true;
        highlightEndTime = Mathf.Max(highlightEndTime, Time.time + duration);
    }
}
