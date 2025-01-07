using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Composite Constraint", menuName = "State Machine Constraint/Composite", order = 11)]
public class CompositeConstraintSO : PlayerConstraintSO
{
    public List<PlayerConstraintSO> CompositeConditions = new List<PlayerConstraintSO>();

    public CompositeType Type;
    public bool Inverse = false;

    public override bool IsValid(PlayerController target)
    {
        switch (Type)
        {
            case CompositeType.And:
                foreach (var condition in CompositeConditions)
                {
                    if (condition.IsValid(target) == Inverse)
                    {
                        return false;
                    }
                }
                return true;
            case CompositeType.Or:
                foreach (var condition in CompositeConditions)
                {
                    if (condition.IsValid(target) != Inverse)
                    {
                        return true;
                    }
                }
                return false;
        }
        return false;
    }

    public enum CompositeType
    {
        And,
        Or
    }
}
