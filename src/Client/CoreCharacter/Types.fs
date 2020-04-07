module CoreCharacter.Types

open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: FateCoreCampaign option
    CampaignId: CampaignId
    Character: FateCoreCharacter option
    NewSkill: string option
    Skills: AbilityType
    Player: string
}

type Msg =
    | ResetCampaign
    | ResetCharacter
    | ToggleCustomSkills
    | RenameSkill of string * string
    | InputNewSkill
    | UpdateNewSkill of string
    | AddNewSkill
    | SetRefresh of int