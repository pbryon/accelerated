module Elmish.Helper

open Elmish

let withAdditionalCommand cmd (model, cmds) =
  model, (Cmd.batch [cmds ; cmd])


let withCommand (cmds : Cmd<'a>) model =
  model, cmds

let withoutCommands model =
  model, Cmd.none

module Icons =
    type fa =
        static member inline github = "fab fa-github"
        static member inline plus = "fas fa-plus"
        static member inline trash = "fas fa-trash-alt"