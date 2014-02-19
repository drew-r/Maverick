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
--libs--
#reference 'Newtonsoft.Json.dll'													--reference any dependencies required to compile your csharp
#reference 'System.Data.Linq.dll'	
#reference 'MavEntity.dll'
#reference 'MavHttp.dll'

--entities--
#compile 'entities/Person.cs'														--#compile directive will include a csharp file into the dynamic assembly

--http api--
#include 'http/person.lua'															--include any additional lua 
#include 'http/user.lua'
																						
																						
--configuration

--entity manager is a module that takes care of entity persistence and retrieval
entityManager = EntityContext('Server=localhost;Database=demoapi;Trusted_Connection=True;')

--create a server from the http module
http = Http()

--api stuff--

--provide a route for preflight OPTIONS CORs 																	
http:OPTIONS('/:nothing',function(req,res) res:Close() end)

--middleware to add CORs access control headers
http:use(function(request,response,next)

	        local accCtrlReqHdr = request.Headers["Access-Control-Request-Headers"]
	        if accCtrlReqHdr then
	        	response:AddHeader("Access-Control-Allow-Headers", accCtrlReqHdr)
	    	end
            response:AddHeader("Access-Control-Allow-Origin","*")
            response:AddHeader("Access-Control-Allow-Methods","OPTIONS,PUT,POST,GET")
            next:Invoke()
end)

--middleware to serialize json request body
http:use(function(request,response,next)

if (request.HasEntityBody == true and request.ContentType == "application/json") then 
	request.Body = JSON.read(request.InputStream)	
end
next:Invoke()
end)

--middleware to enhance response obj with friendlier method to write json
http:use(function(request,response,next)
response.json = function(obj)
	response.ContentType = "application/json"
	JSON.write(response.OutputStream,obj)
end
next:Invoke()
end)

--middleware for authentication
auth = { }

http:use(function(request,response,next)

	--declared within lexical scope of request & response 				
	auth.validateCredentials = function(username,password)
	local person = Q(entityManager.Person):Where("AuthUser=@0 and AuthPass=@1",{username , password}):SingleOrDefault()
	if person then 
		request.session = {}
		request.session.identity = person.ID			
		return true
	end
	return false
	end

	--just one of many ways to enforce authentication (a container / callback)
	auth.required = function(callback)

		--can request be authenticated? 
		local authenticated = false
		if request.Headers['Authorization'] then 
			local authStr = request.Headers['Authorization']
			local s1 = string.find(authStr,' ')	
			authStr = string.sub(authStr,s1+1)
			authStr = Convert.FromBase64String(authStr)
			authStr = Utility.GetEncoding("utf-8"):GetString(authStr)
			local s2 = string.find(authStr,':')
			local username = string.sub(authStr,0,s2-1)
			local password = string.sub(authStr,s2+1)
			Console.WriteLine("Authorization USER " .. username .. " PASS " .. password)
			authenticated = auth.validateCredentials(username,password)
			if authenticated then Console.WriteLine("GRANTED") end
		end

		--yes, invoke callback
		if authenticated then
		callback()
		return
		end
		--no, send authentication response
		response.StatusCode = 401
		response:AddHeader("WWW-Authenticate",'Basic realm="demoapi"')
		response:Close()

	end



next:Invoke()
end)


app = maverick:app(function()							--show time! function passed in here is called after included Lua is executed.
  --the app instance returned can be indexed for objects via a string and so can be used to store app scope data
	http:listen('http://+:8080/')
	Console.WriteLine("Listening..")
	Console.ReadKey()

end)

function byID(collection,ID)

	return Q(collection):Where("ID = @0",{Guid.Parse(ID)}):SingleOrDefault()

end