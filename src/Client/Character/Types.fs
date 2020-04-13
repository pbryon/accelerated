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