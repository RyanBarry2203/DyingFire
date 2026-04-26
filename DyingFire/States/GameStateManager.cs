using System.Collections.Generic;

namespace DyingFire.States
{
    public class GameStateManager
    {
        private Stack<IGameState> _states = new Stack<IGameState>();

        public IGameState CurrentState => _states.Count > 0 ? _states.Peek() : null;

        public void PushState(IGameState newState)
        {
            if (_states.Count > 0) _states.Peek().Exit();
            _states.Push(newState);
            newState.Enter();
        }

        public void PopState()
        {
            if (_states.Count > 0)
            {
                _states.Pop().Exit();
                if (_states.Count > 0) _states.Peek().Enter();
            }
        }

        public void Update()
        {
            if (_states.Count > 0) _states.Peek().Update();
        }
    }
}