namespace Playground.Infrastructure.Identity.Models
{
    using System.Collections.Generic;

    public record AuthenticationResult
    {
        private AuthenticationResult(string token, string refreshtoken)
        {
            IsSuccess = true;
            Token = token;
            RefreshToken = refreshtoken;
        }

        private AuthenticationResult(params string[] errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public string Token { get; }

        public string RefreshToken { get; }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<string> Errors { get; }

        public static AuthenticationResult Success(string token, string refreshToken) => new(token, refreshToken);

        public static AuthenticationResult Fail(params string[] errors) => new(errors);

        public static implicit operator bool(AuthenticationResult result) => result.IsSuccess;
    }
}
