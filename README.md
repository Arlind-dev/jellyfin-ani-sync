# Ani-Sync Jellyfin Plugin — `shokofix` fork (retired)

> ## ⚠️ This fork is retired. Please switch to upstream.
>
> This fork existed to make Ani-Sync handle AniDB IDs at the *season* level so it
> would work correctly with [Shokofin](https://github.com/ShokoAnime/Shokofin).
> That is now fixed upstream.
>
> [**vosmiic/jellyfin-ani-sync#221**](https://github.com/vosmiic/jellyfin-ani-sync/pull/221)
> was merged on 2026-08-24 and closed
> [issue #119](https://github.com/vosmiic/jellyfin-ani-sync/issues/119) — the exact
> issue this fork was waiting on — along with #128, #135, #165, #181, #215 and #219.
>
> Upstream's implementation is a **superset** of this fork's and is covered by unit
> tests. It additionally handles Shokofin *merged* seasons, applies anime-list
> mapping offsets, and looks provider IDs up case-insensitively (`AniDB` as well as
> `Anidb`), none of which this fork did.

## What to install instead

The fix is in upstream's **beta** channel (v4.5b and newer). Upstream stable v4.4
predates the fix, so use the beta manifest until the next stable release:

```
https://raw.githubusercontent.com/vosmiic/jellyfin-ani-sync/master/beta-manifest.json
```

Once a stable release newer than v4.4 is out, the stable manifest is enough:

```
https://raw.githubusercontent.com/vosmiic/jellyfin-ani-sync/master/manifest.json
```

### Migrating off this fork

1. Add the upstream manifest URL above in *Dashboard → Plugins → Repositories*.
2. Remove the `shokofix` repository URL.
3. Uninstall the fork's Ani-Sync plugin, then install Ani-Sync from upstream.
4. Restart Jellyfin.

Authentication and settings use the same plugin GUID and configuration file, so
they carry over. Taking a backup of your plugin configuration first is still wise.

## Status of this branch

`fork-manifest.json` is frozen at **4.4.0.1**; the release workflow's push trigger
is disabled, so no further fork builds will be published. The branch is kept only
so existing installs can still resolve their repository URL and find this notice.

This branch is now upstream `master` plus this notice — it carries no functional
changes of its own.

## History

The original fork work was taken from [Terrails/jellyfin-ani-sync](https://github.com/Terrails/jellyfin-ani-sync)
and wrapped in an automated release pipeline so it could be installed through
Jellyfin's plugin repository UI.

Jellyfin versions required by the fork's published releases, for reference:

| Plugin Version    | Minimum Required Jellyfin Version |
|-------------------|-----------------------------------|
| 3.5.0.\*          | 10.9.11.0                         |
| 3.6.0.\*          | 10.10.1.0                         |
| 3.7.0.1 - 3.7.0.2 | 10.10.3.0                         |
| 3.7.0.3 - 3.7.0.4 | 10.10.7.0                         |
| 3.8.0.\*          | 10.11.0.0                         |
| 3.9.0.\*          | 10.11.4.0                         |
| 4.0.0.\*          | 10.11.6.0                         |
| 4.1.0.\*          | 10.11.6.0                         |
| 4.2.0.\*          | 10.11.8.0                         |
| 4.3.0.\*          | 10.11.8.0                         |
| 4.4.0.1           | 10.11.11.0                        |

For everything about the plugin itself, see the
[upstream README](https://github.com/vosmiic/jellyfin-ani-sync).
