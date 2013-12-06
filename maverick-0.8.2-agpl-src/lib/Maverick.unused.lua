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
function enum(o)
   local e = o:GetEnumerator()
   return function()
      if e:MoveNext() then
        return e.Current
     end
   end
end


function cast(o,type)
	return Convert.ChangeType(o, typeof(type));
end



--	SwitchObject - http://lua-users.org/wiki/SwitchObject
local function switch_isofcase(val, values)
	local vt = type(values)
	if vt == 'table' then
		for i, v in ipairs(values) do
			if v == val then return true end
		end
		return false
	end
	if vt == 'function' then
		return values(val)
	end
	return (values == val)
end

local function switch_test(switch, val)
	for i, case in ipairs(switch.cases) do
		if switch_isofcase(val, case.value) then
			if case.func then return case.func(val, case.value) end
			return case.ret
		end
	end
	if switch.default_func then
		return switch.default_func(val, 'default')
	end
	return switch.default_ret
end

local function switch_case(switch, value, fn)
	if type(fn) == 'function' then
		table.insert(switch.cases, {value = value, func = fn})
	else
		table.insert(switch.cases, {value = value, ret = fn})
	end
end

local function switch_default(switch, fn)
	if type(fn) == 'function' then
		switch.default_func = fn
	else
		switch.default_ret = fn
	end
end


function switch()


	local s = {}
	s.cases = {}
	s.test = switch_test
	s.case = switch_case
	s.default = switch_default
	return s
end
-- /SwitchObject - http://lua-users.org/wiki/SwitchObject