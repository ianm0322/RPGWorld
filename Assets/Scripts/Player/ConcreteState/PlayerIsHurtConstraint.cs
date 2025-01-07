using UnityEngine;

[CreateAssetMenu(fileName = "Player Is Hurt Constraint", menuName = "State Machine Constraint/Player/Is Hurt", order = 11)]
public class PlayerIsHurtConstraint : PlayerConstraintSO
{
    public override bool IsValid(PlayerController target)
    {
        return target.InputHandle.IsHurt;
    }
}
