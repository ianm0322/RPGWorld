using UnityEngine;

[CreateAssetMenu(fileName = "Player Move State", menuName = "State Machine/Player/Move", order = 11)]
public class PlayerMoveStateSO : PlayerStateSO
{
    public override void UpdateState(PlayerController target)
    {
    }

    public override void PhysicsUpdateState(PlayerController target)
    {
        Rigidbody2D rigidbody = target.AttachedRigidbody;
        float destination = target.Stats.MaxSpeedOnGround * target.InputHandle.MoveX;
        float delta = target.Stats.AccelerationOnGround * Time.fixedDeltaTime;
        rigidbody.linearVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, destination, delta);
    }

    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
    }
}
