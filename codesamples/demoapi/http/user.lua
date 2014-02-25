
http:GET('/user',function(request,response)

auth.required(function()

--who am i?
response.json(byID(entityManager.Person,request.session.identity))

end)
end)
