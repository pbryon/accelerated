module Global

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

let validate validation model =
    model
    |> Option.bind (fun model ->
        if validation model
        then Some model
        else None)