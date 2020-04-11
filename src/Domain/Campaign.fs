module Domain.Campaign

open System
open Domain.FateCore
open Domain.FateAccelerated

type CampaignId = CampaignId of Guid

[<RequireQualifiedAccess>]
type CampaignType =
    | Core
    | FAE

[<RequireQualifiedAccess>]
type AbilityType =
    | Custom
    | Default

type AspectSelection =
    | HighConceptAndTrouble
    | ExtraAspects of int
    | PhaseTrio

type FateCoreCampaign = {
    SkillLevel: Rank
    SkillList: string list
    Aspects: AspectSelection list
    StressTracks : StressType list
    StressBoxType: StressBoxType
    Refresh: Refresh
    FreeStunts: int
    MaxStunts: int option
}

let defaultCoreCampaign = {
    SkillLevel = Mediocre;
    SkillList = defaultSkillList
    Aspects = [ HighConceptAndTrouble; PhaseTrio ]
    StressTracks = [ Physical; Mental ]
    StressBoxType = Complex
    Refresh = Refresh 3
    FreeStunts = 0
    MaxStunts = Some 3
}


type FateAcceleratedCampaign = {
    ApproachLevel: Rank
    ApproachList: string list
    Aspects: AspectSelection list
    StressTracks : StressType list
    StressBoxType: StressBoxType
    HighestStressBox: int
    Refresh: Refresh
    FreeStunts: int
    MaxStunts: int option
}

let defaultFAECampaign = {
    ApproachLevel = Mediocre
    ApproachList = defaultApproachList
    Aspects = [ HighConceptAndTrouble; ExtraAspects 1 ]
    Refresh = Refresh 3
    StressTracks = [General]
    StressBoxType = Complex
    HighestStressBox = 3
    FreeStunts = 0
    MaxStunts = Some 3
}

[<RequireQualifiedAccess>]
type Campaign =
    | Core of FateCoreCampaign
    | FAE of FateAcceleratedCampaign