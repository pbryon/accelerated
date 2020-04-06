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
        labelCol [ Bulma.label "Approach selection:" ]
        {
            Size = [ column.is6 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = "Use default Approaches"
                        Color = button.isPrimary
                        Active = model.Approaches = AbilityType.Default
                        OnClick = toggleCustomApproaches
                    }
                    {
                        Text = "Use custom Approaches"
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

        let Approaches =
            match model.Campaign with
            | None -> []
            | Some campaign -> campaign.ApproachList

        let textBoxes = abilityTextBoxes Approaches textChanged

        colLayout [
            labelCol [ Bulma.label "Approaches:" ]
            {
                Size = [ column.is4 ]
                Align = style.textAlign.left
                Content = textBoxes
            }
        ]

let getDocTitle model =
    match model.Character with
    | None -> "Campaign creation"
    | Some _ -> "Character creation"

let debug model =
    [
        Html.h3 "Campaign"
        Html.div (sprintf "%A" model.Campaign)
    ]

let view dispatch model =
    [
        resetCampaign (fun _ -> ResetCampaign |> dispatch)
        toggleCustomApproaches dispatch model
        customiseApproaches dispatch model
        yield! debug model
    ]
    |> box (getDocTitle model)