module Elmish.Common

open Feliz
open Feliz.Bulma
open Browser.Types

let box (title: string) (children: List<ReactElement>) =
    let title3 = Bulma.title3 title
    Bulma.box [
        Bulma.content [
            text.hasTextCentered
            prop.children (title3 :: children)
        ]
    ]