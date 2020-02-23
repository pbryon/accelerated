module Domain.FateCore

open Domain.System

[<AutoOpen>]
module Skills =
    type SkillName = SkillName of string

    type Skill = {
        Name: SkillName
        Rank: Rank
        DeterminesStressType: StressType
    }

    let defaultSkillList = [
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
            | _ -> NA
    }
    let createSkills rank names = names |> List.map (createSkill rank)
    let createStartingSkills names = createSkills Mediocre names

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
        if skill.DeterminesStressType <> NA then
            [1..4]
            |> List.map stressBoxForskill
        else []

    let createStressBoxes skills =
        skills
        |> List.map createStressBoxesForSkill
        |> List.concat

[<AutoOpen>]
module Stunts =
    type Stunt ={
        Skill: Skill option
        Activation: StuntActivation option
    }

    let private createStunt skill activation = {
        Skill = skill
        Activation = activation
    }

    let internal createStunts count =
        match count with
        | 0 -> []
        | negative when negative < 0 -> []
        | _ -> seq{for _ in 1 .. count do
                    createStunt None None}
                    |> List.ofSeq

[<AutoOpen>]
module Campaign =
    type Campaign = {
        SkillLevel: Rank
        SkillList: string list
        Refresh: Refresh
        Stunts: int
    }

    let internal defaultCampaign = {
        SkillLevel = Mediocre;
        SkillList = defaultSkillList
        Refresh = Refresh 3
        Stunts = 3
    }

module Characters =
    type FateCoreCharacter = {
        Name : CharacterName
        Aspects: Aspect list
        Stress: StressBox list
        Skills: Skill list
        Stunts: Stunt list
        Refresh: Refresh
    }

    let createFateCoreCharacter campaign =
        let aspects = createAspects 5
        let skills = createSkills campaign.SkillLevel campaign.SkillList
        let stress  = createStressBoxes skills
        let stunts = createStunts campaign.Stunts
        {
            Name = (CharacterName "")
            Aspects = aspects
            Stress = stress
            Skills = skills
            Stunts = stunts
            Refresh = campaign.Refresh
        }

    let createDefaultFateCoreCharacter =
        createFateCoreCharacter defaultCampaign


