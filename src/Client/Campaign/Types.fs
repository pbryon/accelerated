module Campaign.Types

open Domain.System
open Domain.Campaign

type Model = {
    Player: PlayerName
    CampaignId: CampaignId
    CampaignType: CampaignType
    AbilityType: AbilityType
    Abilities : string list
    NewAbility: string option
    Refresh: Refresh
    FreeStunts: int option
    MaxStunts: int option
}

type Msg =
| ResetCampaign
| SelectCampaignType of CampaignType
| ToggleCustomAbilities
| RenameAbility of string * string
| InputNewAbility
| UpdateNewAbility of string
| AddNewAbility
| SetRefresh of int
| SetFreeStunts of int option
| SetMaxStunts of int option

let abilityName model =
    match model.CampaignType with
    | CampaignType.NotSelected -> "Ability"
    | CampaignType.Core -> "Skill"
    | CampaignType.FAE -> "Approach"

let abilityNamePlural model =
    match model.CampaignType with
    | CampaignType.NotSelected -> "Abilities:"
    | CampaignType.Core -> "Skills:"
    | CampaignType.FAE -> "Approaches:"