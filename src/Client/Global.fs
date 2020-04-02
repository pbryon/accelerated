module Global

open Elmish.UrlParser
open Domain.Campaign
open Fable.Core

type UserData =
  {
    UserName : string
    CampaignId: CampaignId
    //Token : Server.AuthTypes.JWT // TODO add
  }

[<RequireQualifiedAccess>]
type Page =
    | Index
    | Characters

let toHash page =
  match page with
  | Page.Index -> "#"
  | Page.Characters -> "#characters"

let private pageParser: Parser<Page -> Page, _> =
    oneOf
        [
            map Page.Index top
            map Page.Characters (s "characters")
        ]

let urlParser location =
  parseHash pageParser location