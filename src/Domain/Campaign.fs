module Domain.Campaign

open System
open Domain.FateCore
open Domain.FateAccelerated

type CampaignId = CampaignId of Guid

type FateCoreCampaign = {
    SkillLevel: Rank
    SkillList: string list
    StressTracks : StressType list
    StressBoxType: StressBoxType
    Refresh: Refresh
    FreeStunts: int
    MaxStunts: int option
}

let defaultCoreCampaign = {
    SkillLevel = Mediocre;
    SkillList = defaultSkillList
    StressTracks = [ Physical; Mental ]
    StressBoxType = Complex
    Refresh = Refresh 3
    FreeStunts = 0
    MaxStunts = Some 3
}

type FateAcceleratedCampaign = {
    ApproachLevel: Rank
    ApproachList: string list
    StressTracks : StressType list
    StressBoxType: StressBoxType
    HighestStressBox: int
    Refresh: Refresh
    FreeStunts: int
    MaxStunts: int option
}

let defaultFAECampaign = {
    ApproachLevel = Mediocre
    ApproachList = defaultApproaches
    Refresh = Refresh 3
    StressTracks = [General]
    StressBoxType = Complex
    HighestStressBox = 3
    FreeStunts = 0
    MaxStunts = Some 3
}

[<RequireQualifiedAccess>]
type CampaignType =
    | Core
    | FAE

[<RequireQualifiedAccess>]
type AbilityType =
    | Custom
    | Default

[<RequireQualifiedAccess>]
type Campaign =
    | Core of FateCoreCampaign
    | FAE of FateAcceleratedCampaign

type AspectSelection =
    | HighConceptAndTrouble
    | ExtraAspects of int
    | PhaseTrio