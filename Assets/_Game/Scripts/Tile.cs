using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int numValue = 2;
    [SerializeField] private TMP_Text text;

    public void SetValue(int value)
    {
        numValue = value;
        text.text = value.ToString();
    }
}