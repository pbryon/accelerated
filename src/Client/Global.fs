module Global

open Elmish.UrlParser
open Domain.Campaign

type UserData =
  {
    UserName : string
    CampaignId: CampaignId
    //Token : Server.AuthTypes.JWT // TODO add
  }

[<RequireQualifiedAccess>]
type Page =
    | Index
    | CoreCharacter
    | FAECharacter

let toHash page =
  match page with
  | Page.Index -> "#"
  | Page.CoreCharacter -> "#core-character"
  | Page.FAECharacter -> "#fae-character"

let private pageParser: Parser<Page -> Page, _> =
    oneOf
        [
            map Page.Index top
            map Page.CoreCharacter (s "core-character")
            map Page.FAECharacter (s "fae-character")
        ]

let urlParser location =
  parseHash pageParser location