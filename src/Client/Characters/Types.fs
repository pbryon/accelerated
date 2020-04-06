module Characters.Types

open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: Campaign option
    CampaignType: CampaignType
    CampaignId: CampaignId
    Abilities: AbilityType
    Character: PlayerCharacter option
    Player: string
}

type Msg =
    | ResetCampaign
    | SelectCoreCampaign
    | SelectFAECampaign
    | ToggleCustomAbilities
    | RenameAbility of string * string