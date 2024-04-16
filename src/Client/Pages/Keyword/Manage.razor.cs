using System.Security.Claims;
using FSH.BlazorWebAssembly.Client.Components.EntityTable;
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

    protected override async Task OnParametersSetAsync()
    {
        // var preferenceManager = await ClientPreferences.GetPreference() as ClientPreference;

        // CurrentLanguage = preferenceManager?.LanguageCode ?? "ar-EG";
        // LanguageItem.SetActiveLanguge( ActiveTranslations, preferenceManager?.LanguageCode ?? "ar-EG");

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

    public class KeywordTableRow : KeywordDto
    {
        public bool ShowDetails { get; set; }
        public DateTime LocalTime { get; set; }
    }
    //private void SetActiveLangugesFromLocal()
    //{
    //    if(Context?.AddEditModal?.RequestModel?.Languages != null)
    //    {
    //        foreach (string? item in Context.AddEditModal.RequestModel.Languages.Split(',').ToList())
    //        {
    //            LanguageManager.ActivateLanguge(item);
    //        }
    //    }
    //}

    //private void PopulateLanguges(string currentLang)
    //{

    //    Context.AddEditModal.RequestModel.Languages = currentLang;
    //    if(Context.AddEditModal?.RequestModel?.Locals is null)
    //        Context.AddEditModal.RequestModel.Locals = new List<LocalizedKeyword>();
    //    Context.AddEditModal.RequestModel.Locals?.Add(new LocalizedKeyword { CulturCode = currentLang });
    //    SetActiveLangugesFromLocal();
    //}

    // [JSInvokable]
    // private void ToggleLanguage(ChangeEventArgs e)
    // {
    // StateHasChanged();
    // }
}

public record LanguageItem(string Code, string Name, bool IsRtl = false, bool Active = false, bool Selected = false);
public static class LanguageManager
{
    // private static string _selectedlanguage;
    private static List<LanguageItem>? _languages;
    public static List<LanguageItem> GetList(string? defaultLanguageCode = null)
    {
        if (_languages == null)
        {
            _languages = new List<LanguageItem>();
            foreach (var language in LocalizationConstants.SupportedLanguages.OrderBy(e => e.Order))
            {
                _languages.Add(new LanguageItem(language.Code, language.DisplayName, language.IsRTL, language.Code == defaultLanguageCode));
            }
        }

        return _languages;
    }

    public static bool ActivateLanguge(string code)
    {
        bool result = false;
        if (_languages != null && _languages.Any(e => e.Code == code))
        {
            for (int i = 0; i < _languages.Count; i++)
            {
                if (_languages[i].Code == code && _languages[i].Active == false)
                {
                    _languages[i] = new LanguageItem(_languages[i].Code, _languages[i].Name, true, _languages[i].Selected);
                    result = true;
                }
            }
        }

        return result;
    }

    public static bool DeActivateLanguge(string code)
    {
        bool result = false;
        if (_languages != null && _languages.Any(e => e.Code == code))
        {
            for (int i = 0; i < _languages.Count; i++)
            {
                if (_languages[i].Code == code && _languages[i].Active == true)
                {
                    _languages[i] = new LanguageItem(_languages[i].Code, _languages[i].Name, false, _languages[i].Selected);
                    result = true;
                }
            }
        }
        return result;
    }

    public static int SelectLanguge(string code)
    {
        int result = -1;
        if (_languages.Any(e => e.Code == code))
        {
            for (int i = 0; i < _languages.Count; i++)
            {
                if (_languages[i].Code == code)
                {
                    if (!_languages[i].Selected)
                        _languages[i] = new LanguageItem(_languages[i].Code, _languages[i].Name, _languages[i].Active, true);

                    result = i;
                }
                else
                {
                    if (_languages[i].Selected)
                    {
                        _languages[i] = new LanguageItem(_languages[i].Code, _languages[i].Name, _languages[i].Active, false);
                    }
                }
            }
        }

        return result;
    }

    public static LanguageItem? getSelectedLanguge()
    {
        return _languages.Any(e => e.Selected) ? _languages.First(e => e.Selected) : null;
    }
}
