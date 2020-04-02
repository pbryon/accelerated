module Characters.Types

open Domain.Campaign

type Model = {
    Campaign: CampaignType
    CampaignId: CampaignId
    Player: string
}

type Msg =
    | ResetCampaign
    | SelectCoreCampaign
    | SelectFAECampaign