using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "Stats Profile/Player", order = 12)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Movement (Ground)")]
    public float MaxSpeedOnGround = 4;
    public float AccelerationOnGround = 120;

    [Header("Movement (Air)")]
    public float MaxSpeedOnAir = 3;
    public float AccelerationOnAir = 120;

    [Header("Jump")]
    public float JumpPower = 20;

    [Header("Physics Parameter")]
    public LayerMask PlayerLayer;
    public float CollisionOffset = 0.01f;
}
