namespace Playground.Domain.GuardClauses
{
    using System.Text.RegularExpressions;

    internal static partial class EmailGuardClause
    {
        private const string EmailRegexPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public static string InvalidEmail(this IGuardClause guardClause, string input, string parameterName)
        {
            Guard.Against.NullOrWhiteSpace(input, parameterName, "email is not supplied");

            return EmailRegex().IsMatch(input)
                ? input
                : throw new ArgumentException("Invalid email!", parameterName);
        }

        [GeneratedRegex(EmailRegexPattern, RegexOptions.IgnoreCase)]
        private static partial Regex EmailRegex();
    }
}