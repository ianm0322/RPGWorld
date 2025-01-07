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
    public PlayerStates StateTable { get; }
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
            Jump = false;
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
    private PlayerStates _smStates;

    [SerializeField]
    private PlayerContraintTable _smConstraints;

    [SerializeField]
    private List<ConstraintData> _smAnyStateConstraint;

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

    public PlayerStates StateTable => _smStates;

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
        _smStates.RemoveInstanceDatas(this);

        foreach (var constraint in _smAnyStateConstraint)
        {
            constraint.Constraint.RemoveInstanceData(this);
        }

        foreach (var raw in _smConstraints.ConstraintTable)
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

        var constraintRaw = _smConstraints.FindTableRaw(_smCurrentState);

        PlayerStateSO nextState = null;

        foreach (var anyConstraint in _smAnyStateConstraint)
        {
            if (anyConstraint.Constraint.IsValid(this))
            {
                nextState = anyConstraint.DestinationState;
                break;
            }
        }

        if (nextState == null)
        {
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

        _collider.enabled = true;

        if (isGround)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
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

    [System.Serializable]
    public class PlayerStates
    {
        public PlayerStateSO StateIdle;
        public PlayerStateSO StateMove;
        public PlayerStateSO StateJump;
        public PlayerStateSO StateAttack1;
        public PlayerStateSO StateAttack2;
        public PlayerStateSO StateAttack3;
        public PlayerStateSO StateHurt;

        public void RemoveInstanceDatas(PlayerController player)
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                PlayerStateSO stateField = field.GetValue(this) as PlayerStateSO;
                stateField.RemoveInstanceData(player);
            }
        }
    }

    [System.Serializable]
    public class PlayerContraintTable
    {
        public List<ConstraintTableRaw> ConstraintTable;

        public ConstraintTableRaw FindTableRaw(PlayerStateSO startState)
        {
            return ConstraintTable.Find((raw) => raw.StartState == startState);
        }

        [System.Serializable]
        public class ConstraintTableRaw
        {
            public PlayerStateSO StartState;
            public List<ConstraintData> Constraints;
        }
    }

    [System.Serializable]
    public class ConstraintData
    {
        public PlayerStateSO DestinationState;
        public PlayerConstraintSO Constraint;
    }
}
