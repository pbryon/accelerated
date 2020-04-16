module Character.Abilities

open System.Text.RegularExpressions

open Feliz
open Feliz.Bulma

open Utils

open Domain
open Domain.System
open Domain.Campaign
open Domain.SystemReference

open Character.Types
open App.Views.Controls
open App.Views.Layouts

type RankValidation = {
    Rank: int
    Used: int
    Available: int
    MinimumRank: int
    IsErrored: bool
}

let private getHelpTopic model forCore forFae =
    match model.Campaign with
    | None ->
        Html.none
    | Some (Campaign.Core _) ->
        rulesButton "" forCore
    | Some (Campaign.FAE _) ->
        rulesButton "" forFae

let private abilityNamePlural model =
    match model.Campaign with
    | None -> ""
    | Some (Campaign.Core _) -> "Skills"
    | Some (Campaign.FAE _) -> "Approaches"

let private usesSkills model =
    match model.Campaign with
    | Some (Campaign.Core _) -> true
    | _ -> false

let private findUsedAbility model (name: string) =
    model.Abilities
    |> firstBy (fun x -> x.Name = name && x.Rank <> Default)

let private abilityRank (ability: Ability) =
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

let minimumRank model =
    match model.Campaign with
    | None -> 0
    | Some (Campaign.Core campaign) ->
        rankValue campaign.SkillLevel
    | Some (Campaign.FAE campaign) ->
        rankValue campaign.ApproachLevel

let private defaultRanks model =
    match model.Campaign with
    | None -> []
    | Some (Campaign.Core _) -> FateCore.defaultSkillRanks
    | Some (Campaign.FAE _) -> FateAccelerated.defaultApproachRanks
    |> List.sort
    |> List.rev

let private allRanks model  =
    model.Abilities
    |> List.map abilityRank
    |> List.append (defaultRanks model)
    |> List.distinct
    |> List.sort
    |> List.rev

let private abilitiesByRank model ability =
    model.Abilities
    |> List.filter (fun x -> abilityRank x = abilityRank ability)

let private isAbilityErrored model ability =
    abilitiesByRank model ability
    |> containsBy (fun x ->
        match x.Rank with
        | Errored _ -> true
        | _ -> false
        )

let private validateRank ranks sortedAbilities minimumRank rank : RankValidation =
    let withSameRank list = list |> List.filter (fun x -> x = rank)

    let count = (ranks |> withSameRank).Length

    let used = sortedAbilities |> withSameRank

    {
        Used = used.Length
        Rank = rank
        Available = count
        MinimumRank = minimumRank
        IsErrored =
            if rank = minimumRank
            then false
            else count = 0 || used.Length > count
    }

let private validateNewRank model rank =
    let ranks = defaultRanks model
    let sortedAbilities = sortAbilitiesByRank model
    let minimumRank = minimumRank model
    validateRank ranks sortedAbilities minimumRank rank

let private validateRanks ranks model : RankValidation list =
    let minimumRank = minimumRank model
    let sortedAbilities = sortAbilitiesByRank model

    [ minimumRank ]
    |> List.append ranks
    |> List.distinct
    |> List.map (validateRank ranks sortedAbilities minimumRank)

