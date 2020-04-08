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
    | CharacterCreation

let toHash page =
  match page with
  | Page.Index -> "#"
  | Page.CharacterCreation -> "#characters"

let private pageParser: Parser<Page -> Page, _> =
    oneOf
        [
            map Page.Index top
            map Page.CharacterCreation (s "characters")
        ]

let urlParser location =
  parseHash pageParser location