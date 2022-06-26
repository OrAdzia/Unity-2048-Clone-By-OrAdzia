using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCounter : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void UpdateCount(int moveCount)
    {
        bool shouldDisplayPlural = moveCount != 1;
        text.text = $"{moveCount} {(shouldDisplayPlural ? "moves" : "move")}";
    }
}
