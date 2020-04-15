module Character.Validation

open Utils
open Domain.System

open Character.Types
open Aspects.State
open Abilities.State
open Stunts.State

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x -> "" <> Convert.playerName x.Player)
    |> validate (fun x -> "" <> Convert.characterName x.CharacterName)
    |> validate allAspectsNamed
    |> validate allAbilitiesAssigned
    |> validate allAbilitiesValid
    |> validate allStuntsValid
    |> Option.isSome