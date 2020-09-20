module App.Views.Controls

open Feliz
open Feliz.Bulma
open Browser.Types

open Domain.SystemReference
open App.Icons
open Browser.Types

type ButtonState = {
    Text: string
    Active: bool
    Color: IReactProperty
    OnClick: MouseEvent -> unit
}

let private createButton (item: ButtonState) : ReactElement =
    Bulma.button.a [
        prop.text item.Text
        item.Color
        prop.onClick item.OnClick
        prop.className [
            if item.Active
            then "is-active"
            else "is-light"
        ]
    ]

let buttonGroup (items: ButtonState list) =
    [
        Bulma.buttons [
            //buttons.isCentered
            buttons.hasAddons
            prop.children (items |> List.map createButton)
            prop.style [ style.display.inlineFlex ]
        ]
    ]

let imgButton (text: string) icon (props: IReactProperty list) =
    Bulma.button.a [
        yield! props
        prop.children [
            Bulma.icon [
                prop.classes [ icon ]
            ]
            if text = "" then Html.none else Html.span text
        ]
    ]

let imgButtonRight (text: string) icon (props: IReactProperty list) =
    Bulma.button.a [
        yield! props
        prop.children [
            if text = "" then Html.none else Html.span text
            Bulma.icon [
                prop.classes [ icon ]
            ]
        ]
    ]

let resetButton (buttonText: string) (handler: MouseEvent -> unit) =
    Bulma.level [
        Bulma.levelItem [
            prop.children [
                imgButton buttonText Fa.trash [
                    color.isDanger
                    prop.onClick handler
                ]
            ]
        ]
    ]

let newItemButton (text: string) (onAdd: MouseEvent -> unit) =
    Bulma.column [
        column.is4
        prop.className "add-button"
        prop.children [
            imgButton text Fa.plus [
                prop.className "add-item"
                color.isInfo
                prop.onClick onAdd
                prop.style [ style.marginTop 5 ]
            ]
        ]
    ]

let rulesButton (text: string) (topic: Topic) =
    Html.a [
        prop.className "button is-small"
        prop.href (srdLink topic)
        prop.target.blank
        prop.style [
            style.marginLeft 10
            style.marginTop 3
            style.borderRadius 5
        ]
        prop.children [
            Bulma.icon [
                prop.className Fa.question
            ]
        ]
    ]

let newItemInputs
    (name: string)
    (newItemValue: string option)
    (onTextChange: string -> unit)
    (onAdd: MouseEvent -> unit) =
    match newItemValue with
    | None ->
        Html.none

    | Some value ->
        let isEmpty = value = ""
        Bulma.column [
            column.isTwoThirds
            prop.style [ style.marginTop 5 ]
            prop.children [
                Bulma.input.text [
                    prop.name (sprintf "New_%s" name)
                    prop.placeholder (sprintf "New %s" name)
                    prop.defaultValue ""
                    prop.onTextChange onTextChange
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
                imgButton "Add" Fa.check [
                    color.isInfo
                    prop.style [ style.marginLeft 10]
                    prop.disabled isEmpty
                    prop.onClick onAdd
                ]
            ]
        ]

let addonGroup (className: string) (items: ReactElement list) =
    Bulma.field.div [
        field.hasAddons
        prop.className className
        prop.children [
            yield! items |> List.map (fun x ->
                Bulma.control.div [ x] )
        ]
    ]

let addonButton (text: string) (width: IStyleAttribute) =
    Bulma.button.a [
        color.isPrimary
        prop.text text
        prop.tabIndex -1
        prop.style [ width ]
    ]

let onFocusSelectText =
    prop.onFocus (fun e ->
        let target = e.target :?> HTMLInputElement
        target.select()
    )

let onSelectChange handler =
    prop.onChange (fun (e: Event) ->
        let target = e.target :?> HTMLInputElement
        handler target.value
    )