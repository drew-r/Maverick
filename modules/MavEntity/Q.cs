using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using NLua;
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
