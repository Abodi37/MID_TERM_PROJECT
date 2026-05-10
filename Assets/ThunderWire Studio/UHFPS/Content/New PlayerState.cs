using UnityEngine;
using UHFPS.Scriptable;

namespace UHFPS.Runtime.States
{
    public class NewPlayerState : PlayerStateAsset
    {
        public override FSMPlayerState InitState(PlayerStateMachine machine, PlayerStatesGroup group)
        {
            return new NewPlayerState_State(machine);
        }

        public override string StateKey => "Your state key";
        public override string Name => "Your state name";

        public class NewPlayerState_State : FSMPlayerState
        {
            public NewPlayerState_State(PlayerStateMachine machine) : base(machine) { }

            public override void OnStateEnter()
            {
                
            }

            public override void OnStateExit()
            {
                
            }

            public override void OnStateUpdate()
            {
                
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[0];
            }
        }
    }
}