#reference 'MavHttp'
#reference 'MavEntity'
#reference 'System.Data.Linq.dll'
#reference 'Newtonsoft.Json'
#compile 'demoapi/entities/Person.cs'


LOG_CLR_CACHE()
serv = Http()

-- asynchronously call a blocking .NET method then call back via scheduler
local asyncThread = async(Thread) --obtain an async proxy to type Thread 
asyncThread.Sleep(1000):success(function() print "one" end):error(function(e) print(tostring(e))end)--invoke static method Sleep, on success callback to Lua
asyncThread.Sleep(1000):success(function() print "two" end)


serv:GET('/memtest',function(req,res)

--print("memtest")

	--local ctx = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;") 
	--local data = ctx.Person
	--local query =  Q(ctx.Person)
	--local str = JSON.stringify(query)
	--res:Close(str)

	--test 1 -- memory stable
	--res:Close(JSON.stringify(EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;Min Pool Size=5").Person))
	
	--test 2 -- out of memory
	--local db = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;Min Pool Size=5")
	--local data = db.Person	
	--_async.JSON.write(res.OutputStream, data):success(function() res:Close() db:Dispose() db = nil data = nil res = nil collectgarbage() end)


	--test 3
	local db = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;Min Pool Size=5")
	local data = db.Person
	local asyncJSON = async(JSON)
	local promise = asyncJSON.write(res.OutputStream, data)
	promise:success(function() res:Close() db:Dispose() db = nil data = nil res = nil asyncJSON = nil promise = nil collectgarbage() end)
	

	--ctx = nil
	--data = nil
	--query = nil
	--str = nil
--	
	
end)


--plain sync
serv:GET('/',function(req,res)

sw = StreamWriter(res.OutputStream)
sw.AutoFlush = true
sw:Write("Hello Internet!")
res:Close()

end)


serv:GET('/async',function(req,res)	


-- _async mirrors the global table but wraps its elements with async(), providing an alternative syntax. 
_async.JSON.write(res.OutputStream,"Hello Internet! (async)"):success(function() res:Close() res = nil req = nil end):error(function(e) print(tostring(e)) end)
	
end)


serv:GET('/sync_callback',function(req,res)


--call an asynchronous .NET method, providing an AsyncCallback that delegates to the scheduler, with sync_callback()
local resBuffer = Encoding.UTF8:GetBytes("Hello Internet! (sync_callback)")
res.OutputStream:BeginWrite(resBuffer,0,resBuffer.Length,
 	sync_callback(function(x) res.OutputStream:EndWrite(x) res:Close() end)
,obj({}))

end)


serv:GET('/data',function(req,res)

local data = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;") 
local q = Q(data.Person)
_async.JSON.write(res.OutputStream,q)
	:success(function() data:Dispose() data = nil res:Close() res = nil req = nil q = nil end)
	:error(function(e) res:Close(Encoding.UTF8:GetBytes(tostring(e))) end)

end)

serv:GET('/data_sync',function(req,res)

local data = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;") 

sw = StreamWriter(res.OutputStream)
sw:Write(JSON.stringify(Q(data.Person)))
res:Close()
data:Dispose()
req = nil
res = nil
sw = nil
data = nil

end)


serv:listen("http://+:8080/")

Console.WriteLine("Listening 8080")

_async.Console.ReadKey():success(function() exit() end)