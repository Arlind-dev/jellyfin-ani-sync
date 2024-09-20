<h1>Ani-Sync Jellyfin Plugin</h1>

## About

Ani-Sync lets you synchorinze your Jellyfin Anime watch progress to popular services. Please [create a discussion](https://github.com/vosmiic/jellyfin-ani-sync/discussions/new/choose) for new feature ideas.

**This is a fork that changes the way it handles AniDB IDs on a Season level, so it is more comapatible with Shokofin.**

## Build

1. To build this plugin you will need [.Net 8.x](https://dotnet.microsoft.com/download/dotnet/8.0).

2. Build plugin with following command

```
dotnet publish --configuration Release --output bin
```

3. Place the dll-file in the `plugins/ani-sync` folder (you might need to create the folders) of your JF install

## Services/providers

### Currently supported

1. MyAnimeList
2. AniList
3. (Beta) Kitsu
4. (Limited support) Annict
5. Shikimori
6. Simkl

## External tools

### Anime Lists

We use the XML documents in the [anime lists repo](https://github.com/Anime-Lists/anime-lists) to find the anime you are watching on each provider we support.

Please help the project by contributing to the lists of anime, it helps everyone!

### Anime Offline Database/arm server

We use the API offered by the [arm server repo](https://github.com/BeeeQueue/arm-server) which accesses the [anime offline database repo](https://github.com/manami-project/anime-offline-database) that we use to fetch our providers IDs so we can update your progress.

Please also help these projects by contributing to the anime database/helping with the API server.

## Development

Beta releases can be installed automatically by replacing the manifest URL with `https://raw.githubusercontent.com/vosmiic/jellyfin-ani-sync/master/beta-manifest.json`. This will replace your version with the latest beta release (if there is a beta release more recent than the latest stable release). It will be replaced with the latest stable release when it is released, so both repos can be used at the same time. Authentication and other settings should be carried over. Use at your own risk.

Unit tests can be found [here](https://github.com/vosmiic/jellyfin-ani-sync-unit-tests).

The `docker.sh` file can be used to build and automatically copy the resulting DLL to the correct place. The first argument should be either the path to the Ani-Sync folder or the plugins folder (where it will attempt to find the latest Ani-Sync plugin folder). The second optional argument can be an image, or left empty to build using the Dockerfile.
