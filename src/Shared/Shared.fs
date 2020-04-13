namespace Shared

open Domain.Campaign

type Model = {
    Campaign: CampaignType
}

type Msg =
    | ResetCampaign
    | SelectCoreCampaign
    | SelectFAECampaign