using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(
        TRequest                          request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 ct)
    {
        var isCommand = request is ICommand ||
                        typeof(TRequest).GetInterfaces()
                            .Any(i => i.IsGenericType &&
                                      i.GetGenericTypeDefinition() == typeof(ICommand<>));

        var response = await next();

        if (isCommand)
            await _unitOfWork.SaveChangesAsync(ct);

        return response;
    }
}
