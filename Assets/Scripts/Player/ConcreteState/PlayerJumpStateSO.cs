using UnityEngine;

[CreateAssetMenu(fileName = "Player Jump State", menuName = "State Machine/Player/Jump", order = 11)]
public class PlayerJumpStateSO : PlayerStateSO
{
    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
        target.AttachedRigidbody.linearVelocityY = target.Stats.JumpPower;
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
    }

    public override void PhysicsUpdateState(PlayerController target)
    {
        Rigidbody2D rigidbody = target.AttachedRigidbody;
        float destination = target.Stats.MaxSpeedOnAir * target.InputHandle.MoveX;
        float delta = target.Stats.AccelerationOnGround * Time.fixedDeltaTime;
        rigidbody.linearVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, destination, delta);
    }

    public override void UpdateState(PlayerController target)
    {
    }
}
