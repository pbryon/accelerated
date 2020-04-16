module Copyright.View

open Feliz
open Feliz.Bulma

let private ccByLink = "http://creativecommons.org/licenses/by/3.0"

let private fateRpgLink = "http://www.faterpg.com"

let private link href =
    Html.a [
        prop.href href
        prop.text href
        prop.target.blank
    ]

let private ccBy =
    [
        Html.span "This work is based on Fate Core System and Fate Accelerated Edition (found at "
        link fateRpgLink
        Html.span "), products of Evil Hat Productions, LLC, developed, authored, and edited by Leonard Balsera, Brian Engard, Jeremy Keller, Ryan Macklin, Mike Olson, Clark Valentine, Amanda Valentine, Fred Hicks, and Rob Donoghue, and licensed for our use under the Creative Commons Attribution 3.0 Unported license ("
        link ccByLink
        Html.span ")."
    ]

let private poweredByFate =
    "Fate™ is a trademark of Evil Hat Productions, LLC. The Powered by Fate logo is © Evil Hat Productions, LLC and is used with permission."

let private fateCoreFont =
    "The Fate Core font is © Evil Hat Productions, LLC and is used with permission. The Four Actions icons were designed by Jeremy Keller."

let view =
    Bulma.container [
        prop.className "copyright"
        prop.style [ style.textAlign.justify ]
        prop.children [
            Bulma.title3 [
                prop.text "Copyright"
                prop.style [ style.textAlign.center ]
            ]
            Html.p [
                prop.children ccBy
            ]
            Html.p poweredByFate
            Html.p fateCoreFont
        ]
    ]