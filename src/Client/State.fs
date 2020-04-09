module App.State

open System

open Elmish
open Elmish.Helper
open Elmish.Navigation
open Fable.Core

open Global
open App.Types
open Domain.Campaign
open Domain.System

let loadUser () : UserData option =
//   let userDecoder = Decode.Auto.generateDecoder<UserData>()
//   match LocalStorage.load userDecoder LocalStorageUserKey with
//   | Ok user -> Some user
//   | Error _ -> None
    let guid = Guid.NewGuid()
    Some {
        UserName = (PlayerName "Jos")
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

    | Some Page.CampaignCreation ->
        match model.User with
        | Some user ->
            let submodel, cmd = Campaign.State.init user
            { model with CurrentPage = CurrentPage.CampaignCreation submodel }
            |> withCommand (Cmd.map CampaignMsg cmd)

        | None ->
            model |> navigateTo Page.Index

    | Some Page.CharacterCreation ->
        match model.User with
        | Some user ->
            let submodel, cmd = Character.State.init user None
            { model with CurrentPage = CurrentPage.CharacterCreation submodel }
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
    | CampaignMsg msg, CurrentPage.CampaignCreation submodel ->
        let (campaignModel, campaignCmd) = Campaign.State.update msg submodel
        let isDone = defaultArg campaignModel.Finished false

        match (isDone, model.User) with
        | true, Some user ->
            let campaign = Campaign.Types.asCampaign campaignModel
            let (character, characterCmd) = Character.State.init user (Some campaign)
            model
            |> withCurrentPage (CurrentPage.CharacterCreation character)
            |> withCommand (Cmd.map CharacterMsg characterCmd)

        | _, _ ->
            model
            |> withCurrentPage (CurrentPage.CampaignCreation campaignModel)
            |> withCommand (Cmd.map CampaignMsg campaignCmd)

    | LoggedIn newUser, _ ->
        { model with User = Some newUser }
        |> navigateTo Page.Index

    | LoggedOut, _ ->
        { model with User = None }
        |> withCurrentPage CurrentPage.Index
        |> navigateTo Page.Index

    | Logout, _ ->
        model
        |> withCommand deleteUserCmd

    | _, _ ->
        model
        |> withoutCommands