using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private int numValue = 2;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isAnimating;
    private float count;
    private float animationTime = 0.3f;


    public void SetValue(int value)
    {
        numValue = value;
        text.text = value.ToString();
    }
}