module Global

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
    | Characters

let toAnchor page =
  match page with
  | Page.Index -> "#"
  | Page.Characters -> "#characters"