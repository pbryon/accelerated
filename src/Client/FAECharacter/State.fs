module FAECharacter.State

open Elmish

open Global
open Elmish.Helper
open Characters.Common

open Domain.Campaign
open Domain.Characters
open FAECharacter.Types
open Fable.Core

let init (user: UserData) : Model * Cmd<Msg> =
    {
        Campaign = Some defaultFAECampaign
        CampaignId = user.CampaignId
        Approaches = AbilityType.Default
        NewApproach = None
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

    | ResetCharacter ->
        { currentModel with
            Character = createFAECharacter currentModel.Campaign }
        |> withoutCommands

    | ToggleCustomApproaches ->
        { currentModel with
            Approaches = toggleAbilityType currentModel.Approaches }
        |> withoutCommands

    | RenameApproach (oldName, newName) ->
        match currentModel.Campaign with
        | None ->
            currentModel |> withoutCommands

        | Some campaign ->
            let approaches = renameAbility campaign.ApproachList oldName newName

            { currentModel with
                Campaign = Some {
                    campaign with ApproachList = approaches
                }}
            |> withoutCommands

    | InputNewApproach ->
        { currentModel with NewApproach = Some "Untitled" }
        |> withoutCommands

    | UpdateNewApproach name ->
        { currentModel with NewApproach = Some name }
        |> withoutCommands

    | AddNewApproach ->
        match currentModel.Campaign with
        | None ->
            currentModel
            |> withoutCommands

        | Some campaign ->
            let newApproach = defaultArg currentModel.NewApproach ""
            let exists =
                campaign.ApproachList
                |> List.tryFind (fun item -> item = newApproach)

            if exists.IsSome
            then failwithf "Approach already exists: %s" newApproach
            else
                let approaches =
                    [ newApproach ]
                    |> List.append campaign.ApproachList

                { currentModel with
                    Campaign = Some {
                        campaign with ApproachList = approaches
                    }
                    NewApproach = None
                }
                |> withoutCommands
