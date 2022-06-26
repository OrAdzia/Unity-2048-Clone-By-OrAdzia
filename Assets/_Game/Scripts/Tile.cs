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
    private Tile mergeTile;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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
            
            if (mergeTile != null)
            {
                animator.SetTrigger("Merge");
                SetValue(numValue + mergeTile.numValue);
                Destroy(mergeTile.gameObject);
                mergeTile = null;
            }
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

        if (mergeTile != null)
        {
            mergeTile.SetPosition(newPos, false);
        }
    }

    public bool Merge(Tile otherTile)
    {
        if (this.numValue != otherTile.numValue)
        {
            return false;
        }
        if (mergeTile != null || otherTile.mergeTile != null)
        {
            return false;
        }

        mergeTile = otherTile;

        return true;
    }

    public int GetValue()
    {
        return numValue;
    }
}