using Microsoft.AspNetCore.Components;
using Shared.Dtos.Authentication;
using Wobalization.Wasm.Services;

namespace Wobalization.Wasm.Providers;

public partial class AuthenticationProvider : ComponentBase
{
    private LoaderState _loaderState = LoaderState.Loading;

    [Parameter] [EditorRequired] public string LoginPath { get; set; } = default!;

    [Parameter] [EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    [Inject] private NavigationManager _navigationManager { get; set; } = default!;

    [Inject] private AuthenticationService _authenticationService { get; set; } = default!;

    public bool IsAuthenticated { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsAuthenticated = await _authenticationService.CheckAuthenticationAsync();
        _loaderState = LoaderState.None;
    }

    public async Task SignInAsync(InSignInDto dto, string mainPath)
    {
        var result = await _authenticationService.SignInAsync(dto);
        if (result)
        {
            IsAuthenticated = true;
            _navigationManager.NavigateTo(mainPath, replace: true);
        }
    }

    public async Task SignUpAsync(InSignUpDto dto)
    {
        var result = await _authenticationService.SignUpAsync(dto);
        if (result)
        {
            _navigationManager.NavigateTo(LoginPath, replace: true);
        }
    }

    public async Task SignOutAsync()
    {
        var result = await _authenticationService.SignOutAsync();
        if (result)
        {
            _navigationManager.NavigateTo(LoginPath, replace: true);
        }
    }
}