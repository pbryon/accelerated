module Campaign.State

open Elmish

open Global
open Elmish.Helper

open Domain
open Domain.Campaign
open Domain.System
open Types
open Fable.Core

let private initialAspects = [
    HighConceptAndTrouble
]

let init (user: UserData) : Model * Cmd<Msg> =
    let campaign = defaultCoreCampaign
    {
        Player = user.UserName
        CampaignId = user.CampaignId
        CampaignType = None
        AbilityType = AbilityType.Default
        Abilities = []
        NewAbility = None
        Aspects = initialAspects
        Refresh = campaign.Refresh
        FreeStunts = None
        MaxStunts = None
        Finished = None
    }
    |> withoutCommands

let private resetAbilities model =
    match model.CampaignType with
    | None ->
        { model with Abilities = [] }
    | Some CampaignType.Core ->
        { model with Abilities = FateCore.defaultSkillList }
    | Some CampaignType.FAE ->
        { model with Abilities = FateAccelerated.defaultApproaches }

let private resetRefresh model =
    match model.CampaignType with
    | None ->
        { model with Refresh = Refresh 0 }
    | Some _ ->
        { model with Refresh = Refresh 3 }

let private setInitialAspectSelection model =
    let aspects =
        match model.CampaignType with
        | None -> []

        | Some CampaignType.Core -> [
            HighConceptAndTrouble
            PhaseTrio
            ]

        | Some CampaignType.FAE -> [
            HighConceptAndTrouble
            ExtraAspects 1
            ]
    { model with Aspects = aspects }

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

let private addAspect model aspect =
    let existing = findAspectLike model aspect
    if existing.IsSome then
        model
    else
        { model with Aspects = [ aspect ] |> List.append model.Aspects }
    |> withoutCommands

let private toggleAspects model aspect =
    let existing = findAspectLike model aspect

    if existing.IsNone then
        { model with
            Aspects = [ aspect ] |> List.append initialAspects }
    else
        JS.console.log (sprintf "Similar Aspect: %A" existing)
        let aspects =
            if existing.Value = aspect then
                initialAspects
            else
                [ aspect ]
                |> List.append model.Aspects
                |> List.filter (fun x -> x <> existing.Value)
        { model with Aspects = aspects }

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
        | CampaignType.Core
        | CampaignType.FAE ->
            { currentModel with
                CampaignType = Some selectedType }
            |> resetAbilities
            |> resetRefresh
            |> setInitialAspectSelection
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

    | AddAspect aspect ->
        addAspect currentModel aspect

    | ToggleAspect aspect ->
        toggleAspects currentModel aspect

    | SetRefresh value ->
        { currentModel with Refresh = (Refresh value)}
        |> withoutCommands

    | SetFreeStunts value ->
        { currentModel with FreeStunts = value }
        |> withoutCommands

    | SetMaxStunts value ->
        { currentModel with MaxStunts = value }
        |> withoutCommands

    | FinishClicked ->
        { currentModel with Finished = Some (isDone currentModel) }
        |> withoutCommands
