module CoreCharacter.View

open Feliz
open Feliz.Bulma

open Elmish.Common

open CharacterHelper
open CoreCharacter.Types
open Domain.Campaign

let toggleCustomSkills dispatch model =
    let toggleCustomSkills = (fun _ -> ToggleCustomSkills |> dispatch)

    colLayout [
        labelCol [ Bulma.label "Skills:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = "Use default"
                        Color = button.isPrimary
                        Active = model.Skills = AbilityType.Default
                        OnClick = toggleCustomSkills
                    }
                    {
                        Text = "Customise"
                        Color = button.isPrimary
                        Active = model.Skills = AbilityType.Custom
                        OnClick = toggleCustomSkills
                    }
                ]
        }
    ]

let customiseSkills dispatch model =
    match model.Skills with
    | AbilityType.Default ->
        Html.none

    | AbilityType.Custom ->
        let textChanged oldValue newValue =
            RenameSkill (oldValue, newValue) |> dispatch
        let inputSkill _ = InputNewSkill |> dispatch
        let changeNewSkill value = UpdateNewSkill value |> dispatch
        let addSkill _ = AddNewSkill |> dispatch

        let skills =
            match model.Campaign with
            | None -> []
            | Some campaign -> campaign.SkillList

        let showInputButton = newItemButton "Skill" model.NewSkill inputSkill
        let inputFields = newItemInputs "Skill" model.NewSkill changeNewSkill addSkill
        let textBoxes = abilityTextBoxes skills textChanged

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

let view dispatch model =
    [
        resetCampaign (fun _ -> ResetCampaign |> dispatch)
        toggleCustomSkills dispatch model
        customiseSkills dispatch model
        yield! debug model
    ]
    |> box (getDocTitle model)