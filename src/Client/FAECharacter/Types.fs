module FAECharacter.Types

open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: FateAcceleratedCampaign option
    CampaignId: CampaignId
    Character: FateAcceleratedCharacter option
    Approaches: AbilityType
    NewApproach: string option
    Player: string
}

type Msg =
    | ResetCampaign
    | ResetCharacter
    | ToggleCustomApproaches
    | RenameApproach of string * string
    | InputNewApproach
    | UpdateNewApproach of string
    | AddNewApproach
    | SetRefresh of int