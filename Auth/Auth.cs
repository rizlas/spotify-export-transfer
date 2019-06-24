using Spotify.API.NetCore;
using Spotify.API.NetCore.Auth;
using Spotify.API.NetCore.Enums;
using Spotify.API.NetCore.Models;

namespace SpotifyClone
{
    static class Auth
    {
        public static SpotifyWebAPI Spotify { get => _Spotify; }
        private static SpotifyWebAPI _Spotify;
        public static PrivateProfile Profile { get => _Profile; }
        public static bool IsAuthenticated { get => isAuthenticated; }

        private static PrivateProfile _Profile;
        private static string _ClientId;
        private static string _RedirectUri;
        private static int _RedirectPort;
        private static ImplicitGrantAuth _Auth;

        private static bool isAuthenticated;

        public static void Init(string ClientId, string RedirectUri, int RedirectPort)
        {
            _ClientId = ClientId;
            _RedirectUri = RedirectUri;
            _RedirectPort = RedirectPort;

            _Spotify = new SpotifyWebAPI();
            Authentication();
        }

        public static void Authentication()
        {
            _Auth = new ImplicitGrantAuth();
            isAuthenticated = false;

            _Auth.ClientId = _ClientId;
            _Auth.RedirectUri = _RedirectUri;
            _Auth.Scope = Scope.PlaylistReadPrivate | Scope.PlaylistModifyPrivate;

            _Auth.OnResponseReceivedEvent += Auth_OnResponseReceivedEvent;
            _Auth.StartHttpServer(_RedirectPort); // Starts an internal HTTP Server
            _Auth.DoAuth();
        }

        private static void Auth_OnResponseReceivedEvent(Token token, string state)
        {
            if (token.Error == null)
            {
                int ExpiresIn = token.ExpiresIn;
                _Spotify.AccessToken = token.AccessToken;
                _Spotify.TokenType = token.TokenType;
                _Profile = _Spotify.GetPrivateProfile();
                isAuthenticated = true;
                _Auth.StopHttpServer();
            }
        }
    }
}