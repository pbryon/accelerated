module Character.Types.Aspects

open Utils
open Domain.Campaign
open Domain.System

open Character.Types

let aspectLike fst snd =
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


let findAspectLike model aspect =
    model.Aspects
    |> findLike aspectLike aspect

let hasAspectLike model aspect =
    model.Aspects
    |> containsLike aspectLike aspect

let private getCampaignAspects model =
    match model.Campaign with
    | None -> []
    | Some (Campaign.Core core) ->
        core.Aspects

    | Some (Campaign.FAE fae) ->
        fae.Aspects

let aspectTotal model =
    getCampaignAspects model
    |> List.sumBy (fun x ->
        match x with
        | HighConceptAndTrouble -> 2
        | ExtraAspects number -> number
        | PhaseTrio -> 3
    )

let extraAspectTotal model =
    let extra =
        getCampaignAspects model
        |> findLike Campaign.Types.aspectLike (ExtraAspects 1)

    match extra with
    | Some (ExtraAspects number) -> number
    | _ -> 0

let private usesAspectType aspects aspectType =
    aspects
    |> contains aspectType

let usesPhaseTrio model =
    let aspects = getCampaignAspects model
    usesAspectType aspects PhaseTrio

let nextAspect model =
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