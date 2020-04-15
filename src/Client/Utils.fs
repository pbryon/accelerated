module Utils

open System

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

let noneExist (predicate: 'a -> bool) (list: 'a list) =
    list
    |> List.exists predicate
    |> not

let parseInt input =
    match input with
    | "" -> Some 0
    | otherValue ->
        match Int32.TryParse otherValue with
        | true, number -> Some number
        | false, _ -> None

let getLast (length: int) (input: string) =
    if input.Length <= length
    then input
    else input.Substring (input.Length - length, length)

let validate validation model =
    model
    |> Option.bind (fun model ->
        if validation model
        then Some model
        else None)