module Characters.State

open Elmish

open Global
open Elmish.Helper
open Domain.Campaign
open Characters.Types

let init (user: UserData) : Model * Cmd<Msg> =
    let initialModel = {
        CampaignType = CampaignType.NotSelected
        Campaign = None
        CampaignId = user.CampaignId
        Abilities = AbilityType.Default
        Player = user.UserName
        Character = None
    }
    initialModel
    |> withoutCommands

let toggleCustomAbilities model =
    match model.Abilities with
    | AbilityType.Default -> { model with Abilities = AbilityType.Custom }
    | AbilityType.Custom -> { model with Abilities = AbilityType.Default }

let rename name value xs =
    let replaceValue name value x =
        if x = name
        then value
        else x
    List.map (fun x -> replaceValue name value x) <| xs

let renameAbility model (oldName: string) (newName: string) : Campaign option =
    match model.Campaign with
    | Some (Core campaign) ->
        Some (Core { campaign with
                        SkillList =
                            campaign.SkillList
                            |> rename oldName newName })

    | Some (FAE campaign) ->
        Some (FAE { campaign with
                        ApproachList =
                            campaign.ApproachList
                            |> rename oldName newName })

    | None ->
        None


let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        { currentModel with
             CampaignType = CampaignType.NotSelected
             Campaign = None
             Abilities = AbilityType.Default }
        |> withoutCommands

    | SelectCoreCampaign ->
        { currentModel with
            Campaign = Some (Core defaultCoreCampaign);
            CampaignType = CampaignType.Core }
        |> withoutCommands

    | SelectFAECampaign ->
        { currentModel with
            Campaign = Some (FAE defaultFAECampaign);
            CampaignType = CampaignType.FAE }
        |> withoutCommands

    | ToggleCustomAbilities ->
        toggleCustomAbilities currentModel
        |> withoutCommands

    | RenameAbility (oldName, newName) ->
        { currentModel with
            Campaign = renameAbility currentModel oldName newName }
        |> withoutCommands