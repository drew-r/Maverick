#reference 'MavHttp.dll'
#compile 'Something.cs'
import 'System.Threading.Tasks'
LOG_CLR_CACHE()

serv = Http()

-- asynchronously call a blocking .NET method then call back via scheduler
async(Thread).Sleep(1000):success(function() print "waited a sec" end)


serv:GET('/async',function(req,res)	

sw = StreamWriter(res.OutputStream)
sw.AutoFlush = true

-- _async mirrors the global table but wraps its elements with async(), providing an alternative syntax. 
_async.sw:Write("HIII {0}",1):success(function()  print("omfg") res:Close() end)
	
end)



serv:GET('/sync_callback',function(req,res)

--call an asynchronous .NET method, providing an AsyncCallback that delegates to the scheduler, with sync_callback()
res.OutputStream:BeginWrite(Utility.GetBytes("hello"),0,10,
 	sync_callback(function(x) res.OutputStream:EndWrite(x) res:Close() end)
,obj({}))

end)

serv:listen("http://+:8080/")

Console.WriteLine("Listening 8080")

Console.ReadKey()

