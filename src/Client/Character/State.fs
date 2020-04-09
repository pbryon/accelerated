module Character.State

open Elmish
open Elmish.Helper

open Global
open Domain.Campaign
open Character.Types

let init (user: UserData) (campaign: Campaign option): Model * Cmd<Msg> =
    {
        Campaign = campaign
        CampaignId = user.CampaignId
        Player = user.UserName
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCharacter campaign ->
        { currentModel with Campaign = Some campaign }
        |> withoutCommands
