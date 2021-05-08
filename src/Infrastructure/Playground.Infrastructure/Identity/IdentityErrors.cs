namespace Playground.Infrastructure.Identity
{
    public static class IdentityErrors
    {
        public const string InvalidToken = "Invalid Token";
        public const string UserDoesNotExist = "User does not exist";
        public const string TokenHasExpired = "This refresh token has expired";
        public const string TokenHasntExpiredYet = "This token hasn't expired yet";
        public const string TokenDoesNotExist = "This refresh token does not exist";
        public const string RefreshedTokenIsAlreadyUsed = "This refresh token has been used";
        public const string RefreshTokenIsInvalidated = "This refresh token has been invalidated";
        public const string InvalidUserPasswordCombination = "User/password combination is wrong";
        public const string RefreshTokenDoesNotMatchJWT = "This refresh token does not match this JWT";
        public const string EmailIsAlreadyTakenByAnotherUser = "User with this email addres already exists";
    }
}