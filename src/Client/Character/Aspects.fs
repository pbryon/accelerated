module Character.Aspects

open Feliz

open Global
open Utils
open Domain.Campaign
open Domain.System

open Character.Types

let private aspectLike fst snd =
    match fst with
    | _ when fst = snd -> true

    | Aspect.HighConcept _ ->
        match snd with
        | Aspect.HighConcept _ -> true
        | _ -> false

    | Aspect.Trouble _ ->
        match snd with
        | Aspect.Trouble _ -> true
        | _ -> false

    | Aspect.PhaseTrio (phase, _) ->
        match snd with
        | Aspect.PhaseTrio (otherPhase,  _)
            when phase = otherPhase -> true
        | _ -> false

    | Aspect.Other (number, _) ->
        match snd with
        | Aspect.Other (otherNumber, _)
            when number = otherNumber -> true
        | _ -> false


let private findAspectLike model aspect =
    model.Aspects
    |> firstLike aspectLike aspect

let private hasAspectLike model aspect =
    model.Aspects
    |> containsLike aspectLike aspect

let private getCampaignAspects model =
    match model.Campaign with
    | None -> []
    | Some (Campaign.Core core) ->
        core.Aspects

    | Some (Campaign.FAE fae) ->
        fae.Aspects

let private aspectTotal model =
    getCampaignAspects model
    |> List.sumBy (fun x ->
        match x with
        | HighConceptAndTrouble -> 2
        | ExtraAspects number -> number
        | PhaseTrio -> 3
    )

let private extraAspectTotal model =
    let extra =
        getCampaignAspects model
        |> firstLike Campaign.Types.aspectLike (ExtraAspects 1)

    match extra with
    | Some (ExtraAspects number) -> number
    | _ -> 0

let private usesAspectType aspects aspectType =
    aspects
    |> contains aspectType

let private usesPhaseTrio model =
    let aspects = getCampaignAspects model
    usesAspectType aspects PhaseTrio

let private nextAspect model =
    let aspects = getCampaignAspects model
    let usesExtraAspects = usesAspectType aspects (ExtraAspects 1)
    let newName = AspectName ""

    match (model.Aspects |> List.last) with
    | Aspect.HighConcept _
    | Aspect.Trouble _ ->
        None

    | Aspect.PhaseTrio (phase, _) ->
        match (phase, usesExtraAspects) with
        | PhaseThree, true ->
            Some (Aspect.Other (1, newName))
        | _, _ ->
            None

    | Aspect.Other (number, _) ->
        if (number = extraAspectTotal model)
        then None
        else Some (Aspect.Other (number + 1, newName))

module State =
    let private addPhaseTrioAspects model list =
        if usesPhaseTrio model
        then
            [
                Aspect.PhaseTrio (PhaseOne, AspectName "")
                Aspect.PhaseTrio (PhaseTwo, AspectName "")
                Aspect.PhaseTrio (PhaseThree, AspectName "")
            ]
            |> List.append list
        else
            list

    let private addExtraAspects model list =
        match extraAspectTotal model with
        | 0 -> list
        | number ->
            [ 1 .. number ]
            |> List.map (fun x -> Aspect.Other(x, AspectName ""))
            |> List.append list

    let addStartingAspects (model: Model) =
        { model with
            Aspects =
                [
                    Aspect.HighConcept (AspectName "")
                    Aspect.Trouble (AspectName "")
                ]
                |> addPhaseTrioAspects model
                |> addExtraAspects model }

    let onAddAspect currentModel aspect =
        let existing = findAspectLike currentModel aspect
        let aspects =
            if existing.IsNone then
                [aspect]
                |> List.append currentModel.Aspects
            else
                [aspect]
                |> List.append currentModel.Aspects
                |> List.filter (fun x -> x <> existing.Value)
        { currentModel with Aspects = aspects }

    let onUpdateAspect currentModel aspect =
        let existing = findAspectLike currentModel aspect
        if existing.IsNone then
            currentModel
        else
            let aspects =
                currentModel.Aspects
                |> List.map (fun x ->
                    if x = existing.Value
                    then aspect
                    else x)
            { currentModel with Aspects = aspects }

    let allAspectsNamed model =
        model.Aspects
        |> noneExist (fun aspect ->
            match aspect with
            | Aspect.HighConcept name
            | Aspect.Trouble name
            | Aspect.PhaseTrio (_, name) ->
                Convert.aspectName name = ""
            | Aspect.Other _ ->
                false
        )

module View =
    open Feliz.Bulma

    open App.Icons
    open App.Views.Layouts
    open App.Views.Controls

    let private aspectButtonWidth = style.width 150
    let private aspectTextWidth = style.width 300

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
                    onFocusSelectText
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
                    onFocusSelectText
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
            Aspect.PhaseTrio (phase, AspectName text)

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
                    onFocusSelectText
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

    let private otherAspect dispatch model number =
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

    let chooseAspects dispatch model =
        colLayout [
            labelCol [ Bulma.label "Aspects:" ]
            {
                Size = [ column.is8 ]
                Content = [
                    highConcept dispatch model
                    trouble dispatch model
                    yield! phaseTrioAspects dispatch model
                    yield! otherAspects dispatch model
                ]
            }
        ]

    let addNextAspect dispatch model =
        match nextAspect model with
        | None -> Html.none
        | Some aspect ->
            colLayout [
                emptyLabelCol
                {
                    Size = [ column.is4 ]
                    Content = [
                        imgButton "" Fa.plus [
                            prop.onClick (fun _ -> AddAspect aspect |> dispatch)
                        ]
                    ]
                }
            ]