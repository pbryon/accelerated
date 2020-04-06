module Domain.Characters

open Domain.System
open Domain.FateCore
open Domain.FateAccelerated
open Domain.Campaign

type FateCoreCharacter = {
    Name : CharacterName
    Player: PlayerName
    Aspects: Aspect list
    Stress: StressBox list
    Skills: Skill list
    Stunts: FateCore.Stunt list
    Refresh: Refresh
}

let createCoreCharacter campaign =
    let aspects = createAspects 5
    let skills = createSkills campaign.SkillLevel campaign.SkillList
    let stress  = FateCore.createStressBoxes skills
    let stunts = FateCore.createStunts campaign.Stunts
    {
        Name = CharacterName ""
        Player = PlayerName ""
        Aspects = aspects
        Stress = stress
        Skills = skills
        Stunts = stunts
        Refresh = campaign.Refresh
    }

let createDefaultCoreCharacter =
    createCoreCharacter defaultCoreCampaign

type FateAcceleratedCharacter = {
        Name: CharacterName
        Player: PlayerName
        Aspects: Aspect list
        Stress: StressBox list
        Approaches: Approach list
        Stunts: Stunt list
        Refresh: Refresh
    }

let createFAECharacter campaign =
    let aspects = createAspects 5
    let approaches = createApproaches campaign.ApproachLevel campaign.ApproachList
    let stress = FateAccelerated.createStressBoxes campaign.StressTracks campaign.StressBoxType campaign.HighestStressBox
    let stunts = FateAccelerated.createStunts campaign.Stunts
    {
        Name = CharacterName ""
        Player = PlayerName ""
        Aspects = aspects
        Approaches = approaches
        Stress = stress
        Stunts = stunts
        Refresh = campaign.Refresh
    }

let createDefaultFateAcceleratedCharacter =
    createFAECharacter defaultFAECampaign

type PlayerCharacter =
    | Core of FateCoreCharacter
    | FAE of FateAcceleratedCharacter