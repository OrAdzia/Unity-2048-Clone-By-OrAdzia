using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TileSettings tileSettings;

    private int numValue = 2;
    private float count;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isAnimating;

    private void Update()
    {
        if (!isAnimating)
        {
            return;
        }

        count += Time.deltaTime;

        float t = count / tileSettings.animationTime;
        t = tileSettings.animationCurve.Evaluate(t);

        Vector3 newPos = Vector3.Lerp(startPos, endPos, t);

        transform.position = newPos;

        if (count >= tileSettings.animationTime)
        {
            isAnimating = false;
        }
    }

    public void SetValue(int value)
    {
        numValue = value;
        text.text = value.ToString();
    }

    public void SetPosition(Vector3 newPos, bool instant)
    {
        if (instant)
        {
            transform.position = newPos;
            return;
        }

        startPos = transform.position;
        endPos = newPos;
        count = 0;
        isAnimating = true;
    }
}