module State =
    let private createAbility model name  =
        {
            Rank = Ok (minimumRank model)
            Name = name
        }

    let addAbilities model =
        let abilities =
            match model.Campaign with
            | None -> []
            | Some (Campaign.Core campaign) ->
                campaign.SkillList
                |> List.map (createAbility model)
            | Some (Campaign.FAE campaign) ->
                campaign.ApproachList
                |> List.map (createAbility model)

        { model with Abilities = abilities }

    let private replaceAbility ability model =
        let replaceWith (ability: Ability) (item: Ability) =
            if (item.Name = ability.Name)
            then ability
            else item

        { model with
            Abilities =
                model.Abilities
                |> List.map (replaceWith ability)
        }

    let private setErrored result item =
        let sameRank = abilityRank item = result.Rank

        match result.IsErrored with
        | true when result.Rank = result.MinimumRank ->
            item
        | true when sameRank ->
            { item with Rank = Errored (abilityRank item) }
        | false when sameRank ->
            { item with Rank = Ok (abilityRank item) }
        | _ ->
            item

    let private validateAbility model result =
        let sameRank result ability =
            abilityRank ability = result.Rank

        model.Abilities
        |> List.filter (sameRank result)
        |> List.map (setErrored result)

    let private validateAbilities model =
        let resultFor results rank =
            let found =
                results
                |> firstBy (fun x -> x.Rank = rank)
            match found with
            | Some result ->
                result
            | None ->
                validateNewRank model rank

        let ranks = defaultRanks model
        let results = validateRanks ranks model

        let abilities =
            allRanks model
            |> List.collect (fun rank ->
                let result = resultFor results rank
                validateAbility model result
            )
            |> List.sortBy (fun x -> x.Name)

        { model with Abilities = abilities }

    let onUpdateAbility model (ability: Ability) =
        match findUsedAbility model ability.Name with
        | None -> model

        | Some _ ->
            model
            |> replaceAbility ability
            |> validateAbilities

    let allAbilitiesAssigned model =
        let ranks = allRanks model
        validateRanks ranks model
        |> List.exists (fun result ->
            result.Rank <> result.MinimumRank
            && result.Used = result.Available
        )

    let allAbilitiesValid model =
        let validated = validateAbilities model

        validated.Abilities
        |> noneExist (isAbilityErrored validated)

module View =
    let abilityNameWidth = style.width 150

    let private rankButton result =
        [1 .. result.Available]
        |> List.map (fun currentTotal ->
            let isUsed = result.Used >= currentTotal

            Bulma.button [
                prop.text (string result.Rank)
                prop.tabIndex -1

                if result.IsErrored
                then button.isDanger
                else button.isPrimary

                if isUsed
                then button.isLight
                else button.isActive
            ]
        )

    let private abilitySummary model =
        let ranks = defaultRanks model
        let allAssigned = State.allAbilitiesAssigned model

        let ranks =
            validateRanks ranks model
            |> List.collect rankButton
        let description =
            Bulma.button [
                prop.tabIndex -1
                prop.className "ability-summary"
                button.isWhite
                prop.text "Available ranks"
                if not allAssigned then
                    yield! [
                        button.isDanger
                        prop.title "Please assign all abilities"
                    ]
            ]

        let buttons = addonGroup "ability-summary" [
            description
            yield! ranks
        ]

        [
            buttons
            getHelpTopic model Topic.PickSkills Topic.PickApproaches
        ]

    let private updateAbility ability dispatch text =
        let parsedRank =
            text
            |> getLast 1
            |> parseInt

        match parsedRank with
        | None -> ()
        | Some rank ->
            UpdateAbility {
                ability with Rank = (Ok rank)
            }
            |> dispatch

    let private abilityInputs dispatch model ability =
        let value = string (abilityRank ability)
        let isErrored = isAbilityErrored model ability

        let columnWidth =
            if usesSkills model
            then column.isOneThird
            else column.isHalf

        Bulma.column [
            columnWidth
            prop.children [
                addonGroup "edit-ability" [
                    addonButton ability.Name abilityNameWidth
                    Bulma.textInput [
                        text.hasTextCentered
                        prop.value value
                        prop.className "ability-rank"
                        prop.pattern (Regex "^[0-9]+$")
                        onFocusSelectText
                        prop.onTextChange (updateAbility ability dispatch)
                        if isErrored then
                            input.isDanger
                    ]
                ]
            ]
        ]

    let abilityFields dispatch model =
        match model.Campaign with
        | None ->
            []
        | Some (Campaign.Core _) ->
            model.Abilities
            |> List.map (abilityInputs dispatch model)
        | Some (Campaign.FAE _) ->
            model.Abilities
            |> List.map (abilityInputs dispatch model)

    let chooseAbilities dispatch model =
        let abilityName = abilityNamePlural model

        let fields = abilityFields dispatch model

        let columnWidth =
            if usesSkills model
            then column.is8
            else column.is5

        if fields.Length = 0
        then Html.none
        else colLayout [
            labelCol [
                Bulma.label abilityName
                getHelpTopic model Topic.Skills Topic.Approaches
            ]
            {
                Props = [ columnWidth ]
                Content = [
                    yield! abilitySummary model
                    Bulma.columns [
                        columns.isMultiline
                        prop.children fields
                    ]
                ]
            }
        ]