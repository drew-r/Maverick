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