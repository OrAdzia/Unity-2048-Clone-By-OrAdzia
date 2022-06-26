using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TileSettings tileSettings;

    private int _value = 2;
    private float count;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isAnimating;
    private Tile mergeTile;
    private Animator animator;
    private TileManager tileManager;
    private Image tileImage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        tileManager = FindObjectOfType<TileManager>();
        tileImage = GetComponent<Image>();
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
                int newValue = _value + mergeTile._value;
                tileManager.AddScore(newValue);
                SetValue(newValue);
                Destroy(mergeTile.gameObject);
                animator.SetTrigger("Merge");
                mergeTile = null;
            }
        }
    }

    public void SetValue(int value)
    {
        _value = value;
        text.text = value.ToString();
        TileColor newColor = tileSettings.TileColors.FirstOrDefault(color => color.value == _value) ?? new TileColor();
        text.color = newColor.fgColor;
        tileImage.color = newColor.bgColor;
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
        if (this._value != otherTile._value)
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
        return _value;
    }
}