module App.Views.Layouts

open Feliz
open Feliz.Bulma

type ColumnDefinition = {
    Props: IReactProperty list
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

let private createColumn index (item: ColumnDefinition) =
    Bulma.column [
        yield! item.Props
        prop.style [ style.textAlign.left ]
        prop.children item.Content
        prop.className (sprintf "column-%i" (index + 1))
    ]

let labelCol content =
    {
        Props = [ column.is2; column.isOffset1 ]
        Content = content
    }

let emptyLabelCol =
    labelCol []

let colLayout (cols: ColumnDefinition list) =
    Bulma.columns [
        prop.children (cols |> List.mapi createColumn)
    ]

let fluidColLayout (elements: ReactElement list) =
    Bulma.columns [
        columns.isMultiline
        columns.isGapless
        prop.children elements
    ]

let row (padding: IReactProperty option) (elements: ReactElement list) =
    Bulma.column [
        column.isFull
        if padding.IsSome then
            padding.Value
        prop.children elements
    ]

[<RequireQualifiedAccess>]
module Debug =
    let private enableDebugMode = true

    let view (title: string) model =
        if enableDebugMode
        then [
                Html.h3 title
                Html.div (sprintf "%A" model)
            ]
        else []