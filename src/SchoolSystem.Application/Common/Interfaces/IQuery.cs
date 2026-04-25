using MediatR;

namespace SchoolSystem.Application.Common.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }
