module Domain.Campaign

open Domain.System
open Domain.FateCore
open Domain.FateAccelerated

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

type CampaignType =
    | Core of FateCoreCampaign
    | FAE of FateAcceleratedCampaign
    | NotSelected

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