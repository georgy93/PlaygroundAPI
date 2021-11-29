namespace Playground.Application.Common.Exceptions
{
    using Domain.Exceptions;

    public class RecordNotFoundException : BusinessException
    {
        public RecordNotFoundException(long id) : base($"record with Id: {id} not found!")
        { }

        public override int HttpStatusCode { get; }
    }
}