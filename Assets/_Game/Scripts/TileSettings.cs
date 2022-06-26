using UnityEngine;

[CreateAssetMenu(fileName = "TileSettings", menuName = "ScriptableObjects/Tile Settings", order = 0)]
public class TileSettings : ScriptableObject
{
    public float animationTime = 0.3f;
    public AnimationCurve animationCurve;
}