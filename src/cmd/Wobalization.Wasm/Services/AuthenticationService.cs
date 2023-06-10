using RestSharp;
using Shared.Dtos.Authentication;

namespace Wobalization.Wasm.Services;

public class AuthenticationService
{
    private readonly HttpMessageService _httpMessageService;
    private readonly RestClient _restClient;

    public AuthenticationService(RestClient restClient, HttpMessageService httpMessageService)
    {
        _restClient = restClient;
        _httpMessageService = httpMessageService;
    }

    public async Task<bool> CheckStatusAsync()
    {
        var request = new RestRequest("authentication/status");
        var response = await _restClient.ExecuteGetAsync(request);

        return response.IsSuccessful;
    }

    public async Task<bool> CheckAuthenticationAsync()
    {
        var request = new RestRequest("authentication/check");
        var response = await _restClient.ExecuteGetAsync(request);

        return response.IsSuccessful;
    }

    public async Task<bool> SignInAsync(InSignInDto dto)
    {
        var request = new RestRequest("authentication/sign-in");
        request.AddJsonBody(dto);

        var response = await _restClient.ExecutePostAsync(request);
        if (!response.IsSuccessful)
        {
            _httpMessageService.Handle(response);
            return false;
        }

        return true;
    }

    public async Task<bool> SignUpAsync(InSignUpDto dto)
    {
        var request = new RestRequest("authentication/sign-up");
        request.AddJsonBody(dto);

        var response = await _restClient.ExecutePostAsync(request);
        if (!response.IsSuccessful)
        {
            _httpMessageService.Handle(response);
            return false;
        }

        return true;
    }

    public async Task<bool> SignOutAsync()
    {
        var request = new RestRequest("authentication/sign-out");
        var response = await _restClient.ExecutePostAsync(request);

        if (!response.IsSuccessful)
        {
            _httpMessageService.Handle(response);
            return false;
        }

        return true;
    }
}