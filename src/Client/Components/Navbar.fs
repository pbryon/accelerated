module Navbar.View

open Feliz
open Feliz.Bulma

open Global
open App.Icons
open App.Types
open App.Views.Controls

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
    Bulma.navbarStart.div [
        Bulma.navbarItem.a [
            yield! isMenuItemActive Page.CharacterCreation currentPage
            prop.href (toHash Page.CampaignCreation)
            prop.text "Characters"
        ]
        Bulma.navbarItem.a [
            yield! isMenuItemActive Page.Copyright currentPage
            prop.href (toHash Page.Copyright)
            prop.text "Copyright"
        ]
        //viewLoginLogout dispatch user currentPage
    ]


let private navbarEnd =
    Bulma.navbarEnd.div [
        Bulma.navbarItem.div [
            Bulma.field.div [
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
        color.isPrimary
        prop.children [
            Bulma.container [
                container.isFluid
                prop.children [
                    Bulma.navbarBrand.div [
                        Bulma.navbarItem.a [
                            prop.href "#"
                            prop.children [
                                Bulma.title.h4 "Accelerated"
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