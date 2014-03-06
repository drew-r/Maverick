using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maverick;
namespace MavHttp
{
    class FiniteState
    {
        public FiniteState(params object[] states)
        {
            lock (sync)
            {
                _states = new SortedSet<object>(states);
            }
        }
        public override int GetHashCode()
        {
            return activeState.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return object.Equals(activeState,obj);
        }
        
        public static bool operator ==(FiniteState x, object y)
        {
            return object.Equals(x.activeState, y);
        }
        public static bool operator !=(FiniteState x, object y)
        {
            return x == y;
        }

        public void TransitionTo(object state)
        {
            activeState = state;
        }

        object sync = new object();
        readonly SortedSet<object> _states;
        object _activeState;
        object activeState
        {
            get { lock (sync) { return _activeState; } }
            set
            {
                lock (sync)
                {
                    if (_states.Contains(value)) { _activeState = value; }
                    else
                    {
                        throw new InvalidOperationException("Invalid state.");
                    }
                }
            }
        }
        
    }
}
