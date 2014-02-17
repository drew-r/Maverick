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

function arr(t)
return Utility.ObjArrayFromTable(t)
end

function obj(t)
return Utility.ObjFromTable(t)
end

function str_arr(t)
return Utility.StringArrayFromTable(t)
end

function is_set(v)
return Utility.IsSet(v)
end

function typeof(type)
return ServiceLocator.GetType(type)
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
	local indexable = {}	
	return setmetatable(indexable, { __index = 
		function(table,key)
		 	local callable = {}
		 	return setmetatable(callable, { __call = function(table,...) return Promise(proxy[key]:go(arr(arg))) end })  
		end})
end