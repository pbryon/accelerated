module Index.View

open Feliz
open Feliz.Bulma

let view =
    Bulma.container [
        prop.className "index"
        prop.style [ style.textAlign.center ]
        prop.children [
            Bulma.title3 "Accelerated"
            Html.p "Welcome to the Accelerated campaign and character tools."
            Html.p "To create a character, click on the links above."
        ]
  ]

  // TODO: update this and other views to Feliz.Bulma 2.xx API