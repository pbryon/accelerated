module Characters.View

open Feliz
open Feliz.Bulma

open Elmish.Common

open Characters.Types
open Domain.Campaign

let chooseCampaignType dispatch model : List<ReactElement> =
    [
        Bulma.columns [
            Bulma.column [
                Bulma.button [
                    prop.text "Fate Core"
                    prop.className "is-pulled-right"
                    button.isPrimary
                    prop.onClick (fun _ -> SelectCoreCampaign |> dispatch)
                ]
            ]
            Bulma.column [
                Bulma.button [
                    prop.text "Fate Accelerated"
                    prop.className "is-pulled-left"
                    button.isPrimary
                    prop.onClick (fun _ -> SelectFAECampaign |> dispatch)
                ]
            ]
        ]
    ]

let view dispatch model =
    match model with
    | x when x.Campaign = NotSelected ->
        chooseCampaignType dispatch model
        |> box "Select a campaign type"
    | x when x.Campaign <> NotSelected ->
        [
            Html.div (sprintf "Selected campaign type: %A" x.Campaign)
            Bulma.button [
                prop.text "Reset campaign"
                button.isDanger
                prop.onClick (fun _ -> ResetCampaign |> dispatch )
            ]
        ]
        |> box "Selected campaign type"
    | _ ->
        Html.none

