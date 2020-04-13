module Utils

let first item list =
    list
    |> List.tryFind (fun x -> x = item)

let firstLike (comparer: 'a -> 'a -> bool) item list =
    list
    |> List.tryFind (comparer item)

let firstBy (comparer: 'a -> bool) list =
    list
    |> List.tryFind comparer

let contains item list =
    first item list
    |> Option.isSome

let containsLike (comparer: 'a -> 'a -> bool) item list =
    firstLike comparer item list
    |> Option.isSome

let containsBy (comparer: 'a -> bool) list =
    firstBy comparer list
    |> Option.isSome