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
    | Copyright
    | CampaignCreation
    | CharacterCreation

[<RequireQualifiedAccess>]
module Urls =
    let charGen = "character-creation"
    let copyright = "copyright"

let toHash page =
  match page with
  | Page.Index -> "#"
  | Page.Copyright -> sprintf "#%s" Urls.copyright
  | Page.CampaignCreation -> sprintf "#%s" Urls.charGen
  | Page.CharacterCreation -> sprintf "#%s" Urls.charGen

let private pageParser: Parser<Page -> Page, _> =
    oneOf
        [
            map Page.Index top
            map Page.CampaignCreation (s Urls.charGen)
            map Page.CharacterCreation (s Urls.charGen)
            map Page.Copyright (s Urls.copyright)
        ]

let urlParser location =
  parseHash pageParser location