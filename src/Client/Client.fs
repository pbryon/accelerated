module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Thoth.Fetch
open Fulma

open Shared
open Domain.Campaign

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = {
        Campaign = NotSelected
    }
    initialModel, Cmd.none

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | ResetCampaign ->
        init()

    | SelectCoreCampaign ->
        { currentModel with Campaign = (Core defaultCoreCampaign) }, Cmd.none

    | SelectFAECampaign ->
        { currentModel with Campaign = (FAE defaultFAECampaign)}, Cmd.none

let safeComponents =
    let components =
        span [ ]
           [ a [ Href "https://github.com/SAFE-Stack/SAFE-template" ]
               [ str "SAFE  "
                 str Version.template ]
             str ", "
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://fulma.github.io/Fulma" ] [ str "Fulma" ]

           ]

    span [ ]
        [ str "Version "
          strong [ ] [ str Version.app ]
          str " powered by: "
          components ]

let textCenter = Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

          Container.container []
              [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ Heading.h3 [] [ str "Select a campaign type" ] ]
                Columns.columns []
                    [ Column.column [] [ button [OnClick (fun _ -> SelectCoreCampaign |> dispatch) ] [ str "Core"] ]
                      Column.column [] [ button [OnClick (fun _ -> SelectFAECampaign |> dispatch ) ] [ str "FAE"] ]
                      Column.column [] [ button [OnClick (fun _ -> ResetCampaign |> dispatch) ] [ str "Reset"] ] ] ]

          Footer.footer []
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ]
        ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
