module Campaign.State

open Elmish

open Global
open Elmish.Helper

open Domain
open Domain.Campaign
open Domain.System
open Types

let private resetAbilities model =
    match model.CampaignType with
    | CampaignType.NotSelected ->
        { model with Abilities = [] }
    | CampaignType.Core ->
        { model with Abilities = FateCore.defaultSkillList }
    | CampaignType.FAE ->
        { model with Abilities = FateAccelerated.defaultApproaches }

let private resetRefresh model =
    match model.CampaignType with
    | CampaignType.NotSelected ->
        { model with Refresh = Refresh 0 }
    | _ ->
        { model with Refresh = Refresh 3 }

let private toggleAbilityType model =
    match model.AbilityType with
    | AbilityType.Default ->
        { model with
            AbilityType = AbilityType.Custom
            NewAbility = None }
    | AbilityType.Custom ->
        { model with
            AbilityType = AbilityType.Default
            NewAbility = None }

let private renameAbility (list: string list) (oldName: string) (newName: string) : string list =
     let rename name value x =
        if x = name
        then value
        else x

     list
     |> List.map (rename oldName newName)

let private abilityExists (list: string list) (value: string) =
    list
    |> List.tryFind (fun item -> item = value)
    |> Option.isSome


let init (user: UserData) : Model * Cmd<Msg> =
    let campaign = defaultCoreCampaign
    {
        Player = user.UserName
        CampaignId = user.CampaignId
        CampaignType = CampaignType.NotSelected
        AbilityType = AbilityType.Default
        Abilities = []
        NewAbility = None
        Refresh = campaign.Refresh
        FreeStunts = None
        MaxStunts = None
    }
    |> withoutCommands

let update (msg: Msg) (currentModel: Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        init {
            UserName = currentModel.Player
            CampaignId = currentModel.CampaignId
        }

    | SelectCampaignType selectedType ->
        match selectedType with
        | CampaignType.NotSelected ->
            currentModel
            |> withMsg ResetCampaign
        | CampaignType.Core
        | CampaignType.FAE ->
            { currentModel with
                CampaignType = selectedType }
            |> resetAbilities
            |> resetRefresh
            |> withoutCommands

     | ToggleCustomAbilities ->
        currentModel
        |> toggleAbilityType
        |> resetAbilities
        |> withoutCommands

    | RenameAbility (oldName, newName) ->
        { currentModel with
            Abilities = renameAbility currentModel.Abilities oldName newName }
        |> withoutCommands

    | InputNewAbility ->
        { currentModel with NewAbility = Some "" }
        |> withoutCommands

    | UpdateNewAbility name ->
        { currentModel with NewAbility = Some name }
        |> withoutCommands

    | AddNewAbility ->
        match currentModel.NewAbility with
        | None
        | Some "" ->
            currentModel |> withoutCommands

        | Some value ->
            if abilityExists currentModel.Abilities value
            then failwithf "%s already exists: %s" (abilityName currentModel) value
            else
                { currentModel with
                    NewAbility = None
                    Abilities =
                    [ value ]
                    |> List.append currentModel.Abilities }
                |> withoutCommands

    | SetRefresh value ->
        { currentModel with Refresh = (Refresh value)}
        |> withoutCommands

    | SetFreeStunts value ->
        { currentModel with FreeStunts = value }
        |> withoutCommands

    | SetMaxStunts value ->
        { currentModel with MaxStunts = value }
        |> withoutCommands