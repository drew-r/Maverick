local xcm = {}
xcm[".lua"] = "--"
xcm[".sql"] = "--"
xcm[".cs"] = "//"

local license = [[
Copyright © 2013 Drew Rathbone.
drewrathbone@gmail.com 

This file is part of Maverick.

Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
You should have received a copy of the the GNU Affero General Public License, along with Maverick. 

Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

Additional permission under the GNU Affero GPL version 3 section 7: 
If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
]]

function writeWithoutLicense(comment_str,input_file,output_file)
local input_text = File.ReadAllText(input_file)
local lclLicense = comment_str .. license:gsub("\n","\r\n" .. comment_str) .. "\r\n"
local output_text = input_text:replace(lclLicense,"")

File.WriteAllText(output_file, output_text)
input_text = nil
end

local args = Configuration.AppArgs
Console.WriteLine(	String.Format("Removing license header from files in {0}",args[0])	)
if not args[0] and args[1] then error("Mandatory parameters are input_directory output_directory") end
if not Directory.Exists(args[0]) then error(String.Format("Input directory {0} does not exist.",args[0])) end
if Directory.Exists(args[1]) then
Console.Write("Output directory already exists... continue and overwrite y/n? ")
local k = string.char(Console.ReadKey(false).KeyChar)
if k == 'n' then return nil end
if k ~= 'y' then error("Invalid input.") end
else
	Console.Write("\n" .. args[1].." does not exist. Creating...")	
	Directory.CreateDirectory(args[1])
	Console.Write("done.")
end
local files = Directory.GetFiles(args[0],"*",SearchOption.AllDirectories)

for i=0,files.Length-1 do	
	local file = files[i]
	
	local output_path = string.replace(file,args[0],args[1])
	if not Directory.Exists(Path.GetDirectoryName(output_path)) then
		Console.WriteLine("Path " .. Path.GetDirectoryName(output_path) .. " does not exist. Creating...") 
		Directory.CreateDirectory(Path.GetDirectoryName(output_path)) 
	end
	
	local comment_str = xcm[string.lower(Path.GetExtension(file))]
	if comment_str then
				
		writeWithoutLicense(comment_str,file, output_path)
		Console.WriteLine("Processed file " .. file)
	else
		File.Copy(file,output_path,true)
		Console.WriteLine("Skipped file " .. file)

	end	
end

Console.ReadKey(false)
