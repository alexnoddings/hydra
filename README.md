![](hydra.svg)

# hydra

hydra is a tiny personal utility for redirecting web requests.

## Why

I develop systems with multiple user types.
I use [Firefox Containers](https://addons.mozilla.org/en-US/firefox/addon/multi-account-containers/) to have multiple
site instances open in one window with different accounts logged in.
But since the site is hosted on the same domain for all users,
Firefox Containers can't automatically switch containers for me.

hydra solves this (admittedly very minor) problem by providing redirects from any domain.
Rather than manually opening a Firefox Container and then navigating to a site, hydra lets you
navigate to `{component}.localhost/{path}`, where the `component` instructs Firefox to automatically open your 
relevant container, and hydra discerns where to redirect to based on `component` and `path`.

hydra relies on modern browsers automatically mapping `*.localhost` to `localhost`.

## Config

hydra is configured via a `hydra.json` file in the process's working directory.

See the example [`hydra.json`](src/hydra.json).

You can change what port hydra binds to via `appsettings.json`'s Kestrel settings.

## Building

The app can be published via `dotnet publish --configuration Release --runtime {runtime-identifier}`.

A
[single-file](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview), 
[AoT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/), 
[R2R](https://learn.microsoft.com/en-us/dotnet/core/deploying/ready-to-run),
platform-native 
executable artefact will be published to `bin\Release\net10.0\{runtime}\publish`. 

See the [.NET RID Catalogue](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog) for runtime identifiers.
