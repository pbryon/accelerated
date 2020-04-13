module Global

open System

open Elmish.UrlParser
open Domain.Campaign
open Domain.System

type UserData =
  {
    UserName : PlayerName
    CampaignId: CampaignId
    //Token : Server.AuthTypes.JWT // TODO add
  }

[<RequireQualifiedAccess>]
type Page =
    | Index
    | CampaignCreation
    | CharacterCreation

[<RequireQualifiedAccess>]
module Urls =
    let charGen = "character-creation"

let toHash page =
  match page with
  | Page.Index -> "#"
  | Page.CampaignCreation -> sprintf "#%s" Urls.charGen
  | Page.CharacterCreation -> sprintf "#%s" Urls.charGen

let private pageParser: Parser<Page -> Page, _> =
    oneOf
        [
            map Page.Index top
            map Page.CampaignCreation (s Urls.charGen)
            map Page.CharacterCreation (s Urls.charGen)
        ]

let urlParser location =
  parseHash pageParser location

let parseInt input =
    match input with
    | "" -> Some 0
    | otherValue ->
        match Int32.TryParse otherValue with
        | true, number -> Some number
        | false, _ -> None

let getLast (length: int) (input: string) =
    if input.Length <= length
    then input
    else input.Substring (input.Length - length, length)

let validate validation model =
    model
    |> Option.bind (fun model ->
        if validation model
        then Some model
        else None)