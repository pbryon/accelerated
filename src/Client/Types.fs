module App.Types

open Global

[<RequireQualifiedAccess>]
type CurrentPage =
    | Index
    | CoreCharacter of CoreCharacter.Types.Model
    | FAECharacter of FAECharacter.Types.Model

type Model = {
    User: UserData option
    CurrentPage: CurrentPage
}

type Msg =
    | CoreCharacterMsg of CoreCharacter.Types.Msg
    | FAECharacterMsg of FAECharacter.Types.Msg
    | LoggedIn of UserData
    | LoggedOut
    | Logout