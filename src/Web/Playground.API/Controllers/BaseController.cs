﻿namespace Playground.API.Controllers
{
    using Application.Common;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public abstract class BaseController : ControllerBase
    {
        private IMediator GetMediator() => HttpContext.Features.Get<IMediator>();

        protected Task<TResponse> QueryAsync<TResponse>(IRequest<TResponse> request) => GetMediator().Send(request, HttpContext.RequestAborted);

        protected Task<TResponse> CommandAsync<TResponse>(IRequest<TResponse> request) => GetMediator().Send(request, CancellationToken.None);
    }
}