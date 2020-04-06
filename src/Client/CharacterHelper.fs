module CharacterHelper

open Feliz
open Feliz.Bulma
open Browser.Types

open Domain.Campaign

let resetCampaign (handler: MouseEvent -> unit) =
    Bulma.level [
        Bulma.levelItem [
            text.hasTextCentered
            prop.children [
                Bulma.button [
                    button.isDanger
                    prop.onClick handler
                    prop.children [
                        Bulma.icon [
                            prop.className [ "fas"; "fa-trash" ]
                        ]
                        Html.span "Reset campaign"
                    ]
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
    Bulma.textInput [
        prop.placeholder item
        prop.defaultValue item
        prop.name item
        prop.className "is-4"
        prop.onTextChange (fun value -> onTextChange item value)
    ]

let abilityTextBoxes (abilities: string list) (onTextChange: string -> string -> unit) =
    abilities |> List.map (abilityTextBox onTextChange)