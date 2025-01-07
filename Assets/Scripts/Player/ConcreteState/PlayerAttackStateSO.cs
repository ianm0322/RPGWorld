using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Attack State", menuName = "State Machine/Player/Attack", order = 11)]
public class PlayerAttackStateSO : PlayerStateSO, IAutoExitableState
{
    [SerializeField]
    private AnimationCurve _velocityCurve;

    [SerializeField]
    private float _multiplier = 1f;

    [SerializeField]
    private bool _handleVelocityDirection = true;

    [SerializeField]
    private float _endTime = 1f;

    [SerializeField]
    private List<HitboxTiming> _hitboxTimings;

    private void Awake()
    {
        _hitboxTimings?.Sort();
    }

    public bool IsEndNode(object controller)
    {
        if(controller is PlayerController player)
        {
            return player.StateTime > _endTime;
        }

        return false;
    }

    public override void OnStateBegin(PlayerController target, PlayerStateSO previousState)
    {
        var data = GetInstanceData<StateInstance>(target);

        data.startDirection = target.Direction;
        data.handleVelocityDirection = target.HandleVelodityDirection;
        data.hitboxIndex = 0;

        target.Velocity = Vector2.zero;
        target.HandleVelodityDirection = _handleVelocityDirection;
    }

    public override void OnStateEnd(PlayerController target, PlayerStateSO nextState)
    {
        target.HandleVelodityDirection = GetInstanceData<StateInstance>(target).handleVelocityDirection;
    }

    public override void PhysicsUpdateState(PlayerController target)
    {
        float progress = Mathf.Clamp01(target.StateTime / _endTime);
        float movement = _velocityCurve.Evaluate(progress) * _multiplier;
        float direction = GetInstanceData<StateInstance>(target).startDirection == Direction1D.Left ? -1 : 1;

        Vector2 velocity = target.Velocity;
        velocity.x = movement * direction;
        target.Velocity = velocity;
    }

    public override void UpdateState(PlayerController target)
    {
        var stateInstance = GetInstanceData<StateInstance>(target);
        while (_hitboxTimings.Count > stateInstance.hitboxIndex && target.StateTime > _hitboxTimings[stateInstance.hitboxIndex].Time)
        {
            HitboxTiming hitboxTiming = _hitboxTimings[stateInstance.hitboxIndex];
            HitboxSO hitbox = hitboxTiming.Hitbox;
            ++stateInstance.hitboxIndex;

            var hitResults = target.AttachedHitboxCaster.Overlap(hitbox);
            IDamagable damagable;
            foreach(var hitResult in hitResults)
            {
                if(hitResult.TryGetComponent<IDamagable>(out damagable))
                {
                    damagable.ApplyDamage(hitboxTiming.Damage, this, target.gameObject);
                }
            }
        }
    }

    private class StateInstance
    {
        public Direction1D startDirection;
        public bool handleVelocityDirection;
        public int hitboxIndex;
    }

    [System.Serializable]
    public class HitboxTiming : IComparable<HitboxTiming>
    {
        public float Time;
        public HitboxSO Hitbox;
        public int Damage;

        public int CompareTo(HitboxTiming other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
