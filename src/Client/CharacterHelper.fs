module Characters.Common

open Feliz
open Feliz.Bulma
open Browser.Types

open Domain.Campaign
open App.Icons
open App.Views.Common

let resetButton (buttonText: string) (handler: MouseEvent -> unit) =
    Bulma.level [
        Bulma.levelItem [
            text.hasTextCentered
            prop.children [
                imgButton buttonText fa.trash [
                    button.isDanger
                    prop.onClick handler
                ]
            ]
        ]
    ]

let toggleAbilityType (currentType: AbilityType) =
    match currentType with
    | AbilityType.Default -> AbilityType.Custom
    | AbilityType.Custom -> AbilityType.Default

let private rename name value x =
    if x = name
    then value
    else x

let renameAbility (list: string list) (oldName: string) (newName: string) : string list =
    List.map (fun x -> rename oldName newName x) list

let private abilityTextBox (onTextChange: string -> string -> unit ) (item: string) =
    Bulma.column [
        column.isOneThird
        prop.children [
            Bulma.textInput [
                prop.placeholder item
                prop.defaultValue item
                prop.name item
                prop.onTextChange (fun value -> onTextChange item value)
                prop.style [
                    style.maxWidth (length.perc 90)
                    style.marginTop 5 ]
            ]
        ]
    ]

let abilityTextBoxes (abilities: string list) (onTextChange: string -> string -> unit) =
    abilities
    |> List.map (abilityTextBox onTextChange)

let newItemButton (itemType: string) (currentValue: string option) (onAdd: MouseEvent -> unit) =
    match currentValue with
    | Some _ ->
        Html.none

    | None ->
        let text = sprintf "Add new %s" itemType
        Bulma.column [
            column.is4
            prop.children [
                imgButton text fa.plus [
                    button.isInfo
                    prop.onClick onAdd
                    prop.style [ style.marginTop 5 ]
                ]
            ]
        ]

let newItemInputs
    (name: string)
    (value: string option)
    (onTextChange: string -> unit)
    (onAdd: MouseEvent -> unit) =
    match value with
    | None ->
        Html.none

    | Some _ ->
        let field = sprintf "New_%s" name
        Bulma.column [
            column.isTwoThirds
            prop.style [ style.marginTop 5 ]
            prop.children [
                Bulma.textInput [
                    prop.name field
                    prop.className "input"
                    prop.placeholder (sprintf "New %s" name)
                    prop.defaultValue ""
                    prop.onTextChange onTextChange
                    prop.style [ style.maxWidth (length.perc 60) ]
                ]
                imgButton "Add" fa.check [
                    button.isInfo
                    prop.style [ style.marginLeft 10]
                    prop.onClick onAdd
                ]
            ]
        ]