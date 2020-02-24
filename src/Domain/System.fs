module Domain.System

type CharacterName = CharacterName of string
type PlayerName = PlayerName of string

[<AutoOpen>]
module Utils =
    let validateCount =
        function
        | negative when negative < 0 -> []
        | 0 -> []
        | count -> [1 .. count]

[<AutoOpen>]
module Aspects =
    type AspectName = AspectName of string

    type Aspect =
    | HighConcept of AspectName
    | Trouble of AspectName
    | Other of AspectName

    let private nextAspect =
        function
        | 1 -> HighConcept (AspectName "")
        | 2 -> Trouble (AspectName "")
        | _ -> Other (AspectName "")

    let internal createAspects count =
        validateCount count
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

    let rankValue =
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
    type Boxes = Boxes of int
    type ConsequenceName = ConsequenceName of string

    type StressType =
    | Physical
    | Mental
    | General
    | NA

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

[<AutoOpen>]
module Stunts =
    type Refresh = Refresh of int
    type StuntName = StuntName of string

    type StuntActivation =
    | FatePoints of int
    | Scene
    | Conflict
    | Day
    | Session