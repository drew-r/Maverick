--Copyright © 2013 Drew Rathbone.
--drewrathbone@gmail.com 
--
--This file is part of Maverick.
--
--Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
--as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
--You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
--
--Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
--of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
--
--Additional permission under the GNU Affero GPL version 3 section 7: 
--If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
--that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
--

function arr(t,...)
if arg.n>0 then arg.n = nil table.insert(arg,1,t) t = arg 
elseif type(t) ~= "table" and type(t) ~= "userdata" then t = {t}
end
return Utility.ObjArrayFromTable(t)
end

function obj(t)
return Utility.ObjFromTable(t)
end

function str_arr(t,...)
if arg.n>0 then arg.n = nil table.insert(arg,1,t) t = arg 
elseif type(t) ~= "table" and type(t) ~= "userdata" then t = {t}
end
return Utility.StringArrayFromTable(t)
end

function is_set(v)
return Utility.IsSet(v)
end

function typeof(type)
return Utility.GetUnderlyingType(type)
end

function string.replace(target,find,replace)
return Utility.ReplaceString(target,find,replace)
end

function exit()
	Scheduler.Exit()
end

function sync_callback(action)
	return Scheduler.QueuedCallback(action)
end
  
function async(target)
	local proxy = MethodCaptureDelegator(target)
	return _lua_proxy(nil, function(key,args) return Promise(proxy[key]:go(arr(args))) end)
end

--generic support is too damn awkward
--function generic(target)
--	local proxy = MethodCaptureDelegator(target)
--	local ctor = function(discard,...) --to be support for generic classes
--	maverick:debug(args)
--		local typeArgs = arr(arg[1])
--		arg[1] = nil
--		return proxy["ctor"]:go(typeArgs,arr(arg)):Invoke()
--	end
--	return _lua_proxy(ctor,
--	function(key,args) --supports generic methods
--		local typeArgs = arr(args[1])
--		args[1] = nil
--		return proxy[key]:go(typeArgs, arr(args)):Invoke() 
--		end)
--end

function _lua_proxy(constructor_call, method_call)
local proxy = {}	
	return setmetatable(proxy, { 
	__index = 
		function(table,key)
		 	local callable = {}
		 	return setmetatable(callable, { __call = function(ct,...) return method_call(key,arg) end })  
			end,
	__call = constructor_call })
end
--
--function action(a,num_args)
--	local key = "Action"
--	if num_args and num_args > 0 then key = key .. "_" .. tostring(num_args) end
--	Utility.LuaDelegateConverter[key] = a
--	return Utility.LuaDelegateConverter[key]
--end
--
--
--function func(f,num_args)
--	local key = "Func"
--	if num_args and num_args > 0 then key = key .. "_" .. tostring(num_args) end
--	Utility.LuaDelegateConverter[key] = f
--	return Utility.LuaDelegateConverter[key]
--end 


