using UnityEngine;

[CreateAssetMenu(fileName = "Player Grounding Constraint", menuName = "State Machine Constraint/On Grounding", order = 11)]
public class PlayerGroundingConstraintSO : PlayerConstraintSO
{
    public override bool IsValid(PlayerController target)
    {
        return target.IsGrounded && target.Velocity.y < 0;
    }
}
