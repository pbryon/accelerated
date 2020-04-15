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

type StuntType =
| Free
| Paid

type Stunt = {
    Name: string
    Description: string
    Ability: string
    Action: Action option
    Activation: StuntActivation option
    Type: StuntType
    Index: int
}

type Model = {
    Campaign: Campaign option
    CampaignId: CampaignId
    Player: PlayerName
    CharacterName: CharacterName
    Aspects: Aspect list
    Abilities: Ability list
    Stunts: Stunt list
    Refresh: int
    Finished: bool option
}

type Msg =
    | ResetCharacter of Campaign option
    | SetPlayerName of string
    | SetCharacterName of string
    | AddAspect of Aspect
    | UpdateAspect of Aspect
    | UpdateAbility of Ability
    | BuyStunt
    | UpdateStunt of Stunt
    | RemoveStunt of int
    | BackToCampaignClicked of UserData
    | FinishClicked