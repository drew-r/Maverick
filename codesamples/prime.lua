if Configuration.AppArgs[0] then i = tonumber(Configuration.AppArgs[0]) else i = 3 end
while (true) do	
	local c = 2
	continue = true
	while (continue) do		
		if c < i and i % c == 0 then 
			continue = false 
		elseif c >= i then
			continue = false
			Console.WriteLine(tostring(i) .. " IS PRIME")				
		end		
		c = c + 1
	end	
	i = i + 1
end