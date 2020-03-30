module App.State

open Elmish
open Elmish.Helper
open Elmish.Navigation
open Global
open App.Types

let init () : Model * Cmd<Msg> =
    let initialModel = {
        CurrentPage = CurrentPage.Index
        User = None
    }
    initialModel, Cmd.none

let private withCurrentPage page model =
   { model with CurrentPage = page }

let private navigateTo page model =
  model
  |> withCommand (page |> toAnchor |> Navigation.newUrl)

let private deleteUserCmd =
  Cmd.none // TODO
  // Cmd.OfFunc.either LocalStorage.delete LocalStorageUserKey (fun _ -> LoggedOut) StorageFailure

let update msg model =
    match msg, model.CurrentPage with
    | CharacterMsg msg, CurrentPage.Characters submodel ->
        let (character, characterCmd) = Characters.State.update msg submodel
        model
        |> withCurrentPage (CurrentPage.Characters character)
        |> withCommand (Cmd.map CharacterMsg characterCmd)

    | LoggedIn newUser, _ ->
      { model with User = Some newUser }
      |> navigateTo Page.Index

    | LoggedOut, _ ->
        { model with User = None }
        |> withCurrentPage CurrentPage.Index
        |> navigateTo Page.Index

    | Logout, _ ->
        model |> withCommand deleteUserCmd

    | _, _ ->
      model |> withoutCommands