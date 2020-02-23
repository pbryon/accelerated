module Domain.FateAccelerated

open Domain.System

[<AutoOpen>]
module Approaches =
    type ApproachName = ApproachName of string

    type Approach = {
        Name: ApproachName
        Rank: Rank
    }

    let defaultApproaches = [
        "Careful";
        "Clever";
        "Flashy";
        "Forceful";
        "Quick";
        "Sneaky"
    ]

    let createApproaches rank names =
        names
        |> List.map (fun x -> {
            Name = ApproachName x
            Rank = rank})

[<AutoOpen>]
module Stress =
    type StressBoxType =
    | Single
    | Complex

    let createSingleStressBoxes boxes stressType =
        seq {for _ in 1 .. boxes do
                yield {
                    Type = stressType
                    Usable = true
                    Filled = false
                    Stress = Boxes 1
                }}
        |> List.ofSeq

    let createStressBox boxes stressType stressBoxType =
        match stressBoxType with
        | Single
            -> createSingleStressBoxes boxes stressType
        | Complex
            -> [{
                Type = stressType
                Usable = true
                Filled = false
                Stress = Boxes boxes
            }]

    let createStressBoxes stressTypes stressBoxType highestStressBox =
        seq {for stressType in stressTypes do
                for boxes in 1 .. highestStressBox do
                    yield createStressBox boxes stressType stressBoxType
            }
        |> List.concat

[<AutoOpen>]
module Stunts =
    type Stunt = {
        Name: StuntName
        Description: string
        Approach: Approach option
        Activation: StuntActivation option
    }

    let internal createStunts count =
        match count with
        | 0 -> []
        | negative when negative < 0 -> []
        | _ -> seq{for _ in 1 .. count do
                    yield {
                        Name = StuntName ""
                        Description = ""
                        Approach = None
                        Activation = None
                    }}
                |> List.ofSeq

[<AutoOpen>]
module Campaign =
    type FateAcceleratedCampaign = {
        ApproachLevel: Rank
        ApproachList: string list
        StressTracks : StressType list
        StressBoxType: StressBoxType
        HighestStressBox: int
        Refresh: Refresh
        Stunts: int
    }

    let internal defaultCampaign = {
        ApproachLevel = Mediocre
        ApproachList = defaultApproaches
        Refresh = Refresh 3
        StressTracks = [General]
        StressBoxType = Complex
        HighestStressBox = 3
        Stunts = 3
    }

module Characters =
    type FateAcceleratedCharacter = {
        Name: CharacterName
        Player: PlayerName
        Aspects: Aspect list
        Stress: StressBox list
        Approaches: Approach list
        Stunts: Stunt list
        Refresh: Refresh
    }

    let createFateAcceleratedCharacter campaign =
        let aspects = createAspects 5
        let approaches = createApproaches campaign.ApproachLevel campaign.ApproachList
        let stress = createStressBoxes campaign.StressTracks campaign.StressBoxType campaign.HighestStressBox
        let stunts = createStunts campaign.Stunts
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
        createFateAcceleratedCharacter defaultCampaign