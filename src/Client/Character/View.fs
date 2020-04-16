module Character.View

open Feliz
open Feliz.Bulma

open Global
open App.Views.Layouts
open App.Views.Controls
open App.Icons

open Domain.System
open Domain.SystemReference
open Domain.Campaign

open Character.Types
open Character.Aspects
open Character.Abilities

open Abilities.View
open Aspects.View
open Stunts.View

let private getHelpTopic model forCore forFae =
    match model.Campaign with
    | None ->
        Html.none
    | Some (Campaign.Core _) ->
        rulesButton "" forCore
    | Some (Campaign.FAE _) ->
        rulesButton "" forFae

let ruleSetUsed model =
    let ruleset =
        match model.Campaign with
        | None -> "None"
        | Some (Campaign.Core _) -> "Fate Core"
        | Some (Campaign.FAE _) -> "Fate Accelerated"

    colLayout [
        labelCol [ Bulma.label "Ruleset" ]
        {
            Props = [ column.is4 ]
            Content = [
                Html.span [
                    prop.text ruleset
                    prop.style [
                        style.marginTop 5
                        style.display.inlineFlex
                    ]
                ]
                getHelpTopic model Topic.FateCore Topic.FateAccelerated
            ]
        }
    ]

let setPlayerName dispatch model =
    let player = Convert.playerName model.Player
    colLayout [
        labelCol [ Bulma.label "Player" ]
        {
            Props = [ column.is4 ]
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
        labelCol [ Bulma.label "Character" ]
        {
            Props = [ column.is4 ]
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
            Props = [ column.is4 ]
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
        ruleSetUsed model
        setPlayerName dispatch model
        setCharacterName dispatch model
        Html.hr []
        chooseAspects dispatch model
        addNextAspect dispatch model
        Html.hr []
        chooseAbilities dispatch model
        Html.hr []
        selectStunts dispatch model
        backAndFinishButtons dispatch model
        yield! Debug.view "Stunts" model.Stunts
        yield! Debug.view "Model" model
    ]
    |> box "Character creation"