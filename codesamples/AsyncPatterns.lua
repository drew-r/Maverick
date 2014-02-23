#reference 'MavHttp'
#reference 'MavEntity'
#reference 'System.Data.Linq.dll'
#reference 'Newtonsoft.Json'
#compile '\\demoapi\\entities\\Person.cs'


LOG_CLR_CACHE()
serv = Http()

-- asynchronously call a blocking .NET method then call back via scheduler
local asyncThread = async(Thread) --obtain an async proxy to type Thread 
asyncThread.Sleep(1000):success(function() print "one" end):error(function(e) print(tostring(e))end)--invoke static method Sleep, on success callback to Lua
asyncThread.Sleep(1000):success(function() print "two" end)


--plain sync
serv:GET('/',function(req,res)

sw = StreamWriter(res.OutputStream)
sw.AutoFlush = true
sw:Write("Hello Internet!")
res:Close()

end)


serv:GET('/async',function(req,res)	

sw = StreamWriter(res.OutputStream)
sw.AutoFlush = true

-- _async mirrors the global table but wraps its elements with async(), providing an alternative syntax. 
_async.sw:Write("Hello Internet! (async)",1):success(function() res:Close() end):error(function(e) print(tostring(e)) end)
	
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
_async.JSON.stringify(Q(data.Person)):success(function(x)
			sw = StreamWriter(res.OutputStream)
			_async.sw:Write(x):success(function() res:Close() end)
										end):error(function(e) res:Close(Encoding.UTF8:GetBytes(tostring(e))) end)

end)

serv:GET('/data_sync',function(req,res)

local data = EntityContext("Server=localhost;Database=demoapi;Trusted_Connection=True;") 

sw = StreamWriter(res.OutputStream)
sw:Write(JSON.stringify(Q(data.Person)))
res:Close()

end)


serv:listen("http://+:8080/")

Console.WriteLine("Listening 8080")

_async.Console.ReadKey():success(function() exit() end)