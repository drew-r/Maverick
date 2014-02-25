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