using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Wobalization.Wasm.Components;

public partial class AlertDialog : ComponentBase
{
    [Parameter] [EditorRequired] public string Message { get; set; } = default!;

    [CascadingParameter] private MudDialogInstance _mudDialog { get; set; } = default!;

    private void Submit()
    {
        _mudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        _mudDialog.Cancel();
    }
}