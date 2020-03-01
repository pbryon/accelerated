namespace Shared

open Domain.System
open Domain.FateCore
open Domain.FateAccelerated
open Domain.Campaign
open Domain.Characters

type Model = {
    Campaign: CampaignType
}

type Msg =
    | ResetCampaign
    | SelectCoreCampaign
    | SelectFAECampaign