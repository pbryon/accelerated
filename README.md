# Accelerated

![](https://github.com/pbryon/accelerated/workflows/CI/badge.svg) ![](https://github.com/pbryon/accelerated/workflows/npm/badge.svg)

This is a character generator for roleplaying characters in:

* [Fate Core](https://fate-srd.com/fate-core/basics)
* [Fate Accelerated](https://fate-srd.com/fate-accelerated/get-started)

It was built using F# and the [SAFE stack](https://safe-stack.github.io/docs/) with [Feliz](https://github.com/Zaid-Ajaj/Feliz) and [Feliz.Bulma](https://github.com/Dzoukr/Feliz.Bulma)

## Editing the project

I prefer [VS Code](https://code.visualstudio.com/download) with [Ionide](https://ionide.io/), but your mileage may vary.

If this is your first foray into F#, check out my [F# resources](https://github.com/pbryon/resources/blob/master/topics/F%23.md) or [F# for fun and profit](https://fsharpforfunandprofit.com)

## Getting up and running

### 1. Install dependencies

If you haven't already, install the following dotnet global tools:

```bash
dotnet install -g fake-cli
dotnet install -g paket
# optional:
dotnet install -g femto
```

and install [yarn](https://classic.yarnpkg.com/en/docs/install)

#### Pre-existing `fake`

If running under [WSL](https://docs.microsoft.com/en-us/windows/wsl/install-win10) with Ubuntu 16.04/18.04, you might run into an issue with a pre-existing [`fake`](https://packages.ubuntu.com/bionic/fake) command in `/usr/sbin`. To check for this issue, run:

```bash
sudo ls /usr/sbin/fake
# and if it exists:
sudo rm /usr/sbin/fake
dotnet tool uninstall -g fake-cli
dotnet tool install -g fake-cli
```

### 2. Restore the project

Then restore the project:

```bash
# restore NuGet packages:
paket restore
# alternatively, restore NuGet packages and their npm dependencies:
$ femto restore src/Client/Client.fsproj
```

### 3. Build the project

1. through CLI: `fake build -t Run`
2. from VS Code: run the `Watch Client` target (`ctrl-shift-B`).

To view the project in action, open your browser to [`http://localhost:8080`](http://localhost:8080).

This runs the app in watch mode with hot reloading when any source files change.