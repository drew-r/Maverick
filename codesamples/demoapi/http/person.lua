http:GET('/person',
function(request,response)

--get all people
--auth.required(function()

	local ctx = db()
	local data = ctx.Person

response.json(data):success(function() response:Close() gc(data,ctx,request,response) data=nil ctx=nil request=nil response=nil collectgarbage() end)

--end)

end)

http:GET('/person/:id',
function(request,response)

--get person
local person = byID(db().Person,request.RouteParams.id)
response.json(person):success(function() response:Close() person = nil end)

end)


http:POST('/person',
function(request,response)

--create person
local newperson = Person()
newperson.Forename = request.Body.Forename
newperson.Surname = request.Body.Surname
newperson.AuthUser = request.Body.AuthUser
newperson.AuthPass = request.Body.AuthPass
db():Add(newperson)
response.json(newperson):success(function() response:Close() newperson = nil end)
end
)

http:PUT('/person/:id',function(request,response)

--update person
local person = byID(db().Person,request.RouteParams.id)
person.Forename = request.Body.Forename
person.Surname = request.Body.Surname
db():Save()
response.json(person):success(function() response:Close() person = nil end)

end
)


