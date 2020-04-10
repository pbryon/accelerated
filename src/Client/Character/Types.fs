module Character.Types

open Global
open Domain.Campaign
open Domain.System

type Model = {
    Campaign: Campaign option
    CampaignId: CampaignId
    Player: PlayerName
    CharacterName: CharacterName
    Aspects: Aspect list
    Finished: bool option
}

type Msg =
    | ResetCharacter of Campaign
    | SetPlayerName of string
    | SetCharacterName of string
    | AddAspect of Aspect
    | UpdateAspect of Aspect
    | FinishClicked

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

    | Aspect.PhaseTrio (phase, _, _) ->
        match snd with
        | Aspect.PhaseTrio (otherPhase, _, _)
            when phase = otherPhase -> true
        | _ -> false

    | Aspect.Other (number, _) ->
        match snd with
        | Aspect.Other (otherNumber, _)
            when number = otherNumber -> true
        | _ -> false


let findAspectLike model aspect =
    model.Aspects
    |> List.tryFind(fun x -> aspectLike aspect x)

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x ->
        match x.Player with
        | PlayerName "" -> false
        | _ -> true)
    |> validate (fun x ->
        match x.CharacterName with
        | CharacterName "" -> false
        | _ -> true)
    |> validate (fun x -> x.Aspects.Length > 0)
    |> validate (fun x -> false) // TODO: finish this logic
    |> Option.isSome