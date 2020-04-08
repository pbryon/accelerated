module Elmish.Helper

open Elmish

let withAdditionalCommand cmd (model, cmds) =
  model, (Cmd.batch [cmds ; cmd])


let withCommand (cmds : Cmd<'a>) model =
  model, cmds

let withMsg msg model =
    model, Cmd.ofMsg msg

let withoutCommands model =
  model, Cmd.none