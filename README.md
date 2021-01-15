# FNAFOnline
The source code of my multiplayer FNAF1 replica I made in 2017. I never finished it because I was constantly tweaking a network library that I made along with this and I got tired of it.

I decided I would try to quickly finish it, which succeeded, but the code is one big pile of mess unfortunately.

## Project structure

- `.`: Unity version 2018.4.28f1 project
- `FNAF Libraries/`: contains FNAF server plugin and shared server/client code.
- `Server/Stx.Net/`: contains the networking library I made along with it. (StxServer contains the server executable, if built)

## Unstable features

There are a couple of features that are implemented, but are too unstable to release.

- Golden freddy attacks: golden freddy is implemented for afton, but is disabled because there is no way to defend against him.
- Achievements and the Prize Corner (see `Scenes/`)

## Contributions 

Contributions are welcome, when requested, I will update the public server and/or create a release.
