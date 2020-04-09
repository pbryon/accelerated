module Character.State

open Elmish
open Elmish.Helper

open Global
open Domain.Campaign
open Character.Types
open Domain.System
open Fable.Core

let init (user: UserData) (campaign: Campaign option): Model * Cmd<Msg> =
    {
        Campaign = campaign
        CampaignId = user.CampaignId
        Player = user.UserName
        CharacterName = CharacterName ""
        Finished = None
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCharacter campaign ->
        { currentModel with Campaign = Some campaign }
        |> withoutCommands

    | SetPlayerName value ->
        JS.console.log (sprintf "Got player name: %s" value)
        { currentModel with Player = PlayerName value }
        |> withoutCommands

    | SetCharacterName value ->
        { currentModel with CharacterName = CharacterName value }
        |> withoutCommands

    | FinishClicked ->
        { currentModel with Finished = Some true }
        |> withoutCommands

