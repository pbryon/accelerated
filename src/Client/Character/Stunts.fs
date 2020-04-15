module Character.Stunts

open Feliz

open Utils

open Domain
open Domain.System
open Domain.Campaign

open Character.Types
open App.Icons
open Fable.Core

let private refreshValue (Refresh refresh) = refresh

let private initialRefresh model =
    match model.Campaign with
    | None -> 0
    | Some (Campaign.Core campaign) ->
        refreshValue campaign.Refresh
    | Some (Campaign.FAE campaign) ->
        refreshValue campaign.Refresh

let private abilityName model =
    match model.Campaign with
    | None -> ""
    | Some (Campaign.Core _) -> "Skill"
    | Some (Campaign.FAE _) -> "Approach"

let private abilityRank (ability: Ability) =
    match ability.Rank with
    | Ok rank -> rank
    | Errored rank -> rank
    | Default -> 0

let private compareAblityRank (fst: Ability) (snd: Ability) =
        if abilityRank fst > abilityRank snd then -1
        elif abilityRank snd > abilityRank fst then 1
        else compare fst.Name snd.Name

let private abilitiesWithRanks model =
    model.Abilities
    |> List.sortWith compareAblityRank
    |> List.map (fun ability ->
        let text = sprintf "%s +%i" ability.Name (abilityRank ability)
        let value = ability.Name
        (value, text)
    )

let private describeAction action =
    match action with
    | CreateAdvantage -> "Create advantage"
    | _ -> string action

let private describeActivation activationOption =
    match activationOption with
    | None
        -> ""

    | Some activation ->
        match activation with
        | FatePoints _ -> "Fate point"
        | AlwaysOn -> "Passive"
        | value -> sprintf "Once / %s" (string value)

let private parseActivation name =
    match name with
    | "AlwaysOn" ->
        Some AlwaysOn
    | fate when fate.StartsWith "FatePoints" ->
        Some (FatePoints 1)
    | "Scene" ->
        Some Scene
    | "Conflict" ->
        Some Conflict
    | "Day" ->
        Some Day
    | "Session" ->
        Some Session
    | _ ->
        None

let private parseAction name =
    match name with
    | "Attack" -> Some Attack
    | "CreateAdvantage" -> Some CreateAdvantage
    | "Defend" -> Some Defend
    | "Overcome" -> Some Overcome
    | _ -> None

let private findStunt model index =
    model.Stunts
    |> firstBy (fun x -> x.Index = index)

let private createStunt stuntType index =
    {
        Name = ""
        Description = ""
        Ability = ""
        Action = None
        Activation = None
        Type = stuntType
        Index = index
    }

let private freeStunts model =
    match model.Campaign with
    | None ->
        0
    | Some (Campaign.Core campaign) ->
        campaign.FreeStunts
    | Some (Campaign.FAE campaign) ->
        campaign.FreeStunts

let private maxStunts model =
    let defaultMax = initialRefresh model + freeStunts model

    match model.Campaign with
    | None ->
        0
    | Some (Campaign.Core campaign) ->
        defaultArg campaign.MaxStunts defaultMax
    | Some (Campaign.FAE campaign) ->
        defaultArg campaign.MaxStunts defaultMax

let private canAddNewStunt model =
    let maximum = maxStunts model
    let currentTotal = model.Stunts.Length

    currentTotal < maximum

module State =
    let addRefresh (model: Model)  =
        { model with Refresh = initialRefresh model }

    let addStunts model =
        let free = freeStunts model
        if free = 0
        then
            model
        else
            { model with
                Stunts =
                    [1 .. free]
                    |> List.map (fun index -> createStunt Free index) }

    let onBuyStunt model =
        let lastIndex =
            if model.Stunts.Length = 0
            then 0
            else (model.Stunts |> List.last).Index

        let stunt = createStunt Paid (lastIndex + 1)
        { model with
            Stunts = [ stunt ] |> List.append model.Stunts
            Refresh = model.Refresh - 1
        }

    let private replaceStunt stunt model =
        let replaceWith stunt item =
            if (item.Index = stunt.Index)
            then stunt
            else item

        { model with
            Stunts =
                model.Stunts
                |> List.map (replaceWith stunt) }

    let onUpdateStunt model stunt =
        let existing = findStunt model stunt.Index
        match existing with
        | None ->
            model
        | Some _ ->
            model
            |> replaceStunt stunt

    let onRemoveStunt model index =
        let existing = findStunt model index
        match existing with
        | None ->
            model
        | Some stunt ->
            match stunt.Type with
            | Free ->
                model
            | Paid ->
                let stunts =
                    model.Stunts
                    |> List.filter (fun stunt -> stunt.Index <> index)

                { model with
                    Stunts = stunts
                    Refresh = model.Refresh + 1 }

    let stuntCountWithinRange model =
        let total = model.Stunts.Length
        let max = maxStunts model
        let min = freeStunts model

        min <= total && total <= max

    let allStuntsValid model =
        model.Stunts
        |> noneExist (fun stunt ->
            stunt.Name = ""
            || stunt.Description = ""
            || stunt.Ability = ""
            || stunt.Action.IsNone
            || stunt.Activation.IsNone
        )

