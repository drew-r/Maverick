http:GET('/user',function(request,response)

auth.required(function()

--who am i?
response.json(byID(db().Person,request.session.identity))

end)
end)
