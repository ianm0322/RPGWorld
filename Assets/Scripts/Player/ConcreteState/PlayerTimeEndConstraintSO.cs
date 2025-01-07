using UnityEngine;

[CreateAssetMenu(fileName = "Player Time End Constraint", menuName = "State Machine Constraint/Time Over", order = 11)]
public class PlayerTimeEndConstraintSO : PlayerConstraintSO
{
    [SerializeField]
    private float _endTime;

    public override bool IsValid(PlayerController target)
    {
        return target.StateTime > _endTime;
    }
}
