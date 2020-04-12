module Character.Types.Abilities

open Utils

open Domain
open Domain.Campaign

open Character.Types

let abilityNamePlural model =
    match model.Campaign with
    | None -> ""
    | Some (Campaign.Core _) -> "Skills"
    | Some (Campaign.FAE _) -> "Approaches"

let findAbility model ability =
    model.Abilities
    |> findBy (fun x -> x.Name = ability)

let findUsedAbility model (name: string) =
    model.Abilities
    |> findBy (fun x -> x.Name = name && x.Rank <> Default)

let abilityRank ability =
    match ability.Rank with
    | Ok rank -> rank
    | Errored rank -> rank
    | Default -> 0

let sortAbilitiesByRank model =
    model.Abilities
    |> List.map abilityRank
    |> List.filter (fun x -> x > 0)
    |> List.sort
    |> List.rev

let defaultRanks model =
    match model.Campaign with
    | None -> []
    | Some (Campaign.Core _) -> FateCore.defaultSkillRanks
    | Some (Campaign.FAE _) -> FateAccelerated.defaultApproachRanks
    |> List.sort
    |> List.rev

let usedRankCounts model =
    sortAbilitiesByRank model
    |> List.groupBy id
    |> List.map (fun (key, list) -> (key, list.Length))

let private createAbility name =
    {
        Rank = Default
        Name = name
    }

let addAbilities model =
    let abilities =
        match model.Campaign with
        | None -> []
        | Some (Campaign.Core campaign) ->
            campaign.SkillList
        | Some (Campaign.FAE campaign) ->
            campaign.ApproachList
        |> List.map createAbility

    { model with Abilities = abilities }
