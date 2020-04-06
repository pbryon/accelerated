module FAECharacter.Types

open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: FateAcceleratedCampaign option
    CampaignId: CampaignId
    Character: FateAcceleratedCharacter option
    Approaches: AbilityType
    Player: string
}

type Msg =
    | ResetCampaign
    | ToggleCustomApproaches
    | RenameApproach of string * string