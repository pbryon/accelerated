module Characters.View

open Feliz
open Feliz.Bulma

open Elmish.Common

open Characters.Types
open Domain.Campaign

let selectCampaign expected model props =
    if model.CampaignType = expected
    then
        [ button.isActive ]
    else
        [
            button.isLight
        ]
    |> List.append props
    |> Bulma.button

let chooseCampaignType dispatch model =
    let title =
        if model.CampaignType = CampaignType.NotSelected
        then "Select a campaign type:"
        else "Selected campaign type:"

    Bulma.columns [
        columns.isVcentered
        prop.children [
            Bulma.column [
                column.is3
                column.isOffset1
                prop.style [ style.textAlign.left ]
                prop.children [ Bulma.label title ]
            ]
            Bulma.column [
                Bulma.buttons [
                    buttons.hasAddons
                    prop.children [
                        selectCampaign CampaignType.Core model [
                            prop.text "Fate Core"
                            button.isPrimary
                            prop.onClick (fun _ -> SelectCoreCampaign |> dispatch)
                        ]
                        selectCampaign CampaignType.FAE model [
                            prop.text "Fate Accelerated"
                            button.isPrimary
                            prop.onClick (fun _ -> SelectFAECampaign |> dispatch)
                        ]
                        Bulma.button [
                            button.isDanger
                            prop.text "Reset"
                            button.isHovered
                            prop.onClick (fun _ -> ResetCampaign |> dispatch )
                        ]
                    ]
                ]
            ]
        ]
    ]

let abilityName model =
    match model.CampaignType with
    | CampaignType.NotSelected -> ""
    | CampaignType.Core -> "Skills"
    | CampaignType.FAE -> "Approaches"

let toggleCustomAbilities dispatch model =
    let abilityName = abilityName model

    Bulma.columns [
        Bulma.column [
            column.is3
            column.isOffset1
            prop.children [
                Bulma.label [
                    alignLeft
                    prop.text (sprintf "Use default %s:" abilityName)
                ]
            ]
        ]
        Bulma.column [
            column.is4
            alignLeft
            prop.children [
                Bulma.checkboxInput [
                    prop.isChecked (model.Abilities = AbilityType.Default)
                    prop.onClick (fun _ -> ToggleCustomAbilities |> dispatch )
                ]
            ]
        ]
    ]

let getAbilities dispatch model =
    let textBoxFor = (fun item -> Bulma.textInput [
        prop.placeholder item
        prop.defaultValue item
        prop.name item
        prop.className "is-4"
        prop.onTextChange (fun value -> RenameAbility (item, value) |> dispatch)
    ])

    match model.Campaign with
        | Some (Core campaign) -> campaign.SkillList
        | Some (FAE campaign) -> campaign.ApproachList
        | None -> []
    |> List.map textBoxFor

let customiseAbilities dispatch model =
    let abilityName = abilityName model
    let abilities = getAbilities dispatch model

    match model.Abilities with
    | AbilityType.Default ->
        Html.none

    | AbilityType.Custom ->
        Html.div [
            Bulma.columns [
                columns.isMultiline
                prop.children [
                    Bulma.column [
                        column.is3
                        column.isOffset1
                        prop.style [ style.textAlign.left ]
                        prop.children [
                            Bulma.label (sprintf "%s:" abilityName)
                        ]
                    ]
                    Bulma.column [
                        column.is4
                        prop.children abilities
                    ]
                ]
            ]
        ]

let view dispatch model =
    let title =
        match model.Character with
        | None -> "Campaign creation"
        | Some _ -> "Character creation"

    match model.CampaignType with
    | CampaignType.NotSelected ->
        [ chooseCampaignType dispatch model ]

    | _ ->
        [
            (chooseCampaignType dispatch model)
            (toggleCustomAbilities dispatch model)
            (customiseAbilities dispatch model)
            Html.h3 "Campaign"
            Html.div (sprintf "%A" model.Campaign)
        ]

    |> box title

