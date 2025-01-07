using UnityEngine;

[CreateAssetMenu(fileName = "To Jump Constraint", menuName = "State Machine Constraint/To Jump", order = 11)]
public class PlayerToJumpConstraintSO : PlayerConstraintSO
{
    public override bool IsValid(PlayerController target)
    {
        if (!target.IsGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
