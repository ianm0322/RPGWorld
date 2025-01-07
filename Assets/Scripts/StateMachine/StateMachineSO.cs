using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateMachineSO", menuName = "Scriptable Objects/StateMachineSO")]
public class StateMachineSO : ScriptableObject
{
    public PlayerStates States;

    public PlayerContraintTable ConstraintTable;

    public List<ConstraintData> AnyStateConstraint;


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
