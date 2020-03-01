module Domain.FateCore

open Domain.System

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

let createSkills rank names =
    names
    |> List.map (createSkill rank)

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
    |> List.collect createStressBoxesForSkill

type Stunt = {
    Name: StuntName
    Description: string
    Skill: Skill option
    Activation: StuntActivation option
}

let internal createStunts count =
    validateCount count
    |> List.map (fun _ -> {
        Name = StuntName ""
        Description = ""
        Skill = None
        Activation = None
    })