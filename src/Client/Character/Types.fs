module Character.Types

open Domain.Campaign
open Domain.System

type Model = {
    Campaign: Campaign option
    CampaignId: CampaignId
    Player: PlayerName
}

type Msg =
    | ResetCharacter of Campaign