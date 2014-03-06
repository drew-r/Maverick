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
		function(t,key)
		 	local callable = {}			
		 	return setmetatable(callable, { __call = function(ct,...) local arg = table.pack(...) return method_call(key,arg) end })  
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
