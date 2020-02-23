module Domain.System

type CharacterName = CharacterName of string

[<AutoOpen>]
module Aspects =
    type AspectName = AspectName of string with
        static member empty = (AspectName "")
        member x.Value = let (AspectName value) = x in value

    type Aspect =
    | HighConcept of AspectName
    | Trouble of AspectName
    | Other of AspectName

    let private nextAspect =
        function
        | 1 -> HighConcept AspectName.empty
        | 2 -> Trouble AspectName.empty
        | _ -> Other AspectName.empty

    let createAspects count =
        match count with
        | 0 -> []
        | negative when negative < 0 -> []
        | _ -> [ 1 .. count ]
            |> List.map nextAspect

[<AutoOpen>]
module Ladder =
    type Rank =
    | Legendary
    | Epic
    | Fantastic
    | Superb
    | Great
    | Good
    | Fair
    | Average
    | Mediocre
    | Poor
    | Terrible

    let ladderRank =
        function
        | Legendary -> 8
        | Epic -> 7
        | Fantastic -> 6
        | Superb -> 5
        | Great -> 4
        | Good -> 3
        | Fair -> 2
        | Average -> 1
        | Mediocre -> 0
        | Poor -> -1
        | Terrible -> -2

[<AutoOpen>]
module Stress =
    type Boxes = Boxes of int with
        member x.Value = let (Boxes value) = x in value

    type ConsequenceName = ConsequenceName of string with
        static member empty = (ConsequenceName "")
        member x.Value = let (ConsequenceName value) = x in value

    type StressType =
    | Physical
    | Mental
    | General
    | None

    type StressBox = {
        Type: StressType
        Usable: bool
        Filled: bool
        Stress: Boxes
    }

    type ConsequenceType =
    | Mild
    | Moderate
    | Severe

    type Consequence = {
        Type: ConsequenceType
        StressType: StressType
        Name: ConsequenceName
        Stress: Boxes
        Available: bool
    }

    let consequenceBoxes consequence =
        match consequence.Type with
        | Mild _ -> 2
        | Moderate _ -> 4
        | Severe _ -> 6

    let consequenceName consequence =
        let toString (ConsequenceName name) = name
        toString consequence.Name

    let toString consequence =
        let name = consequenceName consequence
        let value = consequenceBoxes consequence
        sprintf "%s (%d)" name value

    let createStressBox stressType stress = {
        Stress = stress
        Usable = false
        Filled = false
        Type = stressType
    }

    let createConsequence consequenceType stressType boxes = {
        Type = consequenceType
        StressType = stressType
        Stress = boxes
        Name = ConsequenceName.empty
        Available = false
    }