module View =
    open Feliz.Bulma

    open Global
    open App.Views.Layouts
    open App.Views.Controls

    let private addonButtonWidth = style.width 150
    let private stuntNameWidth = style.width 300
    let private dropdownWidth = style.width 200
    let private gapBetweenStunts = style.marginBottom 30

    let private toOptions (list: (string * string) list) =
        list
        |> List.append [ ("", "") ]
        |> List.map (fun (value, text) ->
            Html.option [
                prop.text text
                prop.value value
            ])

    let private abilityOptions model =
        abilitiesWithRanks model
        |> toOptions

    let private actionOptions =
        [
            Attack
            CreateAdvantage
            Defend
            Overcome
        ]
        |> List.map (fun action ->
            let text = describeAction action
            let value = string action
            (value, text))
        |> toOptions

    let private activationOptions =
        [
            Some AlwaysOn
            Some (FatePoints 1)
            Some Scene
            Some Conflict
            Some Day
            Some Session
        ]
        |> List.map (fun activation ->
            let text = describeActivation activation
            let value = string activation
            (value, text)
        )
        |> toOptions

    let private currentRefresh model =
        Html.div [
            prop.style [ gapBetweenStunts ]
            prop.children [
                addonGroup [
                    addonButton "Refresh" addonButtonWidth
                    Bulma.button [
                        if model.Refresh > 0
                        then button.isPrimary
                        else button.isWarning

                        button.isLight
                        prop.tabIndex -1
                        prop.text model.Refresh
                    ]
                ]
            ]
        ]

    let private updateStuntName dispatch (stunt: Stunt) name =
        { stunt with Name = name }
        |> UpdateStunt
        |> dispatch

    let private stuntName dispatch model stunt =
        addonGroup [
            addonButton "Name" addonButtonWidth
            Bulma.textInput [
                onFocusSelectText
                prop.placeholder (
                    if stunt.Type = Free
                    then "Free Stunt name"
                    else "Stunt name"
                )
                if stunt.Name = "" then
                    input.isDanger
                prop.value stunt.Name
                prop.style [ stuntNameWidth ]
                prop.onTextChange (updateStuntName dispatch stunt)
            ]
        ]

    let private removeStunt (padding: IReactProperty option) dispatch stunt =
        if stunt.Type = Free
        then Html.none
        else row padding [
            imgButton "Remove stunt" Fa.times [
                button.isDanger
                prop.onClick (fun _ -> RemoveStunt stunt.Index |> dispatch)
            ]
        ]

    let private updateStuntActivation dispatch (stunt: Stunt) name =
        let activation = parseActivation name
        { stunt with Activation = activation }
        |> UpdateStunt
        |> dispatch

    let private stuntActivation dispatch model stunt =
        let activation = activationOptions
        let selectedItem = string stunt.Activation

        addonGroup [
            addonButton "Activation" addonButtonWidth
            Bulma.select [
                prop.value selectedItem
                prop.children activation
                prop.style [ dropdownWidth ]
                if stunt.Activation.IsNone then
                    input.isDanger
                onSelectChange (updateStuntActivation dispatch stunt)
            ]
        ]

    let private updateStuntAbility dispatch stunt name =
        { stunt with Ability = name }
        |> UpdateStunt
        |> dispatch

    let private stuntAbility dispatch model stunt =
       let ability = abilityName model
       let abilities = abilityOptions model

       addonGroup [
            addonButton ability addonButtonWidth
            Bulma.select [
                prop.value stunt.Ability
                prop.children abilities
                prop.style [ dropdownWidth ]
                if stunt.Ability = "" then
                    input.isDanger
                onSelectChange (updateStuntAbility dispatch stunt)
            ]
        ]

    let private updateStuntAction dispatch stunt name =
        { stunt with Action = parseAction name }
        |> UpdateStunt
        |> dispatch

    let private stuntAction dispatch model stunt =
        let actions = actionOptions

        addonGroup [
            addonButton "Action" addonButtonWidth
            Bulma.select [
                prop.value (string stunt.Action)
                prop.children actions
                prop.style [ dropdownWidth ]
                if stunt.Action.IsNone then
                    input.isDanger
                onSelectChange (updateStuntAction dispatch stunt)
            ]
        ]

    let private updateStuntDescription dispatch stunt description =
        { stunt with Description = description }
        |> UpdateStunt
        |> dispatch


    let private stuntDescription dispatch model stunt =
        Bulma.textarea [
            onFocusSelectText
            prop.placeholder "Stunt description"
            prop.value stunt.Description
            prop.style [ style.minHeight 100 ]
            if stunt.Description = "" then
                input.isDanger
            prop.onTextChange (updateStuntDescription dispatch stunt)
        ]

    let private editStunt dispatch model stunt =
        let tight = Some (prop.style [ style.paddingBottom (length.rem 0.25) ])
        Bulma.columns [
            columns.isMultiline
            prop.style [ gapBetweenStunts ]
            prop.children [
                removeStunt tight dispatch stunt
                row tight [ stuntName dispatch model stunt ]
                row tight [ stuntAbility dispatch model stunt ]
                row tight [ stuntAction dispatch model stunt ]
                row tight [ stuntActivation dispatch model stunt ]
                row tight [ stuntDescription dispatch model stunt ]
            ]
        ]

    let private addNewStunt dispatch model =
        if canAddNewStunt model
        then imgButton "Buy stunt with Refresh" Fa.plus [
                prop.onClick (fun _ -> BuyStunt |> dispatch)
                button.isInfo
            ]
        else Html.none

    let selectStunts dispatch model =
        let rows =
            model.Stunts
            |> List.map (editStunt dispatch model)

        colLayout [
            labelCol [ Bulma.label "Stunts:" ]
            {
                Size = [ column.is6 ]
                Content = [
                    currentRefresh model
                    yield! rows
                    addNewStunt dispatch model
                ]
            }
        ]