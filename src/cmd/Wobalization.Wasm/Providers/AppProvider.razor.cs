using Microsoft.AspNetCore.Components;
using Shared.Dtos.App;
using Shared.Dtos.Key;
using Shared.Dtos.Language;

namespace Wobalization.Wasm.Providers;

public partial class AppProvider : ComponentBase
{
    public List<OutAppDto>? Apps { get; set; }
    public List<OutLanguageDto>? Languages { get; set; }
    public List<OutKeyDto>? Keys { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
}