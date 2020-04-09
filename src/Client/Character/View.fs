module Character.View

open Feliz
open Feliz.Bulma


open App.Views.Layouts
open App.Views.Buttons
open App.Icons

open Domain.System
open Character.Types

let setPlayerName dispatch model =
    let playerName (PlayerName value) = value

    colLayout [
        labelCol [ Bulma.label "Player:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "PlayerName"
                    prop.placeholder "Player name"
                    prop.defaultValue (playerName model.Player)
                    prop.onTextChange (fun value -> SetPlayerName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
            ]
        }
    ]

let setCharacterName dispatch model =
    let charName (CharacterName value) = value

    colLayout [
        labelCol [ Bulma.label "Character:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "CharacterName"
                    prop.placeholder "Character name"
                    prop.defaultValue (charName model.CharacterName)
                    prop.onTextChange (fun value -> SetCharacterName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
            ]
        }
    ]

let finishButton dispatch model =
    colLayout [
        labelCol [ Html.none ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                imgButton "Done" Fa.chevronRight [
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
        finishButton dispatch model
        yield! Debug.view model
    ]
    |> box "Character creation"