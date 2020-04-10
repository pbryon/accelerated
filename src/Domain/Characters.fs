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

type FateAcceleratedCharacter = {
        Name: CharacterName
        Player: PlayerName
        Aspects: Aspect list
        Stress: StressBox list
        Approaches: Approach list
        Stunts: Stunt list
        Refresh: Refresh
    }

type PlayerCharacter =
    | Core of FateCoreCharacter
    | FAE of FateAcceleratedCharacter