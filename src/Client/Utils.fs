module Utils

let findItem item list =
    list
    |> List.tryFind (fun x -> x = item)

let findBy (comparer: 'a -> 'a -> bool) item list =
    list
    |> List.tryFind (comparer item)

let contains item list =
    findItem item list
    |> Option.isSome

let containsLike (comparer: 'a -> 'a -> bool) item list =
    findBy comparer item list
    |> Option.isSome