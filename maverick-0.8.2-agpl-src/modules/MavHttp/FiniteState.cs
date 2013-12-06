//Copyright © 2013 Drew Rathbone.
//drewrathbone@gmail.com 
//
//This file is part of Maverick.
//
//Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
//as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
//You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
//
//Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
//Additional permission under the GNU Affero GPL version 3 section 7: 
//If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
//that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
//
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
