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
        |> List.map (fun name -> {
            Name = ApproachName name
            Rank = rank})

[<AutoOpen>]
module Stress =
    type StressBoxType =
    | Single
    | Complex

    let createSingleStressBoxes stressType boxes  =
        [1 .. boxes]
        |> List.map (fun _ -> {
            Type = stressType
            Usable = true
            Filled = false
            Stress = Boxes 1
        })

    let createStressBox stressType stressBoxType boxes =
        match stressBoxType with
        | Single
            -> createSingleStressBoxes stressType boxes
        | Complex
            -> [{
                Type = stressType
                Usable = true
                Filled = false
                Stress = Boxes boxes
            }]

    let createStressBoxes stressTypes stressBoxType highestStressBox =
        stressTypes
        |> List.collect (fun stressType ->
            validateCount highestStressBox
            |> List.collect (createStressBox stressType stressBoxType)
        )

[<AutoOpen>]
module Stunts =
    type Stunt = {
        Name: StuntName
        Description: string
        Approach: Approach option
        Activation: StuntActivation option
    }

    let internal createStunts count =
        validateCount count
        |> List.map (fun _ -> {
            Name = StuntName ""
            Description = ""
            Approach = None
            Activation = None
        })

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