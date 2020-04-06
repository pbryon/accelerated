module Characters.View

open Feliz
open Feliz.Bulma

open Elmish.Common

open Characters.Types
open Domain.Campaign

let chooseCampaignType dispatch model =
    let title =
        if model.CampaignType = CampaignType.NotSelected
        then "Select a campaign type:"
        else "Selected campaign type:"

    colLayout [
        labelCol [ Bulma.label title ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = "Fate Core"
                        Color = button.isPrimary
                        Active = model.CampaignType = CampaignType.Core
                        OnClick = (fun _ -> SelectCoreCampaign |> dispatch)
                    }
                    {
                        Text = "Fate Accelerated"
                        Color = button.isPrimary
                        Active = model.CampaignType = CampaignType.FAE
                        OnClick = (fun _ -> SelectFAECampaign |> dispatch)
                    }
                    {
                        Text = "Reset"
                        Color = button.isDanger
                        Active = true
                        OnClick = (fun _ -> ResetCampaign |> dispatch )
                    }
                ]
        }
    ]

let abilityName model =
    match model.CampaignType with
    | CampaignType.NotSelected -> ""
    | CampaignType.Core -> "Skills"
    | CampaignType.FAE -> "Approaches"

let toggleCustomAbilities dispatch model =
    let abilityName = abilityName model
    let toggleCustomAbilities = (fun _ -> ToggleCustomAbilities |> dispatch)

    colLayout [
        labelCol [ Bulma.label (sprintf "%s selection:" abilityName) ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = (sprintf "Use default %s" abilityName)
                        Color = button.isPrimary
                        Active = model.Abilities = AbilityType.Default
                        OnClick = toggleCustomAbilities
                    }
                    {
                        Text = (sprintf "Customise %s" abilityName)
                        Color = button.isPrimary
                        Active = model.Abilities = AbilityType.Custom
                        OnClick = toggleCustomAbilities
                    }
                ]
        }
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

