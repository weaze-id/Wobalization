using System.Text.Json.Nodes;
using MudBlazor;
using RestSharp;
using Wobalization.Wasm.Components;

namespace Wobalization.Wasm.Services;

public class HttpMessageService
{
    private readonly IDialogService _dialogService;

    public HttpMessageService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public void Handle(RestResponse response)
    {
        var body = JsonNode.Parse(response.Content!)!;
        var parameters = new DialogParameters();

        if (body["errors"] == null)
        {
            parameters.Add("Message", (string?)body["title"]);
            _dialogService.Show<AlertDialog>("Error", parameters);
            return;
        }

        var error = (string?)body["errors"]!.AsArray().FirstOrDefault()?.AsArray().FirstOrDefault();

        parameters.Add("Message", error ?? "Unknown error");
        _dialogService.Show<AlertDialog>("Error", parameters);
    }
}