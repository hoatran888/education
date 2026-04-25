using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SchoolSystem.Web.Services;

namespace SchoolSystem.Web.Shared;

/// <summary>
/// Base class for all pages. Populates CircuitUserService before any data loading,
/// ensuring ICurrentSchoolContext is ready when MediatR handlers resolve IUnitOfWork.
/// </summary>
public abstract class SchoolComponentBase : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private CircuitUserService          UserService        { get; set; } = default!;

    protected sealed override async Task OnInitializedAsync()
    {
        var state = await AuthStateProvider.GetAuthenticationStateAsync();
        UserService.SetUser(state.User);
        await InitializeAsync();
    }

    protected virtual Task InitializeAsync() => Task.CompletedTask;
}
