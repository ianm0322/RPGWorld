using UnityEngine;

[CreateAssetMenu(fileName = "Player Idle State", menuName = "State Machine/Player/Idle", order = 11)]
public class PlayerIdleStateSO : PlayerStateSO
{
    [SerializeField]
    private float HorizontalDamping = 10;

    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
    }

    public override void PhysicsUpdateState(PlayerController target)
    {
        Rigidbody2D rigidbody = target.AttachedRigidbody;
        rigidbody.linearVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, 0, HorizontalDamping * Time.fixedDeltaTime);
    }

    public override void UpdateState(PlayerController target)
    {
    }
}
