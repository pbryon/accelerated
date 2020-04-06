module App.View

open Fable.Core.JsInterop

importAll "./style.scss"

open App.Types
open Fulma
open Feliz
open Feliz.Bulma

let view model dispatch =
    let pageHtml currentPage =
        match currentPage with
        | CurrentPage.Index ->
            Index.View.view

        | CurrentPage.CoreCharacter submodel ->
            CoreCharacter.View.view (CoreCharacterMsg >> dispatch) submodel

        | CurrentPage.FAECharacter submodel ->
            FAECharacter.View.view (FAECharacterMsg >> dispatch) submodel

    [
        Navbar.View.view dispatch model.User model.CurrentPage
        Bulma.container [
            container.isFluid
            prop.children [
                pageHtml model.CurrentPage
            ]
        ]
        Bulma.footer [
            Bulma.content [
                text.hasTextCentered
                prop.children [ SafeComponents.View.view ]
            ]
        ]
    ]
    |> Html.div
