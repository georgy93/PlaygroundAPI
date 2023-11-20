namespace Playground.Domain.GuardClauses;

using Exceptions;
using Services;

internal static class CardGuardClause
{
    public static DateTime ExpiredCard(this IGuardClause guardClause, DateTime expiration, IDateTimeService dateTimeService)
    {
        Guard.Against.Null(dateTimeService);

        return expiration < dateTimeService.Now ? throw new CardExpiredException(expiration) : expiration;
    }
}