luanet.load_assembly("System")

function Greeting()
	if state == 0 then
		state = 1
		Player:SetLock(true)
		NPC:SetLock(true)
		button = this:CreateButton("Continue", "CheckForNpcInteraction")
		this:ClearStory()
		this:PrintStory("Greetings Traveller")
		this:StoryButtons(button)
	elseif state == 1 then
		state = 0
		this:ClearStory()
		Player:SetLock(false)
		NPC:SetLock(false)
	end
end