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


type AspectName = AspectName of string

type Phase =
    | PhaseOne
    | PhaseTwo
    | PhaseThree

[<RequireQualifiedAccess>]
type Aspect =
| HighConcept of AspectName
| Trouble of AspectName
| PhaseTrio of Phase * AspectName
| Other of int * AspectName

let private nextAspect =
    function
    | 1 -> Aspect.HighConcept (AspectName "")
    | 2 -> Aspect.Trouble (AspectName "")
    | value -> Aspect.Other ((value - 2), AspectName "")

let internal createAspects count =
    validateCount count
    |> List.map nextAspect

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

type Refresh = Refresh of int
type StuntName = StuntName of string

type StuntActivation =
| FatePoints of int
| Scene
| Conflict
| Day
| Session

[<RequireQualifiedAccess>]
module Convert =
    let aspectName (AspectName name) = name
    let characterName (CharacterName name) = name
    let playerName (PlayerName name) = name