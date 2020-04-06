module Elmish.Common

open Feliz
open Feliz.Bulma

let alignLeft = prop.style [ style.textAlign.left ]

let box (title: string) (children: List<ReactElement>) =
    let title3 = Bulma.title3 title
    Bulma.box [
        prop.className "main-box"
        prop.style [ style.marginTop 10 ]
        prop.children [
            Bulma.content [
                text.hasTextCentered
                prop.children (title3 :: children)
            ]
        ]
    ]