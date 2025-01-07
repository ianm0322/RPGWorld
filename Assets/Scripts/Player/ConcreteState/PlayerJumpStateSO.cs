using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Jump State", menuName = "State Machine/Player/Jump", order = 11)]
public class PlayerJumpStateSO : PlayerStateSO
{
    public PhysicsMaterial2D JumpingPM;

    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
        var data = GetInstanceData<InstanceData>(target);
        data.cachedPM = target.AttachedRigidbody.sharedMaterial;

        target.AttachedRigidbody.sharedMaterial = JumpingPM;
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
        var data = GetInstanceData<InstanceData>(target);
        target.AttachedRigidbody.sharedMaterial = data.cachedPM;
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

    public class InstanceData
    {
        public PhysicsMaterial2D cachedPM;
    }
}
