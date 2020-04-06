module CoreCharacter.Types

open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: FateCoreCampaign option
    CampaignId: CampaignId
    Character: FateCoreCharacter option
    Skills: AbilityType
    Player: string
}

type Msg =
    | ResetCampaign
    | ToggleCustomSkills
    | RenameSkill of string * string
    | AddNewSkill