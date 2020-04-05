module Navbar.View

open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Fulma
open Global
open App.Types

let private menuItem label page currentPage =
  let isActive =
    match currentPage with
    | CurrentPage.Index _ when page = Page.Index ->
        true

    | CurrentPage.Characters _ when page = Page.Characters ->
        true

    | _ ->
        false

  Navbar.Item.a
    [
      Navbar.Item.IsActive isActive
      Navbar.Item.Props [ Href <| toHash page ]
    ]
    [
      str label
    ]

// let private viewLoginLogout dispatch user currentPage =
//   match user with
//   | None ->
//       menuItem "Login" Page.Login currentPage

//   | Some user ->
//       Navbar.Item.a
//         [
//           Navbar.Item.Props [ OnClick (fun _ -> Logout |> dispatch) ]
//         ]
//         [
//           str <| "Logout " + user.UserName
//         ]

let private navbarStart dispatch user currentPage =
    Navbar.Start.div []
        [
          menuItem "Home" Page.Index currentPage
          menuItem "Characters" Page.Characters currentPage
          //viewLoginLogout dispatch user currentPage
        ]

let private navbarEnd =
  Navbar.End.div []
    [
      Navbar.Item.div []
        [
          Field.div [ Field.IsGrouped ]
            [
              Control.p [ ]
                [
                  Button.a
                    [
                      Button.Props [ Href "https://github.com/pbryon/accelerated" ]
                    ]
                    [
                      Icon.icon [ ] [ Fa.i [ Fa.Brand.Github ] [] ]
                      span [ ] [ str "Source" ]
                    ]
                ]
            ]
        ]
    ]

let view dispatch user currentPage =
  div [ ClassName "navbar-bg" ]
    [
      Container.container [ Container.IsFullHD ]
        [
          Navbar.navbar [ Navbar.Color IsPrimary ]
            [
              Navbar.Brand.div [ ]
                [
                  Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                    [
                      Heading.p [ Heading.Is4 ]
                        [ str "Accelerated" ]
                    ]
                ]
              Navbar.menu []
                [
                  navbarStart dispatch user currentPage
                  navbarEnd
                ]
            ]
        ]
    ]
