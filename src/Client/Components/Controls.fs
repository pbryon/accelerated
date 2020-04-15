module App.Views.Controls

open Feliz
open Feliz.Bulma
open Browser.Types

open App.Icons
open Browser.Types

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
            //buttons.isCentered
            buttons.hasAddons
            prop.children (items |> List.map createButton)
        ]
    ]

let imgButton (text: string) icon (props: IReactProperty list) =
    Bulma.button [
        yield! props
        prop.children [
            Bulma.icon [
                prop.classes [ icon ]
            ]
            if text = "" then Html.none else Html.span text
        ]
    ]

let imgButtonRight (text: string) icon (props: IReactProperty list) =
    Bulma.button [
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
                    button.isDanger
                    prop.onClick handler
                ]
            ]
        ]
    ]

let newItemButton (newItemValue: string option) (onAdd: MouseEvent -> unit) =
    match newItemValue with
    | Some _ ->
        Html.none

    | None ->
        Bulma.column [
            column.is4
            prop.children [
                imgButton "" Fa.plus [
                    button.isInfo
                    prop.onClick onAdd
                    prop.style [ style.marginTop 5 ]
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
                Bulma.textInput [
                    prop.name (sprintf "New_%s" name)
                    prop.placeholder (sprintf "New %s" name)
                    prop.defaultValue ""
                    prop.onTextChange onTextChange
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
                imgButton "Add" Fa.check [
                    button.isInfo
                    prop.style [ style.marginLeft 10]
                    prop.disabled isEmpty
                    prop.onClick onAdd
                ]
            ]
        ]

let addonGroup (items: ReactElement list) =
    Bulma.field [
        field.hasAddons
        prop.children [
            yield! items |> List.map (fun x ->
                Bulma.control [ x] )
        ]
    ]

let addonButton (text: string) (width: IStyleAttribute) =
    Bulma.button [
        button.isPrimary
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