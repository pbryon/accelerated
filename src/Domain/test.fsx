type Boxes = Boxes of int

type ConsequenceName = ConsequenceName of string

type StressType =
| Physical
| Mental
| General

type StressBox = {
    Type: StressType
    Usable: bool
    Filled: bool
    Total: Boxes
}

type ConsequenceType =
| Mild
| Moderate
| Severe

type Consequence = {
    Type: ConsequenceType
    StressType: StressType
    Name: ConsequenceName
    Stress: Boxes
}

let consequenceBoxes consequence =
    match consequence.Type with
    | Mild _ -> 2
    | Moderate _ -> 4
    | Severe _ -> 6

let consequenceName consequence =
    let toString (ConsequenceName name) = name
    toString consequence.Name

let toString consequence =
    let name = consequenceName consequence
    let value = consequenceBoxes consequence
    sprintf "%s (%d)" name value

let stress = {
    Type = Mild
    StressType = Physical
    Name = (ConsequenceName "Ouch!")
    Stress = (Boxes 2)
}
sprintf "%s" (toString stress)