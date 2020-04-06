module Elmish.Common

open Feliz
open Feliz.Bulma
open Browser.Types

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

type ButtonState = {
    Text: string
    Active: bool
    Color: IReactProperty
    OnClick: MouseEvent -> unit
}

let private createButton (item: ButtonState) : ReactElement =
    Bulma.button [
        prop.text item.Text
        item.Color
        prop.onClick item.OnClick
        prop.className [ item.Active, "is-active"; not item.Active, "is-light" ]
    ]

let buttonGroup (items: ButtonState list) =
    [
        Bulma.buttons [
            buttons.isCentered
            buttons.hasAddons
            prop.children (items |> List.map createButton)
        ]
    ]

type ColumnDefinition = {
    Size: IReactProperty list
    Content: ReactElement list
    Align: IStyleAttribute
}

let private createColumn (item: ColumnDefinition) =
    Bulma.column [
        yield! item.Size
        prop.style [ item.Align ]
        prop.children item.Content
    ]

let labelCol content =
    {
        Size = [ column.is3; column.isOffset1 ]
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

let fontAwesome icon =
    Bulma.icon [
        prop.classes [ icon ]
    ]

let imgButton (text: string) icon (props: IReactProperty list) =
    Bulma.button [
        yield! props
        prop.children [
            fontAwesome icon
            Html.span text
        ]
    ]

