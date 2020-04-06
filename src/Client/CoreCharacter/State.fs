module CoreCharacter.State

open Elmish

open Global
open Elmish.Helper
open CharacterHelper

open Domain.Campaign
open CoreCharacter.Types

let init (user: UserData) : Model * Cmd<Msg> =
    {
        Campaign = Some defaultCoreCampaign
        CampaignId = user.CampaignId
        Skills = AbilityType.Default
        Character = None
        Player = user.UserName
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        { currentModel with
            Campaign = Some defaultCoreCampaign
            Character = None
            Skills = AbilityType.Default }
        |> withoutCommands

     | ToggleCustomSkills ->
        { currentModel with
            Skills = toggleAbilityType currentModel.Skills }
        |> withoutCommands

    | RenameSkill (oldName, newName) ->
        { currentModel with
            Campaign =
                match currentModel.Campaign with
                | Some campaign ->
                    Some {
                        campaign with
                            SkillList = renameAbility campaign.SkillList oldName newName
                    }

                | None -> None
        }
        |> withoutCommands
