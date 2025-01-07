using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player Input Constraint", menuName = "State Machine Constraint/Input", order = 11)]
public class PlayerInputConstraintSO : PlayerConstraintSO
{
    [SerializeField]
    private InputActionAsset _bindingInputAsset;

    [SerializeField]
    private string _inputActionKey;

    [SerializeField]
    private InputType _inputType;

    private InputAction _bindingInputAction;

    private void OnEnable()
    {
        _bindingInputAction = _bindingInputAsset.FindAction(_inputActionKey);
        _bindingInputAction.Enable();
    }

    public override bool IsValid(PlayerController target)
    {
        var data = GetInstanceData<InstanceData>(target);

        if(data.playerInput == null)
        {
            data.playerInput = target.GetComponent<PlayerInput>();
        }

        if (data.playerInput != null && data.playerInput.enabled)
        {
            bool isPressed = data.playerInput.currentActionMap.FindAction(_inputActionKey).IsPressed();

            switch (_inputType)
            {
                case InputType.Pressed:
                    return isPressed;
                case InputType.Released:
                    return !isPressed;
            }
        }
        return false;
    }

    public class InstanceData
    {
        public PlayerInput playerInput;
    }

    public enum InputType
    {
        Pressed,
        Released,
    }
}
