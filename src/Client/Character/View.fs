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
    let player = Convert.playerName model.Player
    colLayout [
        labelCol [ Bulma.label "Player:" ]
        {
            Size = [ column.is4 ]
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "PlayerName"
                    prop.placeholder "Player name"
                    prop.value player
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
            Align = style.textAlign.left
            Content = [
                Bulma.textInput [
                    prop.name "CharacterName"
                    prop.placeholder "Character name"
                    prop.value character
                    prop.onTextChange (fun value -> SetCharacterName value |> dispatch)
                    prop.style [ style.maxWidth (length.perc 60) ]
                    if character = "" then
                        input.isDanger
                ]
            ]
        }
    ]

let aspectButtonWidth = style.width 150
let aspectTextWidth = style.width 300

let private highConcept dispatch model =
    let newHighConcept name = Aspect.HighConcept (AspectName name)
    let existing = findAspectLike model (newHighConcept "")

    match existing with
    | Some (Aspect.HighConcept name) ->
        let aspectName = Convert.aspectName name
        addonGroup [
            addonButton "High Concept" aspectButtonWidth
            Bulma.textInput [
                prop.name "high-concept"
                prop.placeholder "High Concept"
                prop.value aspectName
                prop.onTextChange (newHighConcept >> UpdateAspect >> dispatch)
                prop.style [ aspectTextWidth ]
                if aspectName = "" then
                    input.isDanger
            ]
        ]
    | _ ->
        Html.none

let private trouble dispatch model =
    let newTrouble name = Aspect.Trouble (AspectName name)
    let existing = findAspectLike model (newTrouble "")

    match existing with
    | Some (Aspect.Trouble name) ->
        let aspectName = Convert.aspectName name
        addonGroup [
            addonButton "Trouble" aspectButtonWidth
            Bulma.textInput [
                prop.name "trouble"
                prop.placeholder "Trouble"
                prop.value aspectName
                prop.onTextChange (newTrouble >> UpdateAspect >> dispatch)
                prop.style [ aspectTextWidth ]
                if aspectName = "" then
                    input.isDanger
            ]
        ]
    | _ ->
        Html.none

let private phaseAspect dispatch model number =
    let (phase, phaseName) =
        match number with
        | 1 -> (PhaseOne, "Phase One")
        | 2 -> (PhaseTwo, "Phase Two")
        | 3 -> (PhaseThree, "Phase Three")
        | _ -> failwithf "Unsupported phase number: %i" number

    let newPhase phase text =
        Aspect.PhaseTrio (phase, AspectName "")
    let defaultAspect = newPhase phase ""
    let existing = findAspectLike model defaultAspect

    match existing with
    | Some (Aspect.PhaseTrio (phase, name)) ->
        let aspectName = Convert.aspectName name
        addonGroup [
            addonButton phaseName aspectButtonWidth
            Bulma.textInput [
                prop.name (sprintf "phase-%i" number)
                prop.placeholder phaseName
                prop.value aspectName
                prop.onTextChange (newPhase phase >> UpdateAspect >> dispatch)
                prop.style [ aspectTextWidth ]
                if aspectName = "" then
                    input.isDanger
            ]
        ]

    | _ -> Html.none

let private phaseTrioAspects dispatch model =
    if usesPhaseTrio model
    then
        [ 1 .. 3]
        |> List.map (phaseAspect dispatch model)
    else
        []

let otherAspect dispatch model number =
    let newAspect number text =
        Aspect.Other (number, AspectName text)
    let defaultAspect = newAspect number ""
    let existing = findAspectLike model defaultAspect

    let start = aspectTotal model - extraAspectTotal model

    match existing with
    | Some (Aspect.Other (_, aspectName)) ->
        let text = sprintf "Aspect %i" (start + number)
        addonGroup [
            addonButton text aspectButtonWidth
            Bulma.textInput [
                prop.name (sprintf "other-aspect-%i" number)
                prop.placeholder "New Aspect"
                prop.value (Convert.aspectName aspectName)
                prop.onTextChange (newAspect number >> UpdateAspect >> dispatch)
                prop.style [ aspectTextWidth ]
            ]
        ]

    | _ -> Html.none

let private otherAspects dispatch model =
    match extraAspectTotal model with
    | 0 ->
        []
    | total ->
        [ 1 .. total ]
        |> List.map (otherAspect dispatch model)

let private chooseAspects dispatch model =
    colLayout [
        labelCol [ Bulma.label "Aspects:" ]
        {
            Size = [ column.is8 ]
            Align = style.textAlign.left
            Content = [
                highConcept dispatch model
                trouble dispatch model
                yield! phaseTrioAspects dispatch model
                yield! otherAspects dispatch model
            ]
        }
    ]

let private addNextAspect dispatch model =
    match nextAspect model with
    | None -> Html.none
    | Some aspect ->
        colLayout [
            emptyLabelCol
            {
                Size = [ column.is4 ]
                Align = style.textAlign.left
                Content = [
                    imgButton "" Fa.plus [
                        prop.onClick (fun _ -> AddAspect aspect |> dispatch)
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
        resetButton "Reset" (fun _ -> ResetCharacter model.Campaign |> dispatch)
        setPlayerName dispatch model
        setCharacterName dispatch model
        chooseAspects dispatch model
        addNextAspect dispatch model
        backAndFinishButtons dispatch model
        yield! Debug.view model
    ]
    |> box "Character creation"