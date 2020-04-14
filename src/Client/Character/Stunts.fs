module Character.Stunts

open Feliz

open Utils

open Domain
open Domain.System
open Domain.Campaign

open Character.Types

let private refreshValue (Refresh refresh) = refresh

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
        sprintf "%s +%i" ability.Name (abilityRank ability)
    )

let private describeAction action =
    match action with
    | CreateAdvantage -> "Create advantage"
    | _ -> string action

let private describeActivation activation =
    match activation with
    | FatePoints _ -> "Fate point"
    | AlwaysOn -> "Passive"
    | value -> sprintf "Once / %s" (string value)

let private findStunt model name =
    model.Stunts
    |> firstBy (fun x -> x.Name = name)

let private createStunt stuntType =
    {
        Name = ""
        Description = ""
        Ability = None
        Action = None
        Activation = None
        Type = stuntType
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
    match model.Campaign with
    | None ->
        0
    | Some (Campaign.Core campaign) ->
        defaultArg campaign.MaxStunts (refreshValue campaign.Refresh)
    | Some (Campaign.FAE campaign) ->
        defaultArg campaign.MaxStunts (refreshValue campaign.Refresh)

module State =
    let addRefresh model  =
        let refresh =
            match model.Campaign with
            | None ->
                0
            | Some (Campaign.Core campaign) ->
                refreshValue campaign.Refresh
            | Some (Campaign.FAE campaign) ->
                refreshValue campaign.Refresh

        { model with Refresh = refresh }

    let addStunts model =
        let free = freeStunts model
        if free = 0
        then
            model
        else
            { model with
                Stunts =
                    [1 .. free]
                    |> List.map (fun _ -> createStunt Free) }

    let private replaceStunt stunt model =
        let replaceWith stunt item =
            if (item.Name = stunt.Name)
            then stunt
            else item

        { model with
            Stunts =
                model.Stunts
                |> List.map (replaceWith stunt)
        }

    let onUpdateStunt model stunt =
        match findStunt model stunt.Name with
        | None -> model

        | Some _ ->
            model
            |> replaceStunt stunt

module View =
    open Feliz.Bulma

    open Global
    open App.Views.Layouts
    open App.Views.Controls

    let private addonButtonWidth = style.width 150
    let private stuntNameWidth = style.width 300
    let private dropdownWidth = style.width 200
    let private gapBetweenStunts = style.marginBottom 20

    let private toOptions (list: string list) =
        list
        |> List.append [ "" ]
        |> List.map (fun item ->
            Html.option [
                prop.text item
                prop.value item
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
        |> List.map describeAction
        |> toOptions

    let private activationOptions =
        [
            AlwaysOn
            FatePoints 1
            Scene
            Conflict
            Day
            Session
        ]
        |> List.map describeActivation
        |> toOptions

    let currentRefresh model =
        Html.div [
            prop.style [ style.marginBottom 30 ]
            prop.children [
                addonGroup [
                    addonButton "Refresh" addonButtonWidth
                    Bulma.button [
                        if model.Refresh > 0
                        then button.isPrimary
                        else button.isDanger

                        button.isLight
                        prop.tabIndex -1
                        prop.text model.Refresh
                    ]
                ]
            ]
        ]

    let stuntName dispatch model stunt =
        addonGroup [
            addonButton "Name" addonButtonWidth
            Bulma.textInput [
                onFocusSelectText
                prop.placeholder (
                    if stunt.Type = Free
                    then "Free Stunt name"
                    else "Stunt name"
                )
                prop.defaultValue stunt.Name
                prop.style [ stuntNameWidth ]
            ]
        ]

    let stuntActivation dispatch model stunt =
        let activation = activationOptions

        addonGroup [
            addonButton "Activation" addonButtonWidth
            Bulma.select [
                prop.defaultValue ""
                prop.children activation
                prop.style [ dropdownWidth ]
            ]
        ]

    let private stuntAbility dispatch model stunt =
       let ability = abilityName model
       let abilities = abilityOptions model

       addonGroup [
            addonButton ability addonButtonWidth
            Bulma.select [
                prop.defaultValue ""
                prop.children abilities
                prop.style [ dropdownWidth ]
            ]
        ]

    let private stuntAction dispatch model stunt =
        let actions = actionOptions

        addonGroup [
            addonButton "Action" addonButtonWidth
            Bulma.select [
                prop.defaultValue ""
                prop.children actions
                prop.style [ dropdownWidth ]
            ]
        ]

    let stuntDescription dispatch model stunt =
        Bulma.textarea [
            onFocusSelectText
            prop.placeholder "Stunt description"
            prop.defaultValue ""
            prop.style [ style.minHeight 100; gapBetweenStunts ]
        ]

    let private editStunt dispatch model stunt =
        let tight = Some (prop.style [ style.paddingBottom (length.rem 0.25) ])
        Bulma.columns [
            columns.isMultiline
            prop.children [
                row tight [ stuntName dispatch model stunt ]
                row tight [ stuntAbility dispatch model stunt ]
                row tight [ stuntAction dispatch model stunt ]
                row tight [ stuntActivation dispatch model stunt ]
                row tight [ stuntDescription dispatch model stunt ]
            ]
        ]

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
                ]
            }
        ]