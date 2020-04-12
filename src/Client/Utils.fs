module Utils

let findItem item list =
    list
    |> List.tryFind (fun x -> x = item)

let findLike (comparer: 'a -> 'a -> bool) item list =
    list
    |> List.tryFind (comparer item)

let findBy (comparer: 'a -> bool) list =
    list
    |> List.tryFind comparer

let contains item list =
    findItem item list
    |> Option.isSome

let containsLike (comparer: 'a -> 'a -> bool) item list =
    findLike comparer item list
    |> Option.isSome

let containsBy (comparer: 'a -> bool) list =
    findBy comparer list
    |> Option.isSome