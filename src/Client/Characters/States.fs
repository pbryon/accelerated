module Characters.State

open Domain.Campaign
open Elmish
open Characters.Types

let init () : Model * Cmd<Msg> =
    let initialModel = {
        Campaign = NotSelected
    }
    initialModel, Cmd.none

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        init()

    | SelectCoreCampaign ->
        { currentModel with Campaign = (Core defaultCoreCampaign) }, Cmd.none

    | SelectFAECampaign ->
        { currentModel with Campaign = (FAE defaultFAECampaign)}, Cmd.none