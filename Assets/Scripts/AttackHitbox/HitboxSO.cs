using UnityEngine;

[CreateAssetMenu(fileName = "Hitbox", menuName = "Scriptable Objects/Hitbox")]
public class HitboxSO : ScriptableObject
{
    public Vector2 Point = new Vector2(1, 0);
    public Vector2 Size = new Vector2(1, 1);
    public float Angle = 0;
}
