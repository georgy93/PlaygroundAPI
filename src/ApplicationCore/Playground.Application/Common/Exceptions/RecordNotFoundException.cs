namespace Playground.Application.Common.Exceptions;

using Domain.Exceptions;

public class RecordNotFoundException(long id) : BusinessException($"record with Id: {id} not found!")
{
    public override int HttpStatusCode { get; }
}