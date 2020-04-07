module Navbar.View

open Feliz
open Feliz.Bulma

open Global
open App.Icons
open App.Types
open App.Views.Common

let private isMenuItemActive page currentPage =
    match currentPage with
    | CurrentPage.Index _ when page = Page.Index ->
        [ navbarItem.isActive ]

    | CurrentPage.CoreCharacter _ when page = Page.CoreCharacter ->
        [ navbarItem.isActive ]

    | CurrentPage.FAECharacter _ when page = Page.FAECharacter ->
        [ navbarItem.isActive ]

    | _ ->
        []

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
    Bulma.navbarStart [
        Bulma.navbarItemA [
            yield! isMenuItemActive Page.CoreCharacter currentPage
            prop.href (toHash Page.CoreCharacter)
            prop.text "Fate Core"
        ]
        Bulma.navbarItemA [
            yield! isMenuItemActive Page.FAECharacter currentPage
            prop.href (toHash Page.FAECharacter)
            prop.text "Fate Accelerated"
        ]
        //viewLoginLogout dispatch user currentPage
    ]

let private navbarEnd =
    Bulma.navbarEnd [
        Bulma.navbarItemDiv [
            Bulma.field [
                field.isGrouped
                prop.children [
                    imgButton "Source" fa.github [
                        prop.href "https://github.com/pbryon/accelerated"
                    ]
                ]
            ]
        ]
    ]

let view dispatch user currentPage =
    Bulma.navbar [
        navbar.isPrimary
        prop.children [
            Bulma.container [
                container.isFluid
                prop.children [
                    Bulma.navbarBrand [
                        Bulma.navbarItemA [
                            prop.href "#"
                            prop.children [
                                Bulma.title4 "Accelerated"
                            ]
                        ]
                    ]
                    Bulma.navbarMenu [
                        navbarStart dispatch user currentPage
                        navbarEnd
                    ]
                ]
            ]
        ]
    ]