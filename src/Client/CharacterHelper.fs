module CharacterHelper

open Feliz
open Feliz.Bulma
open Browser.Types

open Domain.Campaign
open Icons
open Elmish.Common

let resetCampaign (handler: MouseEvent -> unit) =
    Bulma.level [
        Bulma.levelItem [
            text.hasTextCentered
            prop.children [
                imgButton "Reset campaign" fa.trash [
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
                prop.style [ style.maxWidth (length.perc 90) ]
            ]
        ]
    ]


let abilityTextBoxes
    (abilities: string list)
    (onTextChange: string -> string -> unit)
    (onAdd: MouseEvent -> unit) =

    let addNew = imgButton "Add new" fa.plus [
        button.isInfo
        prop.onClick onAdd
        prop.style [ style.marginTop 5 ]
    ]
    let rows =
        abilities
        |> List.map (abilityTextBox onTextChange)

    [
        Bulma.columns [
            columns.isMultiline
            columns.isGapless
            prop.children ([addNew] |> List.append(rows))
        ]
    ]