module Campaign.View

open Feliz
open Feliz.Bulma
open Browser.Types

open Global
open App.Icons
open App.Views.Controls
open App.Views.Layouts

open Domain.Campaign
open Domain.System

open Campaign.Types

let private selectCampaignType dispatch model =
    colLayout [
        labelCol [ Bulma.label "Campaign type:" ]
        {
            Size = [ column.is4 ]
            Content =
                buttonGroup [
                    {
                        Text = "Fate Core"
                        Color = button.isPrimary
                        Active = model.CampaignType = Some CampaignType.Core
                        OnClick = (fun _ -> SelectCampaignType CampaignType.Core |> dispatch )
                    }
                    {
                        Text = "Fate Accelerated"
                        Color = button.isPrimary
                        Active = model.CampaignType = Some CampaignType.FAE
                        OnClick = (fun _ -> SelectCampaignType CampaignType.FAE |> dispatch)
                    }
                ]
        }
    ]

let private abilityTextBox (onTextChange: string -> string -> unit ) (item: string) =
    Bulma.column [
        column.isOneThird
        prop.children [
            Bulma.textInput [
                prop.placeholder item
                prop.defaultValue item
                prop.name item
                prop.onTextChange (fun value -> onTextChange item value)
                prop.style [
                    style.maxWidth (length.perc 90)
                    style.marginTop 5 ]
            ]
        ]
    ]

let private abilityTextBoxes (abilities: string list) (onTextChange: string -> string -> unit) =
    abilities
    |> List.map (abilityTextBox onTextChange)


let stringifyStunt current =
    match current with
    | None -> "Individual choice"
    | Some value -> sprintf "%i" value

let private toggleCustomAbilities dispatch model =
    let toggleCustomSkills = (fun _ -> ToggleCustomAbilities |> dispatch)
    let text = abilityNamePlural model

    colLayout [
        labelCol [ Bulma.label text ]
        {
            Size = [ column.is4 ]
            Content =
                buttonGroup [
                    {
                        Text = "Use default"
                        Color = button.isPrimary
                        Active = model.AbilityType = AbilityType.Default
                        OnClick = toggleCustomSkills
                    }
                    {
                        Text = "Customise"
                        Color = button.isPrimary
                        Active = model.AbilityType = AbilityType.Custom
                        OnClick = toggleCustomSkills
                    }
                ]
        }
    ]

let private customiseAbilities dispatch model =
    match model.AbilityType with
    | AbilityType.Default ->
        Html.none

    | AbilityType.Custom ->
        let onTextChanged oldValue newValue = RenameAbility (oldValue, newValue) |> dispatch
        let onInputSkill _ = InputNewAbility |> dispatch
        let onChangeNewSkill value = UpdateNewAbility value |> dispatch
        let onAddSkill _ = AddNewAbility |> dispatch

        let showInputButton = newItemButton model.NewAbility onInputSkill

        let title = abilityName model
        let inputFields = newItemInputs title model.NewAbility onChangeNewSkill onAddSkill
        let textBoxes = abilityTextBoxes model.Abilities onTextChanged

        let elements =
            [ showInputButton; inputFields ]
            |> List.append textBoxes

        colLayout [
            labelCol []
            {
                Size = [ column.is8 ]
                Content = [ fluidColLayout elements ]
            }
        ]

let private selectAspectCount dispatch model =
    let numberButtons =
        [ 1 .. 5 ]
        |> List.map (fun x ->
            {
                Text = sprintf "+ %i" x
                Active = hasAspect model (ExtraAspects x)
                Color = color.isPrimary
                OnClick = (fun _ -> ToggleAspect (ExtraAspects x) |> dispatch )
            })

    colLayout [
        labelCol [ Bulma.label "Aspects:"]
        {
            Size = [ column.is8 ]
            Content = buttonGroup [
                {
                    Text = "High Concept"
                    Active = true
                    Color = color.isPrimary
                    OnClick = (fun _ -> ())
                }
                {
                    Text = "Trouble"
                    Active = true
                    Color = color.isPrimary
                    OnClick = (fun _ -> ())
                }
                {
                    Text = "Phase Trio"
                    Active = hasAspect model PhaseTrio
                    Color = color.isPrimary
                    OnClick = (fun _ -> ToggleAspect PhaseTrio |> dispatch)
                }
                yield! numberButtons
            ]
        }
    ]

let private adjustRefresh dispatch model =
    let refreshIs value = model.Refresh = (Refresh value)

    let buttons =
        [0 .. 10]
        |> List.map (fun value -> {
            Text = sprintf "%i" value
            Active = refreshIs value
            Color = color.isPrimary
            OnClick = fun _ -> SetRefresh value |> dispatch
        })

    colLayout [
        labelCol [ Bulma.label "Refresh:" ]
        {
            Size = [ column.is6]
            Content = buttonGroup buttons
        }
    ]

let private selectFreeStunts dispatch model =
    let buttons =
        [0 .. 5]
        |> List.map Some
        |> List.map (fun value -> {
            Text = sprintf "%i" (defaultArg value 0)
            Active = model.FreeStunts = value
            Color = color.isPrimary
            OnClick = fun _ -> SetFreeStunts value |> dispatch
        })

    colLayout [
        labelCol [ Bulma.label "Free stunts:"]
        {
            Size = [ column.is6 ]
            Content = buttonGroup buttons
        }
    ]

let private selectMaxStunts dispatch model =
    match model.FreeStunts with
        | None ->
            Html.none
        | Some forFree ->
            let buttons =
                match (model.Refresh, forFree) with
                | (Refresh 0, 0) ->
                    [ Some 0 ]

                | (Refresh refresh, 0) ->
                    [ 0 .. refresh ]
                    |> List.map Some
                    |> List.append [ None ]

                | (Refresh refresh, freeStunts) ->
                    let total = refresh + freeStunts
                    let actual =
                        if total > 10
                        then 10
                        else total

                    [ freeStunts .. actual ]
                    |> List.map Some
                    |> List.append [ None ]

                |> List.map (fun value -> {
                    Text = stringifyStunt value
                    Active = model.MaxStunts = value
                    Color = color.isPrimary
                    OnClick = fun _ -> SetMaxStunts value |> dispatch
                })

            colLayout [
                labelCol [ Bulma.label "Maximum stunts:"]
                {
                    Size = [ column.is8 ]
                    Content = buttonGroup buttons
                }
            ]

let finishButton dispatch model =
    colLayout [
        labelCol [ Html.none ]
        {
            Size = [ column.is4 ]
            Content = [
                imgButtonRight "Create character" Fa.chevronRight [
                    prop.onClick (fun _ -> FinishClicked |> dispatch)
                    prop.disabled (not (isDone model))
                ]
            ]
        }
    ]

let view dispatch model =
   [
        resetButton "Reset campaign" (fun _ -> ResetCampaign |> dispatch)
        selectCampaignType dispatch model
        if model.CampaignType.IsSome then
            yield! [
            toggleCustomAbilities dispatch model
            customiseAbilities dispatch model
            selectAspectCount dispatch model
            adjustRefresh dispatch model
            selectFreeStunts dispatch model
            selectMaxStunts dispatch model
            finishButton dispatch model
        ]
        yield! Debug.view "Model" model
   ]
   |> box ("Campaign creation")