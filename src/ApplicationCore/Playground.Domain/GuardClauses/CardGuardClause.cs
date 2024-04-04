namespace Playground.Domain.GuardClauses;

using Exceptions;

internal static class CardGuardClause
{
    public static DateTime ExpiredCard(this IGuardClause guardClause, DateTime expiration, TimeProvider timeProvider)
    {
        Guard.Against.Null(timeProvider);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        return expiration < now ? throw new CardExpiredException(expiration) : expiration;
    }
}