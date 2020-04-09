module Navbar.View

open Feliz
open Feliz.Bulma

open Global
open App.Icons
open App.Types
open App.Views.Buttons

let private isMenuItemActive page currentPage =
    match currentPage with
    | CurrentPage.Index _ when page = Page.Index ->
        [ navbarItem.isActive ]

    | CurrentPage.CampaignCreation _ when page = Page.CharacterCreation ->
        [ navbarItem.isActive ]

    | CurrentPage.CharacterCreation _ when page = Page.CharacterCreation ->
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
            yield! isMenuItemActive Page.CharacterCreation currentPage
            prop.href (toHash Page.CampaignCreation)
            prop.text "Characters"
        ]
        //viewLoginLogout dispatch user currentPage
    ]

let private navbarEnd =
    Bulma.navbarEnd [
        Bulma.navbarItemDiv [
            Bulma.field [
                field.isGrouped
                prop.children [
                    imgButton "Source" Fa.github [
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