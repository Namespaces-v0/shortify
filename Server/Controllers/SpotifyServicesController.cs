using shortify.Shared;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using shortify.Server.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;


namespace shortify.Server.Controllers
{


    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SpotifyServicesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private IMemoryCache _cache;
        private IConfiguration _configuration { get; }

        public SpotifyServicesController(IConfiguration configuration, IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _cache = memoryCache;
        }
        [HttpGet]
        public string Authorize()
        {
            var SpotifyServicesConfiguration = _configuration.GetSection("SpotifyServices")
                                .Get<SpotifyServicesConfigurations>();
            var my_client_id = SpotifyServicesConfiguration.ClientId;
            var redirect_uri = SpotifyServicesConfiguration.RedirectUri;
            var scopes = SpotifyServicesConfiguration.Scopes;
            var response_type = "code";
            return SpotifyServicesConfiguration.AuthorizeAPIBaseURI + "/authorize?response_type=" + response_type + "&client_id=" + my_client_id + "&scope=" + scopes + "&redirect_uri=" + redirect_uri + "&state=34fFsedfdere29kd09";
        }
        [HttpGet]
        public async Task<LocalRedirectResult> Callback([FromQuery] string code, [FromQuery] string state)
        {

            string _access_token;
            string _token_type;
            string _scope;
            int _expires_in;
            string _refresh_token;
            var SpotifyServicesConfiguration = _configuration.GetSection("SpotifyServices")
                                .Get<SpotifyServicesConfigurations>();
            var redirect_uri = SpotifyServicesConfiguration.RedirectUri;
            var client_id = SpotifyServicesConfiguration.ClientId;
            var client_secret = SpotifyServicesConfiguration.ClientSecret;
            var request = new HttpRequestMessage(HttpMethod.Post, SpotifyServicesConfiguration.AuthorizeAPIBaseURI + "/api/token");
            var client = _clientFactory.CreateClient();
            var RequestBody = new Dictionary<string, string>();
            RequestBody.Add("grant_type", "authorization_code");
            RequestBody.Add("code", code);
            RequestBody.Add("redirect_uri", redirect_uri);
            request.Content = new FormUrlEncodedContent(RequestBody);
            var Credentials = client_id + ":" + client_secret;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(Credentials);
            var base64EncodedCredentials = System.Convert.ToBase64String(data);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedCredentials);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var _spotifyservicesapitoken = JsonSerializer.Deserialize<SpotifyServicesAPIToken>(content);
                if (!_cache.TryGetValue(SpotifyServicesAPITokenCache.access_token, out _access_token))
                {
                    _access_token = _spotifyservicesapitoken.access_token;
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    _cache.Set(SpotifyServicesAPITokenCache.access_token, _access_token, cacheEntryOptions);
                }
               
                if (!_cache.TryGetValue(SpotifyServicesAPITokenCache.token_type, out _token_type))
                {
                    _token_type = _spotifyservicesapitoken.token_type;
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    _cache.Set(SpotifyServicesAPITokenCache.token_type, _token_type, cacheEntryOptions);
                }
                if (!_cache.TryGetValue(SpotifyServicesAPITokenCache.scope, out _scope))
                {
                    _scope = _spotifyservicesapitoken.scope;
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    _cache.Set(SpotifyServicesAPITokenCache.scope, _scope, cacheEntryOptions);
                }
                if (!_cache.TryGetValue(SpotifyServicesAPITokenCache.expires_in, out _expires_in))
                {
                    _expires_in = _spotifyservicesapitoken.expires_in;
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    _cache.Set(SpotifyServicesAPITokenCache.expires_in, _expires_in, cacheEntryOptions);
                }
                if (!_cache.TryGetValue(SpotifyServicesAPITokenCache.refresh_token, out _refresh_token))
                {
                    _refresh_token = _spotifyservicesapitoken.refresh_token;
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    _cache.Set(SpotifyServicesAPITokenCache.refresh_token, _refresh_token, cacheEntryOptions);
                }
            return LocalRedirect("/MusicPage");
        }
        [HttpGet]
        public async Task<string> Library()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/albums");
            var client = _clientFactory.CreateClient();
            var access_token = _cache.Get(SpotifyServicesAPITokenCache.access_token);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token.ToString());
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
