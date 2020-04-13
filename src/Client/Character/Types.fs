module Character.Types

open Global
open Domain.Campaign
open Domain.System

type AbilityRank =
    | Ok of int
    | Errored of int
    | Default

type Ability = {
    Rank: AbilityRank
    Name: string
}

type Model = {
    Campaign: Campaign option
    CampaignId: CampaignId
    Player: PlayerName
    CharacterName: CharacterName
    Aspects: Aspect list
    Abilities: Ability list
    Finished: bool option
}

type Msg =
    | ResetCharacter of Campaign option
    | SetPlayerName of string
    | SetCharacterName of string
    | AddAspect of Aspect
    | UpdateAspect of Aspect
    | UpdateAbility of Ability
    | BackToCampaignClicked of UserData
    | FinishClicked

let validateAspects model =
    let valid =
        model.Aspects
        |> List.filter (fun aspect ->
            match aspect with
            | Aspect.HighConcept name
            | Aspect.Trouble name ->
                "" <> Convert.aspectName name
            | Aspect.PhaseTrio (_, name) ->
                "" <> Convert.aspectName name
            | Aspect.Other _ ->
                true
        )

    valid.Length = model.Aspects.Length

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x -> "" <> Convert.playerName x.Player)
    |> validate (fun x -> "" <> Convert.characterName x.CharacterName)
    |> validate validateAspects
    |> Option.isSome