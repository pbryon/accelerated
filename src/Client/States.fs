module App.State

open Elmish
open Elmish.Helper
open Elmish.Navigation
open Fable.Core

open Global
open App.Types
open Domain.Campaign
open System

let loadUser () : UserData option =
//   let userDecoder = Decode.Auto.generateDecoder<UserData>()
//   match LocalStorage.load userDecoder LocalStorageUserKey with
//   | Ok user -> Some user
//   | Error _ -> None
    let guid = Guid.NewGuid()
    Some {
        UserName = "Jos"
        CampaignId = (CampaignId guid)
    }

let private navigateTo page model =
    model
    |> withCommand (page |> toHash |> Navigation.newUrl)

let urlUpdate (result : Page option) model =
    match result with
    | None ->
        JS.console.error("Error parsing URL: " + Browser.Dom.window.location.href)
        model |> navigateTo Page.Index

    | Some Page.Index ->
        { model with CurrentPage = CurrentPage.Index}
        |> withoutCommands

    | Some Page.Characters ->
        match model.User with
        | Some user ->
            let submodel,cmd = Characters.State.init user
            { model with CurrentPage = CurrentPage.Characters submodel }
            |> withCommand (Cmd.map CharacterMsg cmd)

        | None ->
            model |> navigateTo Page.Index

let init result : Model * Cmd<Msg> =
    let user : UserData option = loadUser ()
    let model = {
        CurrentPage = CurrentPage.Index
        User = user
    }
    urlUpdate result model

let private withCurrentPage page model =
    { model with CurrentPage = page }

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