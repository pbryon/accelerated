module Character.State

open Elmish
open Elmish.Helper
open Fable.Core

open Global
open Domain.Campaign
open Domain.System

open Character.Types
open Character.Aspects.State
open Character.Abilities.State
open Character.Stunts.State

let init (user: UserData) (campaign: Campaign option): Model * Cmd<Msg> =
    {
        Campaign = campaign
        CampaignId = user.CampaignId
        Player = user.UserName
        CharacterName = CharacterName ""
        Aspects = []
        Abilities = []
        Stunts = []
        Refresh = 0
        Finished = None
    }
    |> addStartingAspects
    |> addAbilities
    |> addRefresh
    |> addStunts
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCharacter campaign ->
        let userData = {
            UserName = currentModel.Player
            CampaignId = currentModel.CampaignId
        }
        init userData campaign

    | SetPlayerName value ->
        { currentModel with Player = PlayerName value }
        |> withoutCommands

    | SetCharacterName value ->
        { currentModel with CharacterName = CharacterName value }
        |> withoutCommands

    | AddAspect aspect ->
        onAddAspect currentModel aspect
        |> withoutCommands

    | UpdateAspect aspect ->
        onUpdateAspect currentModel aspect
        |> withoutCommands

    | UpdateAbility ability ->
        onUpdateAbility currentModel ability
        |> withoutCommands

    | BackToCampaignClicked _ ->
        currentModel
        |> withoutCommands

    | FinishClicked ->
        { currentModel with Finished = Some true }
        |> withoutCommands

