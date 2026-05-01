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

TBA

## Building

TBA
