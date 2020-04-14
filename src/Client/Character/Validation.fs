module Character.Validation

open Global
open Domain.System

open Character.Types
open Character.Aspects
open Character.Abilities
open Aspects.State
open Abilities.State

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x -> "" <> Convert.playerName x.Player)
    |> validate (fun x -> "" <> Convert.characterName x.CharacterName)
    |> validate allAspectsNamed
    |> validate allAbilitiesAssigned
    |> validate allAbilitiesValid
    |> Option.isSome