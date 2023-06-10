using Microsoft.AspNetCore.Components;
using Wobalization.Wasm.Providers;

namespace Wobalization.Wasm.Components;

public partial class GuardView : ComponentBase
{
    private bool _isAuthorized = false;

    [Parameter] [EditorRequired] public RenderFragment AuthorizeView { get; set; } = default!;

    [Parameter] public RenderFragment? UnauthorizedView { get; set; }

    [Parameter] public Func<bool>? AuthorizationHandler { get; set; }

    [Parameter] public Func<Task<bool>>? AuthorizationHandlerAsync { get; set; }

    [CascadingParameter] private AuthenticationProvider? _authenticationProvider { get; set; }

    [Inject] private NavigationManager _navigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (!_authenticationProvider!.IsAuthenticated)
        {
            _navigationManager.NavigateTo(_authenticationProvider.LoginPath, replace: true);
        }

        if (AuthorizationHandler != null)
        {
            _isAuthorized = AuthorizationHandler.Invoke();
        }
        else if (AuthorizationHandlerAsync != null)
        {
            _isAuthorized = await AuthorizationHandlerAsync.Invoke();
        }
        else
        {
            _isAuthorized = true;
        }
    }
}