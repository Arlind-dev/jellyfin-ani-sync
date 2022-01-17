using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Diacritics.Extensions;
using jellyfin_ani_sync.Api;
using jellyfin_ani_sync.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Logging;
using Episode = MediaBrowser.Controller.Entities.TV.Episode;

namespace jellyfin_ani_sync;

public class ServerEntry : IServerEntryPoint {
    private ISessionManager _sessionManager;
    private ILogger<ServerEntry> _logger;
    private MalApiCalls _malApiCalls;

    public ServerEntry(ISessionManager sessionManager, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) {
        _sessionManager = sessionManager;
        _logger = loggerFactory.CreateLogger<ServerEntry>();
        _malApiCalls = new MalApiCalls(httpClientFactory, loggerFactory);
    }

    public Task RunAsync() {
        _sessionManager.PlaybackStopped += PlaybackStopped;
        return Task.CompletedTask;
    }

    public async void PlaybackStopped(object sender, PlaybackStopEventArgs e) {
        var video = e.Item as Video;
        Episode? episode = video as Episode;
        Anime malAnime;
        if (episode != null) {
            // todo add played to completion after debug
            List<Anime> animeList = await _malApiCalls.SearchAnime(episode.SeriesName, new[] { "id", "title", "alternative_titles" });
            foreach (var anime in animeList) {
                if (String.Compare(anime.Title, episode.SeriesName, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0 ||
                    String.Compare(anime.AlternativeTitles.En, episode.SeriesName, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0) {
                    _logger.LogInformation($"Found matching series: {anime.Title}");
                    UserAnimeListData detectedAnime = await GetAnime(anime.Id, Status.Watching);
                    if (detectedAnime == null) {
                        if (Plugin.Instance.PluginConfiguration.PlanToWatchOnly) {
                            // only search for shows that the user plans to watch
                            detectedAnime = await GetAnime(anime.Id, Status.Plan_to_watch);
                            if (detectedAnime != null) {
                                _logger.LogInformation($"Series ({anime.Title}) found on plan to watch list");
                                await UpdateAnimeWatchProgress(detectedAnime, episode.IndexNumber);
                                return;
                            }

                            _logger.LogWarning($"Series ({anime.Title}) found, but not on Plan To Watch list so ignoring");
                            return;
                        } else {
                            // do a general search for the show
                            detectedAnime = await GetAnime(anime.Id);
                            if (detectedAnime.ListStatus.Status == Status.Completed && Plugin.Instance.PluginConfiguration.RewatchCompleted) {
                                // user has already watched the show, and wants the show to be set as re-watching as per config
                            }
                        }
                    } else {
                        _logger.LogInformation($"Series ({anime.Title}) found on watching list");
                        await UpdateAnimeWatchProgress(detectedAnime, episode.IndexNumber);
                        return;
                    }
                }
            }

            _logger.LogWarning("Series not found");
        }
    }

    private async Task<UserAnimeListData> GetAnime(int animeId, Status? status = null) {
        List<UserAnimeListData> watchingAnime = await _malApiCalls.GetUserAnimeList(status, idSearch: animeId);

        foreach (var animeListData in watchingAnime) {
            if (animeListData.Anime.Id == animeId) {
                return animeListData;
            }
        }

        return null;
    }

    private async Task UpdateAnimeWatchProgress(UserAnimeListData detectedAnime, int? indexNumber) {
        if (indexNumber != null && detectedAnime.ListStatus.NumEpisodesWatched < indexNumber.Value) {
            if (indexNumber.Value == detectedAnime.Anime.NumEpisodes) {
                // user has reached the number of episodes in the show, set as completed
                var response = await _malApiCalls.UpdateAnimeStatus(detectedAnime.Anime.Id, indexNumber.Value, Status.Completed);
                _logger.LogInformation($"Series ({detectedAnime.Anime.Title}) complete, marking show as complete in MAL");
                if (detectedAnime.ListStatus.IsRewatching) {
                    // also increase number of times re-watched by 1
                    // only way to get the number of times re-watched is by doing the update and capturing the response, and then re-updating :/
                    _logger.LogInformation($"Series ({detectedAnime.Anime.Title}) has also been re-watched, increasing re-watch count by 1");
                    await _malApiCalls.UpdateAnimeStatus(detectedAnime.Anime.Id, indexNumber.Value, Status.Completed, numberOfTimesRewatched: response.NumTimesRewatched + 1);
                }
            } else {
                if (detectedAnime.ListStatus.IsRewatching) {
                    // MAL likes to mark re-watching shows as completed, instead of watching. I guess technically both are correct
                    await _malApiCalls.UpdateAnimeStatus(detectedAnime.Anime.Id, indexNumber.Value, Status.Completed);
                } else {
                    await _malApiCalls.UpdateAnimeStatus(detectedAnime.Anime.Id, indexNumber.Value, Status.Watching);
                }
            }

            _logger.LogInformation($"Updated series ({detectedAnime.Anime.Title}) progress to {indexNumber.Value}");
        } else {
            _logger.LogInformation("MAL reports episode already watched; not updating");
        }
    }

    public void Dispose() {
        _sessionManager.PlaybackStopped -= PlaybackStopped;
        GC.SuppressFinalize(this);
    }
}