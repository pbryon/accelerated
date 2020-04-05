module App.View

open Fable.Core.JsInterop

importAll "./style.scss"

open Fable.React
open App.Types
open Fulma
open Feliz

let view model dispatch =
  let pageHtml currentPage =
    match currentPage with
    | CurrentPage.Index ->
        Index.View.view

    | CurrentPage.Characters submodel ->
        Characters.View.view (CharacterMsg >> dispatch) submodel

  [
    Navbar.View.view dispatch model.User model.CurrentPage
    pageHtml model.CurrentPage
    Footer.footer []
        [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
            [ SafeComponents.View.view ] ]
  ]
  |> Html.div
