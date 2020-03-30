module Characters.View

open Fable.React
open Fable.React.Props
open Fulma
open Characters.Types

let view dispatch model =
  [
    Container.container []
      [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
          [ Heading.h3 [] [ str "Select a campaign type" ] ]
        Columns.columns []
          [
            Column.column [] [ button [OnClick (fun _ -> SelectCoreCampaign |> dispatch) ] [ str "Core"] ]
            Column.column [] [ button [OnClick (fun _ -> SelectFAECampaign |> dispatch ) ] [ str "FAE"] ]
            Column.column [] [ button [OnClick (fun _ -> ResetCampaign |> dispatch) ] [ str "Reset"] ] ]
         ]
  ]
  |> div []
