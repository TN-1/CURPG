luanet.load_assembly("System")

function Greeting()
	if state.NPC0 == 0 then
		state.NPC0 = 1
		Player:SetLock(true)
		NPC:SetLock(true)
		button = this:CreateButton("Continue", "CheckForNpcInteraction")
		this:ClearStory()
		this:PrintStory("Greetings Traveller")
		this:StoryButtons(button)
	elseif state.NPC0 == 1 then
		state.NPC0 = 0
		this:ClearStory()
		Player:SetLock(false)
		NPC:SetLock(false)
	end
end