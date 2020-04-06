module FAECharacter.State

open Elmish

open Global
open Elmish.Helper
open CharacterHelper

open Domain.Campaign
open FAECharacter.Types

let init (user: UserData) : Model * Cmd<Msg> =
    {
        Campaign = Some defaultFAECampaign
        CampaignId = user.CampaignId
        Approaches = AbilityType.Default
        Character = None
        Player = user.UserName
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        { currentModel with
            Campaign = Some defaultFAECampaign
            Character = None
            Approaches = AbilityType.Default }
        |> withoutCommands

     | ToggleCustomApproaches ->
        { currentModel with
            Approaches = toggleAbilityType currentModel.Approaches }
        |> withoutCommands

    | RenameApproach (oldName, newName) ->
        { currentModel with
            Campaign =
                match currentModel.Campaign with
                | Some campaign ->
                    Some {
                        campaign with
                            ApproachList = renameAbility campaign.ApproachList oldName newName
                    }

                | None -> None
        }
        |> withoutCommands
