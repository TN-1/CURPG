luanet.load_assembly("System")
String = luanet.import_type("System.String")

function Greeting()
	if state == 0 then
		state = 1
		Player:SetLock(true)
		NPC:SetLock(true)
		s = String[1]
		s[0] = "Greetings traveller!"
		button = this:CreateButton("Continue", "CheckForNpcInteraction")
		this:ClearStory()
		this:PrintStory(s)
		this:StoryButtons(button)
	elseif state == 1 then
		state = 0
		this:ClearStory()
		Player:SetLock(false)
		NPC:SetLock(false)
	end
end