module CoreCharacter.State

open Elmish

open Global
open Elmish.Helper
open Characters.Common

open Domain.Campaign
open Domain.Characters
open CoreCharacter.Types
open Domain.System

let init (user: UserData) : Model * Cmd<Msg> =
    {
        Campaign = Some defaultCoreCampaign
        CampaignId = user.CampaignId
        Skills = AbilityType.Default
        Character = None
        NewSkill = None
        Player = user.UserName
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        { currentModel with
            Campaign = Some defaultCoreCampaign
            Character = None
            NewSkill = None
            Skills = AbilityType.Default }
        |> withoutCommands

    | ResetCharacter ->
        { currentModel with
            Character = createCoreCharacter currentModel.Campaign }
        |> withoutCommands

     | ToggleCustomSkills ->
        { currentModel with
            Skills = toggleAbilityType currentModel.Skills
            NewSkill = None }
        |> withoutCommands

    | RenameSkill (oldName, newName) ->
        match currentModel.Campaign with
        | None ->
            currentModel |> withoutCommands

        | Some campaign ->
            let approaches = renameAbility campaign.SkillList oldName newName

            { currentModel with
                Campaign = Some {
                    campaign with SkillList = approaches
                }}
            |> withoutCommands

    | InputNewSkill ->
        { currentModel with NewSkill = Some "Untitled" }
        |> withoutCommands

    | UpdateNewSkill name ->
        { currentModel with NewSkill = Some name }
        |> withoutCommands

    | AddNewSkill ->
        match currentModel.Campaign with
        | None ->
            currentModel
            |> withoutCommands

        | Some campaign ->
            let newSkill = defaultArg currentModel.NewSkill ""
            let exists =
                campaign.SkillList
                |> List.tryFind (fun item -> item = newSkill)

            if exists.IsSome
            then failwithf "Skill already exists: %s" newSkill
            else
                let skills =
                    [ newSkill ]
                    |> List.append campaign.SkillList

                { currentModel with
                    Campaign = Some {
                        campaign with SkillList = skills
                    }
                    NewSkill = None
                }
                |> withoutCommands

    | SetRefresh value ->
        match currentModel.Campaign with
        | None ->
            currentModel
            |> withoutCommands

        | Some campaign ->
            { currentModel with
                Campaign = Some {
                    // TODO: add type constraint for posint
                    campaign with Refresh = (Refresh value)
                }}
            |> withoutCommands