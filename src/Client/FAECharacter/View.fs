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
        let newApproach _ = AddNewApproach |> dispatch

        let approaches =
            match model.Campaign with
            | None -> []
            | Some campaign -> campaign.ApproachList

        let textBoxes = abilityTextBoxes approaches textChanged newApproach

        colLayout [
            labelCol []
            {
                Size = [ column.is8 ]
                Align = style.textAlign.left
                Content = textBoxes
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

let view dispatch model =
    [
        resetCampaign (fun _ -> ResetCampaign |> dispatch)
        toggleCustomApproaches dispatch model
        customiseApproaches dispatch model
        yield! debug model
    ]
    |> box (getDocTitle model)