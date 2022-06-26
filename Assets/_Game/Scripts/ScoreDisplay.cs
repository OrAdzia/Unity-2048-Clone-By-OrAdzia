using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text text;
    private Animator animator;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
    }

    public void UpdateScore(int score) 
    {
        text.text = score.ToString();
        animator.SetTrigger("ScoreUpdated");
    }
}