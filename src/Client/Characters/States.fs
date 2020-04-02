module Characters.State

open Elmish

open Global
open Elmish.Helper
open Domain.Campaign
open Characters.Types

let init (user: UserData) : Model * Cmd<Msg> =
    let initialModel = {
        Campaign = NotSelected
        CampaignId = user.CampaignId
        Player = user.UserName
    }
    initialModel
    |> withoutCommands

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        { currentModel with Campaign = NotSelected }
        |> withoutCommands

    | SelectCoreCampaign ->
        { currentModel with Campaign = (Core defaultCoreCampaign) }
        |> withoutCommands

    | SelectFAECampaign ->
        { currentModel with Campaign = (FAE defaultFAECampaign)}
        |> withoutCommands