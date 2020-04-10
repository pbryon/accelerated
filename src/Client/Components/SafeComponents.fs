module SafeComponents.View

open Feliz
open Fable.React.Props

let separate (sep: string) (items: ReactElement list) =
    items
    |> List.collect (fun x -> [
        x
        Html.text sep
    ])
    |> List.truncate (2 * items.Length - 1)

let view =
    let components =
        [
            Html.a [
                prop.href "https://github.com/SAFE-Stack/SAFE-template"
                prop.text (sprintf "SAFE %s" Version.template)
            ]
            Html.a [
                prop.href "https://saturnframework.github.io"
                prop.text "Saturn"
            ]
            Html.a [
                prop.href "http://fable.io"
                prop.text "Fable"
            ]
            Html.a [
                prop.href "https://elmish.github.io"
                prop.text "Elmish"
            ]
            Html.a [
                prop.href "https://github.com/Zaid-Ajaj/Feliz"
                prop.text "Feliz"
            ]
            Html.a [
                prop.href "https://github.com/Dzoukr/Feliz.Bulma"
                prop.text "Feliz.Bulma"
            ]
        ]
        |> separate ", "

    let version =
        [
            Html.text "Version"
            Html.strong Version.app
            Html.text "powered by: "
        ]
        |> separate " "

    Html.span [
        prop.children [
            yield! version
            yield! components
        ]
    ]