http:GET('/person',
function(request,response)

--get all people
auth.required(function()
response.json(entityManager.Person)
end)

end)

http:GET('/person/:id',
function(request,response)

--get person
local person = byID(entityManager.Person,request.RouteParams.id)
response.json(person)

end)


http:POST('/person',
function(request,response)

--create person
local newperson = Person()
newperson.Forename = request.Body.Forename
newperson.Surname = request.Body.Surname
newperson.AuthUser = request.Body.AuthUser
newperson.AuthPass = request.Body.AuthPass
entityManager:Add(newperson)
response.json(newperson)
end
)

http:PUT('/person/:id',function(request,response)

--update person
local person = byID(entityManager.Person,request.RouteParams.id)
person.Forename = request.Body.Forename
person.Surname = request.Body.Surname
entityManager:Save()
response.json(person)
person = nil
end
)

