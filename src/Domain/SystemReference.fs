module Domain.SystemReference

[<RequireQualifiedAccess>]
type Topic =
    | FateCore
    | FateAccelerated
    | Approaches
    | Aspects
    | Refresh
    | Skills
    | StuntsCore
    | StuntsFae

let srdLink (topic: Topic) =
    match topic with
    | Topic.FateCore ->
        "https://fate-srd.com/fate-core/basics"
    | Topic.FateAccelerated ->
        "https://fate-srd.com/fate-accelerated/get-started"
    | Topic.Approaches ->
        "https://fate-srd.com/fate-accelerated/how-do-stuff-outcomes-actions-and-approaches#choose-your-approach"
    | Topic.Aspects ->
        "https://fate-srd.com/fate-core/aspects-fate-points"
    | Topic.Refresh ->
        "https://fate-srd.com/fate-core/fate-point-economy#refresh"
    | Topic.Skills ->
        "https://fate-srd.com/fate-core/skills-stunts"
    | Topic.StuntsCore ->
        "https://fate-srd.com/fate-core/building-stunts"
    | Topic.StuntsFae ->
        "https://fate-srd.com/fate-accelerated/stunts"