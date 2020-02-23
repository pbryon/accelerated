namespace Shared

module Core =
    open Shared.System

    type Skill = string * Rank
    type CoreCharacter = {
        Name : CharacterName
        Aspects: Aspect list
        Stress: StressBox list
        Skills: Skill list
    }

    let createCharacter = {
        Name = (CharacterName "")
        Aspects = [
            HighConcept (AspectName "")
            Trouble (AspectName "")
        ]
        Stress = []
        Skills = []
    }

module Accelerated =
    open Aspects

    type Approach = string * Rank