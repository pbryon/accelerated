module Character.Abilities

open Feliz

open Utils

open Domain
open Domain.Campaign

open Character.Types

type RankValidation = {
    Rank: int
    Used: int
    Count: int
    MinimumRank: int
    IsErrored: bool
}

let private abilityNamePlural model =
    match model.Campaign with
    | None -> ""
    | Some (Campaign.Core _) -> "Skills"
    | Some (Campaign.FAE _) -> "Approaches"

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
        System.rankValue campaign.SkillLevel
    | Some (Campaign.FAE campaign) ->
        System.rankValue campaign.ApproachLevel

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
        Count = count
        MinimumRank = minimumRank
        IsErrored = count = 0 || used.Length > count
    }

let private validateRanks model : RankValidation list =
    let ranks = defaultRanks model
    let minimumRank = minimumRank model
    let sortedAbilities = sortAbilitiesByRank model

    [ minimumRank ]
    |> List.append ranks
    |> List.distinct
    |> List.map (validateRank ranks sortedAbilities minimumRank)

module State =
    open Global

    let private createAbility name =
        {
            Rank = Default
            Name = name
        }

    let private createAbilityOk rank name =
     {
         Rank = Ok rank
         Name = name
     }

    let addAbilities model =
        let minimumRank = minimumRank model
        let abilities =
            match model.Campaign with
            | None -> []
            | Some (Campaign.Core campaign) ->
                campaign.SkillList
                |> List.map createAbility
            | Some (Campaign.FAE campaign) ->
                campaign.ApproachList
                |> List.map (createAbilityOk minimumRank)

        { model with Abilities = abilities }

    let private replaceAbility ability model =
        let replaceWith ability item =
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

    let private validateAbility model resultOption =
        let sameRank result ability =
            abilityRank ability = result.Rank

        match resultOption with
        | None -> []

        | Some result ->
            model.Abilities
            |> List.filter (sameRank result)
            |> List.map (setErrored result)

    let private validateAbilities model =
        let resultFor results rank =
            results
            |> firstBy (fun x -> x.Rank = rank)

        let results = validateRanks model

        let abilities =
            allRanks model
            |> List.collect (fun rank ->
                let result = resultFor results rank
                validateAbility model result
            )
            |> List.sortBy (fun x -> x.Name)

        { model with Abilities = abilities }

    let updateAbility model ability =
        match findUsedAbility model ability.Name with
        | None -> model

        | Some _ ->
            model
            |> replaceAbility ability
            |> validateAbilities

    let allAbilitiesAssigned model =
        validateRanks model
        |> List.exists (fun result ->
            result.Rank <> result.MinimumRank
            && result.Used = result.Count
        )

module View =
    open System.Text.RegularExpressions

    open Feliz.Bulma

    open Global
    open App.Views.Layouts
    open App.Views.Buttons

    let abilityNameWidth = style.width 150
    let abilityRankWidth = style.width 40

    let private rankButton result =
        [1 .. result.Count]
        |> List.map (fun currentTotal ->
            let isUsed = result.Used >= currentTotal

            {
                Text = string result.Rank
                Active = not isUsed
                Color =
                    if result.IsErrored then
                        button.isDanger
                    else
                        button.isPrimary
                OnClick = (fun _ -> ())
            }
        )

    let private rankSummary model =
        validateRanks model
        |> List.collect rankButton

    let private abilitySummary model =
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

    let private onUpdateAbility ability dispatch text =
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

    let private approachInputs dispatch model ability =
        let value = string (abilityRank ability)
        let isErrored = isAbilityErrored model ability

        Bulma.column [
            column.isHalf
            prop.children [
                addonGroup [
                    addonButton ability.Name abilityNameWidth
                    Bulma.textInput [
                        text.hasTextCentered
                        prop.value value
                        prop.style [ abilityRankWidth ]
                        prop.pattern (Regex "^[0-9]+$")
                        prop.onTextChange (onUpdateAbility ability dispatch)
                        if isErrored then
                            input.isDanger
                    ]
                ]
            ]
        ]

    let private selectApproachRanks dispatch model =
        let fields =
            model.Abilities
            |> List.map (approachInputs dispatch model)

        [
            colLayout [
                {
                    Size = [ column.is8 ]
                    Align = style.textAlign.left
                    Content = [
                        Bulma.columns [
                            columns.isMultiline
                            prop.children fields
                        ]
                    ]
                }
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
                selectApproachRanks dispatch model

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
                        abilitySummary model
                        yield! setAbilityRanks
                    ]
                }
            ]