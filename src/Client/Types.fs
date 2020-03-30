module App.Types

open Global

[<RequireQualifiedAccess>]
type CurrentPage =
    | Index
    | Characters of Characters.Types.Model

type Model = {
    User: UserData option
    CurrentPage: CurrentPage
}

type Msg =
    | CharacterMsg of Characters.Types.Msg
    | LoggedIn of UserData
    | LoggedOut
    | Logout