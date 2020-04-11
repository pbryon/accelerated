module Character.View

open Feliz
open Feliz.Bulma

open Global
open App.Views.Layouts
open App.Views.Buttons
open App.Icons

open Domain.System
open Character.Types

let setPlayerName dispatch model =
    colLayout [
        labelCol [ Bulma.label "Player:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "PlayerName"
                    prop.placeholder "Player name"
                    prop.defaultValue (Convert.playerName model.Player)
                    prop.onTextChange (fun value -> SetPlayerName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
            ]
        }
    ]

let setCharacterName dispatch model =
    colLayout [
        labelCol [ Bulma.label "Character:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "CharacterName"
                    prop.placeholder "Character name"
                    prop.defaultValue (Convert.characterName model.CharacterName)
                    prop.onTextChange (fun value -> SetCharacterName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
            ]
        }
    ]

let private highConcept dispatch model =
    let newHighConcept name = Aspect.HighConcept (AspectName name)
    let existing = findAspectLike model (newHighConcept "")

    match existing with
    | Some (Aspect.HighConcept name) ->
        addonGroup [
            Bulma.button [
                button.isPrimary
                prop.text "High concept"
                prop.style [ style.minWidth 150 ]
            ]
            Bulma.textInput [
                prop.name "high-concept"
                prop.placeholder "High Concept"
                prop.defaultValue (Convert.aspectName name)
                prop.onTextChange (newHighConcept >> UpdateAspect >> dispatch)
                prop.style [ style.minWidth 300 ]
            ]
        ]
    | _ ->
        Html.none

let private trouble dispatch model =
    let newTrouble name = Aspect.Trouble (AspectName name)
    let existing = findAspectLike model (newTrouble "")

    match existing with
    | Some (Aspect.Trouble name) ->
        addonGroup [
            Bulma.button [
                button.isPrimary
                prop.text "Trouble"
                prop.style [ style.minWidth 150 ]
            ]
            Bulma.textInput [
                prop.name "trouble"
                prop.placeholder "Trouble"
                prop.defaultValue (Convert.aspectName name)
                prop.onTextChange (newTrouble >> UpdateAspect >> dispatch)
                prop.style [ style.minWidth 300 ]
            ]
        ]
    | _ ->
        Html.none

let private chooseAspects dispatch model =
    colLayout [
        labelCol [ Bulma.label "Aspects:" ]
        {
            Size = [ column.is8 ]
            Align = style.textAlign.left
            Content = [
                highConcept dispatch model
                trouble dispatch model
            ]
        }
    ]

let backAndFinishButtons dispatch model =
    let userData = {
        UserName = model.Player
        CampaignId = model.CampaignId
    }
    colLayout [
        labelCol [ Html.none ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                imgButton "Back" Fa.chevronLeft [
                    prop.onClick (fun _ -> BackToCampaignClicked userData |> dispatch )
                ]
                imgButtonRight "Done" Fa.chevronRight [
                    prop.onClick (fun _ -> FinishClicked |> dispatch)
                    prop.disabled (not (isDone model))
                ]
            ]
        }
    ]

let view dispatch model =
    [
        setPlayerName dispatch model
        setCharacterName dispatch model
        chooseAspects dispatch model
        backAndFinishButtons dispatch model
        yield! Debug.view model
    ]
    |> box "Character creation"