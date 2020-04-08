module Domain.Campaign

open System
open Domain.FateCore
open Domain.FateAccelerated

type CampaignId = CampaignId of Guid

type FateCoreCampaign = {
    SkillLevel: Rank
    SkillList: string list
    Refresh: Refresh
    Stunts: int
}

type FateAcceleratedCampaign = {
    ApproachLevel: Rank
    ApproachList: string list
    StressTracks : StressType list
    StressBoxType: StressBoxType
    HighestStressBox: int
    Refresh: Refresh
    Stunts: int
}

[<RequireQualifiedAccess>]
type CampaignType =
    | Core
    | FAE
    | NotSelected

[<RequireQualifiedAccess>]
type AbilityType =
    | Custom
    | Default

type Campaign =
    | Core of FateCoreCampaign option
    | FAE of FateAcceleratedCampaign option

let defaultCoreCampaign = {
    SkillLevel = Mediocre;
    SkillList = defaultSkillList
    Refresh = Refresh 3
    Stunts = 3
}

let defaultFAECampaign = {
    ApproachLevel = Mediocre
    ApproachList = defaultApproaches
    Refresh = Refresh 3
    StressTracks = [General]
    StressBoxType = Complex
    HighestStressBox = 3
    Stunts = 3
}