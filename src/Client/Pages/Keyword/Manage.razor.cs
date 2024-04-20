using System.Security.Claims;
using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Components.Localization;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Authorization;
using FSH.WebApi.Shared.Multitenancy;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Syncfusion.Blazor.DropDowns;
using static FSH.BlazorWebAssembly.Client.Pages.Personal.AuditLogs;

namespace FSH.BlazorWebAssembly.Client.Pages.Keyword;

public partial class Manage
{
    [Parameter]
    public string Id { get; set; } = default!; // from route

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;
    [Inject]
    protected IKeywordClient KeywordClient { get; set; } = default!;

    protected EntityServerTableContext<KeywordTableRow, Guid, UpdateKeywordRequest> Context { get; set; } = default!;
    private PaginationResponse<KeywordTableRow> _trails = new();
    // private ICollection<LocalizedKeyword> Locals ;
    // public string CurrentLanguage { get; set; } = "ar-EG";
    // public int CurrentLanguageIndex { get; set; }

    // [Parameter] public Dictionary<string, bool> ActiveTranslations { get; set; } = new Dictionary<string, bool>();
    // public List<LanguageItem> ActiveTranslations { get; set; } = LanguageItem.GetList(null);

    static Manage() =>
        TypeAdapterConfig<KeywordDto, KeywordTableRow>.NewConfig().Map(
            dest => dest.LocalTime,
            src => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToLocalTime());


    protected override async Task OnParametersSetAsync()
    {
        var preferenceManager = await ClientPreferences.GetPreference() as ClientPreference;

        // CurrentLanguage = preferenceManager?.LanguageCode ?? "ar-EG";
        LanguageManager.SelectLanguge(preferenceManager?.LanguageCode ?? "ar-EG");

        // LanguageManager.ActivateLanguge(preferenceManager?.LanguageCode ?? "ar-EG");
    }

    protected override void OnInitialized() => Context = new(
        entityName: L["Keyword"],
        entityNamePlural: L["Keywords"],
        entityResource: FSHResource.Keywords,
        hasExtraActionsFunc: () => true,
        fields: new()
            {
                new(keyword => keyword.Id, L["Id"], "Id"),
                new(keyword => keyword.IsOrganization, L["Organization"], Template: FieldTemplate)
            },
        idFunc: keyword => keyword.Id ?? Guid.NewGuid(),
        editFormInitializedFunc: OnEditFormInitialized,
        // loadDataFunc: async () => _trails = (await KeywordClient.GetAsync()).Adapt<List<KeywordTableRow>>(),
        searchFunc: async filter => _trails = (await KeywordClient.SearchAsync(filter.Adapt<SearchKeywordRequest>()))
        .Adapt<PaginationResponse<KeywordTableRow>>(),
        createFunc: async keyword => await KeywordClient.CreateAsync(keyword.Adapt<CreateKeywordRequest>()),
        updateFunc: async (id, keyword) => await KeywordClient.UpdateAsync(id, keyword),
        deleteFunc: async id => await KeywordClient.DeleteAsync(id),
        exportAction: string.Empty);

    protected async Task OnEditFormInitialized(){}

    private void ForceRender(EventArgs e) { /*Context.AddEditModal.ForceRender();*/ StateHasChanged(); }

    private void ShowBtnPress(Guid? id)
    {
        var trail = _trails.Data.First(f => f.Id == id);
        trail.ShowDetails = !trail.ShowDetails;
        foreach (var otherTrail in _trails.Data.Except(new[] { trail }))
        {
            otherTrail.ShowDetails = false;
        }
    }

    void OnAddLanguage(LocalizedKeywordDto item)
    {

    }

    Task OnUpdateLocals(LocalizedKeywordDto item)
    {
        SyncWithContextLocals(Context, item);
        return Task.CompletedTask;
    }

    private Task UpdateTextItem(LocalizedKeywordDto item, string value)
    {
        // item.Title = value;
        SyncWithContextLocals(Context, item);
        return Task.CompletedTask;
    }

    private static void SyncWithContextLocals(EntityTableContext<KeywordTableRow, Guid, UpdateKeywordRequest> Context, LocalizedKeywordDto item)
    {
        AddEditModelLocalizer<KeywordTableRow, LocalizedKeywordDto, Guid, UpdateKeywordRequest>.SyncWithContextLocals(Context, item);
    }

    public class KeywordTableRow : KeywordDto
    {
        public bool ShowDetails { get; set; }
        public DateTime LocalTime { get; set; }
    }
}

// [JSInvokable]
// private void ToggleLanguage(ChangeEventArgs e)
// {
// StateHasChanged();
// }