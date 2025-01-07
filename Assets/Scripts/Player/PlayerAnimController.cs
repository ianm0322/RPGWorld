using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private PlayerController.PlayerStates playerStateTable;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private IPlayerController _playerController;
    private IDamagable _damagable;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController = GetComponentInParent<PlayerController>();
        _damagable = GetComponentInParent<IDamagable>();

        playerStateTable = _playerController.StateTable;
        _damagable.OnDamaged += (a, b, c, d) => HandleHurtAnimation();
    }

    private void Update()
    {
        HandleDirection();
        HandleStateAnimation();
    }

    private void HandleDirection()
    {
        Direction1D direction = _playerController.Direction;

        switch (direction)
        {
            case Direction1D.Left:
                _spriteRenderer.flipX = true;
                break;
            case Direction1D.Right:
                _spriteRenderer.flipX = false;
                break;
        }
    }

    // ---- Animation Keys ----
    private const string Float_Move_key = "MoveX";
    private const string Trigger_Attack_Key = "OnAttack";
    private const string Integer_AttackIndex_Key = "AttackIndex";
    private const string Trigger_Hurt_Key = "OnHurt";
    private const string Boolean_Living_Key = "IsLiving";
    private const string Boolean_IsAttack_Key = "IsAttack";

    public void HandleStateAnimation()
    {
        var currState = _playerController.CurrentState;

        _animator.SetBool(Boolean_IsAttack_Key, false);

        if (currState == playerStateTable.StateIdle)
        {
            _animator.SetFloat(Float_Move_key, 0);
        }
        else if (currState == playerStateTable.StateMove)
        {
            _animator.SetFloat(Float_Move_key, Mathf.Abs(_playerController.Velocity.x));
        }
        else if (currState is PlayerAttackStateSO)
        {
            _animator.SetBool(Boolean_IsAttack_Key, true);

            if (currState == playerStateTable.StateAttack1)
            {
                _animator.SetInteger(Integer_AttackIndex_Key, 1);
            }
            else if (currState == playerStateTable.StateAttack2)
            {
                _animator.SetInteger(Integer_AttackIndex_Key, 2);
            }
            else if (currState == playerStateTable.StateAttack3)
            {
                _animator.SetInteger(Integer_AttackIndex_Key, 3);
            }
        }
    }

    private void HandleHurtAnimation()
    {
        _animator.SetTrigger(Trigger_Hurt_Key);
    }
}
