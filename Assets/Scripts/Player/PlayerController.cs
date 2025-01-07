using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerController;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public interface IPlayerController
{
    public Direction1D Direction { get; set; }
    public bool IsGrounded { get; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public PlayerStateSO CurrentState { get; }
    public StateMachineSO.PlayerStates StateTable { get; }
    public PlayerInputHandle InputHandle { get; }

    public class PlayerInputHandle
    {
        public float MoveX;
        public bool Jump;
        public bool Attack;
        public int AttackIndex;
        public bool IsHurt;

        public void Clear()
        {
            Attack = false;
            IsHurt = false;
        }
    }
}

[SelectionBase]
public class PlayerController : MonoBehaviour, IPlayerController, IDamagable
{
    // ---- Serialize Field ----
    public PlayerStatsSO Stats;


    // ---- Components ----
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _collider;
    private IHitboxCaster _hitboxCaster;
    private Animator _animator;

    public Rigidbody2D AttachedRigidbody => _rigidbody;
    public CapsuleCollider2D AttachedCapsuleCollider => _collider;
    public IHitboxCaster AttachedHitboxCaster => _hitboxCaster;
    public Animator AttachedAnimator => _animator;


    // ---- State Machine ----
    [SerializeField]
    private StateMachineSO _smModel;

    [SerializeField]
    private PlayerStateSO _smDefaultState;

    [SerializeField]
    private PlayerStateSO _smCurrentState;

    public float StateTime { get; private set; }


    #region Interface Implementation : IPlayerController
    public Direction1D Direction { get; set; }

    public bool IsGrounded => _isGrounded;

    public Vector2 Position
    {
        get => _rigidbody.position;
        set => _rigidbody.position = value;
    }

    public Vector2 Velocity
    {
        get => _rigidbody.linearVelocity;
        set => _rigidbody.linearVelocity = value;
    }

    public PlayerStateSO CurrentState => _smCurrentState;

    public StateMachineSO.PlayerStates StateTable => _smModel.States;

    public IPlayerController.PlayerInputHandle InputHandle => _inputHandle;
    #endregion

    #region Interface Implementation : IDamagable
    public event IDamagable.DamageEventDelegate OnDamaged;

    public void ApplyDamage(float damage, object cause, GameObject instigator)
    {
        if (instigator != null)
        {
            Vector2 betweenVector = _rigidbody.position - (Vector2)instigator.transform.position;
            float direction = Mathf.Sign(betweenVector.x);

            _rigidbody.linearVelocity = Vector2.right * direction * 2 + Vector2.up * 1; // TODO: Temporary const value.
            _inputHandle.IsHurt = true;

            OnDamaged?.Invoke(this, damage, cause, instigator);
        }
    }
    #endregion


    private void Start()
    {
        SetupComponent();
        SetupStateMachine();
    }
    private void SetupComponent()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _hitboxCaster = GetComponent<IHitboxCaster>();
        _collider = GetComponent<CapsuleCollider2D>();
    }
    private void SetupStateMachine()
    {
#if UNITY_EDITOR
        if (_smDefaultState == null)
        {
            throw new System.Exception("PlayerController._smDefaultState 프로퍼티가 할당되지 않았습니다.");
        }
#endif

        _smCurrentState = _smDefaultState;
        _smCurrentState.OnStateBegin(this, null);
    }

    private void OnDisable()
    {
        ClearStateMachineCacheDatas();
    }
    private void ClearStateMachineCacheDatas()
    {
        _smModel.States.RemoveInstanceDatas(this);

        foreach (var constraint in _smModel.AnyStateConstraint)
        {
            constraint.Constraint.RemoveInstanceData(this);
        }

        foreach (var raw in _smModel.ConstraintTable.ConstraintTable)
        {
            foreach (var constraint in raw.Constraints)
            {
                constraint.Constraint.RemoveInstanceData(this);
            }
        }
    }

    #region Logic
    private void Update()
    {
        UpdateStateMachine();

        HandleDirection();

        ClearInput();
    }
    private void UpdateStateMachine()
    {
        StateTime += Time.deltaTime;

        if (_smCurrentState == null)
        {
            return;
        }

        PlayerStateSO nextState = null;

        foreach (var anyConstraint in _smModel.AnyStateConstraint)
        {
            if (anyConstraint.Constraint.IsValid(this))
            {
                nextState = anyConstraint.DestinationState;
                break;
            }
        }

        if (nextState == null)
        {
            var constraintRaw = _smModel.ConstraintTable.FindTableRaw(_smCurrentState);
            foreach (var constraint in constraintRaw.Constraints)
            {
                if (constraint.Constraint.IsValid(this))
                {
                    nextState = constraint.DestinationState;
                    break;
                }
            }
        }

        if (nextState != null)
        {
            PlayerStateSO preState = _smCurrentState;

            _smCurrentState.OnStateEnd(this, nextState);
            _smCurrentState = nextState;
            _smCurrentState.OnStateBegin(this, preState);

            StateTime = 0f;
        }

        _smCurrentState.UpdateState(this);
    }
    private void ClearInput()
    {
        _inputHandle.Clear();
    }

    private void FixedUpdate()
    {
        CheckCollision();

        HandleJump();

        FixedUpdateStateMachine();
    }
    private void FixedUpdateStateMachine()
    {
        _smCurrentState.PhysicsUpdateState(this);
    }
    #endregion


    #region Physics
    private bool _isGrounded;
    private bool _handleVelocityDirection = true;

    public bool HandleVelodityDirection
    {
        get => _handleVelocityDirection;
        set => _handleVelocityDirection = value;
    }

    private void CheckCollision()
    {
        _collider.enabled = false;

        bool isGround = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0f, Vector2.down, Stats.CollisionOffset, ~Stats.PlayerLayer);
        bool isWallHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0f, Vector2.right * Mathf.Sign(_rigidbody.linearVelocityX), Stats.CollisionOffset, ~Stats.PlayerLayer);

        _collider.enabled = true;

        if (isGround)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        if (isWallHit && !Mathf.Approximately(_rigidbody.linearVelocityX, 0f))
        {
            _rigidbody.linearVelocityX = 0f;
        }
    }
    private const float TURN_DIRECTION_VALUE = 0.01f;
    private void HandleDirection()
    {
        if (!_handleVelocityDirection)
        {
            return;
        }

        if (Mathf.Abs(_rigidbody.linearVelocityX) > TURN_DIRECTION_VALUE)
        {
            Direction = _rigidbody.linearVelocityX < 0 ? Direction1D.Left : Direction1D.Right;
        }
    }
    private void HandleJump()
    {
        if (_inputHandle.Jump && IsGrounded)
        {
            _rigidbody.linearVelocityY = Stats.JumpPower;
        }

        _inputHandle.Jump = false;
    }
    #endregion


    #region Input Handle
    private IPlayerController.PlayerInputHandle _inputHandle = new IPlayerController.PlayerInputHandle();

    public void InputMove(CallbackContext context)
    {
        _inputHandle.MoveX = context.ReadValue<float>();
    }
    public void InputJump(CallbackContext context)
    {
        if (context.started)
        {
            _inputHandle.Jump = true;
        }
    }
    public void InputAttack(int attackIndex)
    {
        _inputHandle.Attack = true;
        _inputHandle.AttackIndex = attackIndex;
    }
    #endregion
}
