module App.Types

open Global

[<RequireQualifiedAccess>]
type CurrentPage =
    | Index
    | CampaignCreation of Campaign.Types.Model
    | CharacterCreation of Character.Types.Model
    | Copyright

type Model = {
    User: UserData option
    CurrentPage: CurrentPage
}

type Msg =
    | CampaignMsg of Campaign.Types.Msg
    | CharacterMsg of Character.Types.Msg
    | LoggedIn of UserData
    | LoggedOut
    | Logout