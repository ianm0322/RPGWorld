using UnityEngine;

[CreateAssetMenu(fileName = "Auto Exit Constaint", menuName = "State Machine Constraint/Auto Exit", order = 11)]
public class AutoExitConstraintSO : PlayerConstraintSO
{
    public override bool IsValid(PlayerController target)
    {
        if(target.CurrentState is IAutoExitableState state)
        {
            return state.IsEndNode(target);
        }

        throw new System.Exception("시작 노드가 IAutoExitableState 인터페이스를 상속하지 않았습니다.");
    }
}
