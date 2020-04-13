module Character.Abilities

open Feliz

open Utils

open Domain
open Domain.Campaign

open Character.Types

let abilityNameWidth = style.width 200
let abilityRankWidth = style.width 40

let private abilityNamePlural model =
    match model.Campaign with
    | None -> ""
    | Some (Campaign.Core _) -> "Skills"
    | Some (Campaign.FAE _) -> "Approaches"

let private findAbility model ability =
    model.Abilities
    |> findBy (fun x -> x.Name = ability)

let private findUsedAbility model (name: string) =
    model.Abilities
    |> findBy (fun x -> x.Name = name && x.Rank <> Default)

let private abilityRank ability =
    match ability.Rank with
    | Ok rank -> rank
    | Errored rank -> rank
    | Default -> 0

let private sortAbilitiesByRank model =
    model.Abilities
    |> List.map abilityRank
    |> List.filter (fun x -> x > 0)
    |> List.sort
    |> List.rev

let private defaultRanks model =
    match model.Campaign with
    | None -> []
    | Some (Campaign.Core _) -> FateCore.defaultSkillRanks
    | Some (Campaign.FAE _) -> FateAccelerated.defaultApproachRanks
    |> List.sort
    |> List.rev

let private usedRankCounts model =
    sortAbilitiesByRank model
    |> List.groupBy id
    |> List.map (fun (key, list) -> (key, list.Length))

module State =
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

module View =
    open Feliz.Bulma

    open App.Icons
    open App.Views.Layouts
    open App.Views.Buttons

    let private rankButton ranks sortedAbilities rank =
        let withSameRank list = list |> List.filter (fun x -> x = rank)

        let count = (ranks |> withSameRank).Length
        let used = sortedAbilities |> withSameRank

        [1 .. count]
        |> List.map (fun currentTotal ->
            let isUsed = used.Length >= currentTotal
            let tooMany = used.Length > count

            {
                Text = string rank
                Active = not isUsed
                Color =
                    if tooMany then
                        button.isDanger
                    else
                        button.isPrimary
                OnClick = (fun _ -> ())
            }
        )


    let private rankSummary model =
        let ranks = defaultRanks model
        let sortedAbilities = sortAbilitiesByRank model

        ranks
        |> List.distinct
        |> List.collect (rankButton ranks sortedAbilities)

    let private abilitySummary dispatch model =
        let buttons = rankSummary model
        Bulma.field [
            field.isHorizontal
            prop.children [
                Bulma.fieldLabel [
                    Bulma.label [
                        prop.text "Available ranks:"
                        prop.style [
                            style.fontWeight.normal
                            style.display.inlineElement
                        ]
                    ]
                ]
                Bulma.fieldBody [
                    yield! buttonGroup buttons
                ]
            ]
        ]

    let chooseAbilities dispatch model =
        let abilityName = abilityNamePlural model

        let setAbilityRanks =
            match model.Campaign with
            | None
            | Some (Campaign.Core _) ->
                []
            | Some (Campaign.FAE _) ->
                []//setApproachRanks dispatch model

        if setAbilityRanks.Length = 0
        then
            Html.none
        else
            colLayout [
                labelCol [ Bulma.label (abilityName + ":") ]
                {
                    Size = [ column.is8 ]
                    Align = style.textAlign.left
                    Content = [
                        abilitySummary dispatch model
                        yield! setAbilityRanks
                    ]
                }
            ]