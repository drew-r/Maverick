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
using System.Linq.Dynamic;
using LuaInterface;
using System.Collections;
using Maverick;

namespace MavEntity
{
    public class Q : IEnumerable
    {
        public Q(IEnumerable data)
        {
            _data = data.AsQueryable();
        }
        public Q(IQueryable data)
        {
            _data = data;              
        }
        IQueryable _data;
        
        

        object[] a(LuaTable t) { return Utility.ObjArrayFromTable(t); }

        public IQueryable Result() { return _data; }

        public Q Where(string q, LuaTable args)
        {
            _data = _data.Where(q, a(args));
            return this;
        }

        public Q Select(string q, LuaTable args)
        {
            _data = _data.Select(q, a(args));
            return this;
        }
        public Q GroupBy(string q, string s, LuaTable args)
        {
            _data = _data.GroupBy(q, s, a(args));
            return this;
        }
        public Q OrderBy(string q, LuaTable args)
        {
            _data = _data.OrderBy(q, a(args));
            return this;
        }

        public Q Take(int count)
        {
            _data = _data.Take(count);
            return this;
        }

        public bool Any()
        {
            return _data.Any();
        }

        public int Count()
        {
            return _data.Count();
        }

        public Q Skip(int count)
        {
            _data = _data.Skip(count);
            return this;
        }

     
        public dynamic SingleOrDefault()
        {
             return (_data as IQueryable<dynamic>).SingleOrDefault();
        }

        

        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }

}
