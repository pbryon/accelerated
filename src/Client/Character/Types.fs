module Character.Types

open Global
open Domain.Campaign
open Domain.System

type Model = {
    Campaign: Campaign option
    CampaignId: CampaignId
    Player: PlayerName
    CharacterName: CharacterName
    Finished: bool option
}

type Msg =
    | ResetCharacter of Campaign
    | SetPlayerName of string
    | SetCharacterName of string
    | FinishClicked

let isDone model =
    Some model
    |> validate (fun x -> x.Campaign.IsSome)
    |> validate (fun x ->
        match x.Player with
        | PlayerName "" -> false
        | _ -> true)
    |> validate (fun x ->
        match x.CharacterName with
        | CharacterName "" -> false
        | _ -> true)
    |> validate (fun x -> false) // TODO: finish this logic
    |> Option.isSome