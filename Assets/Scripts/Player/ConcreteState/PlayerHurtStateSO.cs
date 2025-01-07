using UnityEngine;

[CreateAssetMenu(fileName = "Player Hurt State", menuName = "State Machine/Player/Hurt", order = 11)]
public class PlayerHurtStateSO : PlayerStateSO
{
    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
        target.HandleVelodityDirection = false;
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
        target.HandleVelodityDirection = true;
    }

    public override void PhysicsUpdateState(PlayerController target)
    {
    }

    public override void UpdateState(PlayerController target)
    {
    }
}
