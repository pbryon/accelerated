module App.Views.Layouts

open Feliz
open Feliz.Bulma

type ColumnDefinition = {
    Size: IReactProperty list
    Align: IStyleAttribute
    Content: ReactElement list
}

let box (title: string) (children: List<ReactElement>) =
    let title3 = Bulma.title3 title
    Bulma.box [
        prop.className "main-box"
        prop.style [ style.marginTop 10; style.padding 15 ]
        prop.children [
            Bulma.content [
                text.hasTextCentered
                prop.children (title3 :: children)
            ]
        ]
    ]

let private createColumn (item: ColumnDefinition) =
    Bulma.column [
        yield! item.Size
        prop.style [ item.Align ]
        prop.children item.Content
    ]

let labelCol content =
    {
        Size = [ column.is2; column.isOffset1 ]
        Align = style.textAlign.left
        Content = content
    }

let colLayout (cols: ColumnDefinition list) =
    Bulma.columns [
        prop.children (cols |> List.map createColumn)
    ]

let fluidColLayout (elements: ReactElement list) =
    Bulma.columns [
        columns.isMultiline
        columns.isGapless
        prop.children elements
    ]

[<RequireQualifiedAccess>]
module Debug =
    let private enableDebugMode = true

    let view model =
        if enableDebugMode
        then
        [
            Html.h3 "Model"
            Html.div (sprintf "%A" model)
        ]
        else []