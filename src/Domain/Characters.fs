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
    match campaign with
    | None ->
        None
    | Some c->
        let aspects = createAspects 5
        let skills = createSkills c.SkillLevel c.SkillList
        let stress  = FateCore.createStressBoxes skills
        let stunts = FateCore.createStunts c.Stunts
        Some {
            Name = CharacterName ""
            Player = PlayerName ""
            Aspects = aspects
            Stress = stress
            Skills = skills
            Stunts = stunts
            Refresh = c.Refresh
        }

let createDefaultCoreCharacter =
    createCoreCharacter (Some defaultCoreCampaign)

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
    match campaign with
    | None ->
        None

    | Some c ->
        let aspects = createAspects 5
        let approaches = createApproaches c.ApproachLevel c.ApproachList
        let stress = FateAccelerated.createStressBoxes c.StressTracks c.StressBoxType c.HighestStressBox
        let stunts = FateAccelerated.createStunts c.Stunts
        Some {
            Name = CharacterName ""
            Player = PlayerName ""
            Aspects = aspects
            Approaches = approaches
            Stress = stress
            Stunts = stunts
            Refresh = c.Refresh
        }

let createDefaultFateAcceleratedCharacter =
    createFAECharacter (Some defaultFAECampaign)

type PlayerCharacter =
    | Core of FateCoreCharacter
    | FAE of FateAcceleratedCharacter