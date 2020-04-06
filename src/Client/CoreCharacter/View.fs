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
        labelCol [ Bulma.label "Skill selection:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content =
                buttonGroup [
                    {
                        Text = "Use default Skills"
                        Color = button.isPrimary
                        Active = model.Skills = AbilityType.Default
                        OnClick = toggleCustomSkills
                    }
                    {
                        Text = "Use custom skills"
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

        let skills =
            match model.Campaign with
            | None -> []
            | Some campaign -> campaign.SkillList

        let textBoxes = abilityTextBoxes skills textChanged

        colLayout [
            labelCol [ Bulma.label "Skills:" ]
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
        toggleCustomSkills dispatch model
        customiseSkills dispatch model
        yield! debug model
    ]
    |> box (getDocTitle model)