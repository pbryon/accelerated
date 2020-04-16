module Campaign.Types

open Global
open Utils
open Domain.System
open Domain.Campaign
open Domain.FateCore
open Domain.FateAccelerated

type Model = {
    Player: PlayerName
    CampaignId: CampaignId
    CampaignType: CampaignType option
    AbilityType: AbilityType
    Abilities : string list
    NewAbility: string option
    Aspects: AspectSelection list
    Refresh: Refresh
    FreeStunts: int option
    MaxStunts: int option
    Finished: bool option
}

type Msg =
| ResetCampaign
| SelectCampaignType of CampaignType
| ToggleCustomAbilities
| RenameAbility of string * string
| InputNewAbility
| UpdateNewAbility of string
| AddNewAbility
| AddAspect of AspectSelection
| ToggleAspect of AspectSelection
| SetRefresh of int
| SetFreeStunts of int option
| SetMaxStunts of int option
| FinishClicked

let abilityName model =
    match model.CampaignType with
    | None -> "Ability"
    | Some CampaignType.Core -> "Skill"
    | Some CampaignType.FAE -> "Approach"

let abilityNamePlural model =
    match model.CampaignType with
    | None -> "Abilities"
    | Some CampaignType.Core -> "Skills"
    | Some CampaignType.FAE -> "Approaches"

let abilityType campaign =
    match campaign with
    | Campaign.Core core ->
        if core.SkillList = defaultSkillList
        then AbilityType.Default
        else AbilityType.Custom
    | Campaign.FAE fae ->
        if fae.ApproachList = defaultApproachList
        then AbilityType.Default
        else AbilityType.Custom


let asCampaign model : Campaign =
    let campaign = defaultArg model.CampaignType CampaignType.Core

    match campaign with
    | CampaignType.Core ->
        Campaign.Core {
            defaultCoreCampaign with
                SkillList = model.Abilities
                Aspects = model.Aspects
                Refresh = model.Refresh
                FreeStunts = defaultArg model.FreeStunts 0
                MaxStunts = model.MaxStunts
        }

    | CampaignType.FAE ->
        Campaign.FAE {
            defaultFAECampaign with
                ApproachList = model.Abilities
                Aspects = model.Aspects
                Refresh = model.Refresh
                FreeStunts = defaultArg model.FreeStunts 0
                MaxStunts = model.MaxStunts
        }

let hasAspect model aspect =
    model.Aspects
    |> contains aspect

let aspectLike aspect other =
    match aspect with
    | _ when aspect = other -> true

    | ExtraAspects _ ->
        match other with
        | ExtraAspects _ -> true
        | _ -> false

    | _ -> false

let findAspectLike model aspect =
    model.Aspects
    |> firstLike aspectLike aspect

let isDone model =
    Some model
    |> validate (fun x -> x.CampaignType.IsSome)
    |> validate (fun x -> x.Abilities.Length > 0)
    |> validate (fun x -> x.Aspects.Length > 0)
    |> validate (fun x -> x.FreeStunts.IsSome)
    |> Option.isSome