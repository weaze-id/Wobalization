using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.Authentication;
using Wobalization.Wasm.Providers;
using Wobalization.Wasm.Services;

namespace Wobalization.Wasm.Pages.Authentication;

public partial class SignInPage : ComponentBase
{
    private readonly InSignInDto _dto = new();
    private MudForm _form = new();

    [CascadingParameter] private AuthenticationProvider _authenticationProvider { get; set; } = default!;

    [Inject] private NavigationManager _navigationManager { get; set; } = default!;

    [Inject] private AuthenticationService _authenticationService { get; set; } = default!;

    [Inject] private IValidator<InSignInDto> _validator { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_authenticationProvider.IsAuthenticated)
        {
            _navigationManager.NavigateTo("/", replace: true);
        }

        var appStatus = await _authenticationService.CheckStatusAsync();
        if (!appStatus)
        {
            _navigationManager.NavigateTo("/authentication/sign-up");
        }
    }

    private async Task OnSubmittedAsync()
    {
        await _authenticationProvider.SignInAsync(_dto, "/");
    }
}