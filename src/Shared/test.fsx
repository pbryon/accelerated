type Approach = Approach of string * Rank
type Skill = Skill of string * Rank



type Stress = {
    Physical: StressCount
    Mental: StressCount
}



let printHealth stress =
    let (Boxes current) = stress.Current;
    let (Boxes total) = stress.Total;
    printfn "%i / %i" current total;

let printStatus stress =
    let dead = stress.Physical.Current = stress.Physical.Total;
    let unconscious = stress.Mental.Current = stress.Mental.Total;
    match (dead, unconscious) with
    | (true, _) -> printfn "Dead"
    | (_, true) -> printf "Unconscious"
    | (_, _) -> printfn "Fine"


open Fate
[<EntryPoint>]
let main args =
    let asNumber = ladderRank Good
    printfn "Rank is %d" asNumber
    let physical = { Current = Boxes 1; Total = Boxes 3}
    printHealth physical
    0;;