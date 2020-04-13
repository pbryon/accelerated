module Character.Validation

open Global
open Domain.System

open Character.Types
open Character.Aspects
open Character.Abilities

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x -> "" <> Convert.playerName x.Player)
    |> validate (fun x -> "" <> Convert.characterName x.CharacterName)
    |> validate State.allAspectsNamed
    |> validate State.allAbilitiesAssigned
    |> Option.isSome