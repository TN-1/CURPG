luanet.load_assembly("System")
String = luanet.import_type("System.String")
s = String[2]
s[0] = "Hello from-"
s[1] = "Lua!"
this:PrintStory(s)