#reference 'MavHttp.dll'


import 'System.Threading.Tasks'

LOG_CLR_CACHE()

serv = Http()

-- asynchronously call a blocking .NET method then call back via scheduler
async(Thread).Sleep(1000):success(function() print "waited a sec" end)


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
_async.sw:Write("Hello Internet! (async)",1):success(function() res:Close() end)
	
end)


serv:GET('/sync_callback',function(req,res)


--call an asynchronous .NET method, providing an AsyncCallback that delegates to the scheduler, with sync_callback()
local resBuffer = Encoding.UTF8:GetBytes("Hello Internet! (sync_callback)")
res.OutputStream:BeginWrite(resBuffer,0,resBuffer.Length,
 	sync_callback(function(x) res.OutputStream:EndWrite(x) res:Close() end)
,obj({}))

end)


serv:GET('/close',function(req,res)

--a quick method of closing a response asynchronously
res:Close(Encoding.UTF8:GetBytes("Hello Internet! (close)"),false)

end)

serv:listen("http://+:8080/")

Console.WriteLine("Listening 8080")

_async.Console.ReadKey():success(function() exit() end)