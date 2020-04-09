module Character.View

open Feliz
open Feliz.Bulma

open App.Views.Layouts

let view dispatch model =
    [
        Html.div "Under construction"
        yield! Debug.view model
    ]
    |> box "Character creation"