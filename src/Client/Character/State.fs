module Character.State

open Elmish
open Elmish.Helper
open Fable.Core

open Global
open Domain.Campaign
open Domain.System

open Character.Types
open Character.Types.Aspects
open Character.Types.Abilities

let init (user: UserData) (campaign: Campaign option): Model * Cmd<Msg> =
    {
        Campaign = campaign
        CampaignId = user.CampaignId
        Player = user.UserName
        CharacterName = CharacterName ""
        Aspects = []
        Abilities = []
        Finished = None
    }
    |> addStartingAspects
    |> addAbilities
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
        |> withoutCommands

    | UpdateAspect aspect ->
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
        |> withoutCommands

    | BackToCampaignClicked _ ->
        currentModel
        |> withoutCommands

    | FinishClicked ->
        { currentModel with Finished = Some true }
        |> withoutCommands

