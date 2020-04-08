module App.Types

open Global

[<RequireQualifiedAccess>]
type CurrentPage =
    | Index
    | CampaignCreation of Campaign.Types.Model

type Model = {
    User: UserData option
    CurrentPage: CurrentPage
}

type Msg =
    | CampaignMsg of Campaign.Types.Msg
    | LoggedIn of UserData
    | LoggedOut
    | Logout