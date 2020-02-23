module Domain.FateCore

open Domain.System

[<AutoOpen>]
module Skills =
    type SkillName = SkillName of string with
        member x.Value = let (SkillName value) = x in value

    type Skill = {
        Name: SkillName
        Rank: Rank
        DeterminesStressType: StressType
    }

    let defaultSkills = [
        "Athletics";
        "Burglary";
        "Contacts";
        "Crafts";
        "Deceive";
        "Drive";
        "Empathy";
        "Fight";
        "Investigate";
        "Lore";
        "Notice";
        "Physique";
        "Provoke";
        "Rapport";
        "Resources";
        "Shoot";
        "Stealth";
        "Will"
    ]

    let createSkill rank name = {
        Name = SkillName name;
        Rank = rank;
        DeterminesStressType =
            match name with
            | "Physique" -> Physical
            | "Will" -> Mental
            | _ -> None
    }
    let createSkills rank names = names |> List.map (createSkill rank)

[<AutoOpen>]
module Stress =
    let stressBoxAvailableAt skill stress =
        match skill.Rank with
        | Terrible
        | Poor
        | Mediocre
            -> stress <= 2
        | Fair
        | Good
            -> stress <= 3
        | _ -> stress <= 4

    let createStressBox skill boxes =
        {
            Type = skill.DeterminesStressType
            Usable = stressBoxAvailableAt skill boxes
            Filled = false
            Stress = (Boxes boxes)
        }

    let createStressBoxesForSkill skill =
        let stressBoxForskill = createStressBox skill
        if skill.DeterminesStressType <> None then
            [1..4]
            |> List.map stressBoxForskill
        else []

    let createStressBoxes skills =
        skills
        |> List.map createStressBoxesForSkill
        |> List.concat

module Characters =
    type CoreCharacter = {
        Name : CharacterName
        Aspects: Aspect list
        Stress: StressBox list
        Skills: Skill list
    }

    let createCoreCharacter =
        let aspects = createAspects 5
        let skills = createSkills Mediocre defaultSkills
        let stress  = createStressBoxes skills
        {
            Name = (CharacterName "")
            Aspects = aspects
            Stress = stress
            Skills = skills
        }
