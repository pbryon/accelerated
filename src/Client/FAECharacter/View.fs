module FAECharacter.View

open Feliz
open Feliz.Bulma

open Elmish.Common

open CharacterHelper
open FAECharacter.Types
open Domain.Campaign

let toggleCustomApproaches dispatch model =
    let toggleCustomApproaches = (fun _ -> ToggleCustomApproaches |> dispatch)

    colLayout [
        labelCol [ Bulma.label "Approaches:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = "Use default"
                        Color = button.isPrimary
                        Active = model.Approaches = AbilityType.Default
                        OnClick = toggleCustomApproaches
                    }
                    {
                        Text = "Customise"
                        Color = button.isPrimary
                        Active = model.Approaches = AbilityType.Custom
                        OnClick = toggleCustomApproaches
                    }
                ]
        }
    ]

let customiseApproaches dispatch model =
    match model.Approaches with
    | AbilityType.Default ->
        Html.none

    | AbilityType.Custom ->
        let textChanged oldValue newValue =
            RenameApproach (oldValue, newValue) |> dispatch
        let inputApproach _ = InputNewApproach |> dispatch
        let changeNewApproach value = UpdateNewApproach value |> dispatch
        let addApproach _ = AddNewApproach |> dispatch

        let approaches =
            match model.Campaign with
            | None -> []
            | Some campaign -> campaign.ApproachList

        let showInputButton = newItemButton "Approach" model.NewApproach inputApproach
        let inputFields = newItemInputs "Approach" model.NewApproach changeNewApproach addApproach
        let textBoxes = abilityTextBoxes approaches textChanged

        let elements =
            [ showInputButton; inputFields ]
            |> List.append textBoxes

        colLayout [
            labelCol []
            {
                Size = [ column.is8 ]
                Align = style.textAlign.left
                Content = [ fluidColLayout elements ]
            }
        ]

let getDocTitle model =
    match model.Character with
    | None -> "Campaign creation"
    | Some _ -> "Character creation"

let debug model =
    match (model.Campaign, model.Character) with
    | (_, None) ->
        [
            Html.h3 "Campaign"
            Html.div (sprintf "%A" model.Campaign)
        ]
    | (_, Some character) ->
        [
            Html.h3 "Character"
            Html.div (sprintf "%A" character)
        ]

let resetView dispatch model =
    match model.Character with
    | None ->
        resetButton "Reset campaign" (fun _ -> ResetCampaign |> dispatch)
    | Some _ ->
        resetButton "Reset character" (fun _ -> ResetCharacter |> dispatch )

let view dispatch model =
    [
        resetView dispatch model
        toggleCustomApproaches dispatch model
        customiseApproaches dispatch model
        yield! debug model
    ]
    |> box (getDocTitle model)