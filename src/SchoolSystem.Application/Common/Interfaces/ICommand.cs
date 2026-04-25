using MediatR;

namespace SchoolSystem.Application.Common.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest { }
