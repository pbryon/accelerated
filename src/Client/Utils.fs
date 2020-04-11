module Utils

let findItem list item =
    list
    |> List.tryFind (fun x -> x = item)

let findLike list (comparer: 'a -> 'a -> bool) item =
    list
    |> List.tryFind (comparer item)

let contains list item =
    findItem list item
    |> Option.isSome

let containsLike list (comparer: 'a -> 'a -> bool) item =
    findLike list comparer item
    |> Option.isSome