module Character.View

open Feliz
open Feliz.Bulma

open Global
open App.Views.Layouts
open App.Views.Controls
open App.Icons

open Domain.System

open Character.Types
open Character.Aspects
open Character.Abilities

open Abilities.View
open Aspects.View

let setPlayerName dispatch model =
    let player = Convert.playerName model.Player
    colLayout [
        labelCol [ Bulma.label "Player:" ]
        {
            Size = [ column.is4 ]
            Content = [
                Bulma.textInput [
                    prop.name "PlayerName"
                    prop.placeholder "Player name"
                    prop.value player
                    onFocusSelectText
                    prop.onTextChange (fun value -> SetPlayerName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                    if player = "" then
                        input.isDanger
                ]
            ]
        }
    ]

let setCharacterName dispatch model =
    let character = Convert.characterName model.CharacterName
    colLayout [
        labelCol [ Bulma.label "Character:" ]
        {
            Size = [ column.is4 ]
            Content = [
                Bulma.textInput [
                    prop.name "CharacterName"
                    prop.placeholder "Character name"
                    prop.value character
                    onFocusSelectText
                    prop.onTextChange (fun value -> SetCharacterName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                    if character = "" then
                        input.isDanger
                ]
            ]
        }
    ]

let private backAndFinishButtons dispatch model =
    let userData = {
        UserName = model.Player
        CampaignId = model.CampaignId
    }
    colLayout [
        labelCol [ Html.none ]
        {
            Size = [ column.is4 ]
            Content = [
                imgButton "Back" Fa.chevronLeft [
                    prop.onClick (fun _ -> BackToCampaignClicked userData |> dispatch )
                ]
                imgButtonRight "Done" Fa.chevronRight [
                    prop.onClick (fun _ -> FinishClicked |> dispatch)
                    prop.disabled (not (Validation.isDone model))
                ]
            ]
        }
    ]

let view dispatch model =
    [
        resetButton "Reset" (fun _ -> ResetCharacter model.Campaign |> dispatch)
        setPlayerName dispatch model
        setCharacterName dispatch model
        chooseAspects dispatch model
        addNextAspect dispatch model
        chooseAbilities dispatch model
        backAndFinishButtons dispatch model
        yield! Debug.view "Abilities" model.Abilities
    ]
    |> box "Character creation"