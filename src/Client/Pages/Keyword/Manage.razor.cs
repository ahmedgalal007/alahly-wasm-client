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

    protected EntityServerTableContext<KeywordDto, Guid, UpdateKeywordRequest> Context { get; set; } = default!;
    // private ICollection<LocalizedKeyword> Locals ;
    // public string CurrentLanguage { get; set; } = "ar-EG";
    // public int CurrentLanguageIndex { get; set; }

    // [Parameter] public Dictionary<string, bool> ActiveTranslations { get; set; } = new Dictionary<string, bool>();
    // public List<LanguageItem> ActiveTranslations { get; set; } = LanguageItem.GetList(null);

    protected override async Task OnParametersSetAsync()
    {
        var preferenceManager = await ClientPreferences.GetPreference() as ClientPreference;

        // CurrentLanguage = preferenceManager?.LanguageCode ?? "ar-EG";
        // LanguageItem.SetActiveLanguge( ActiveTranslations, preferenceManager?.LanguageCode ?? "ar-EG");

        LanguageManager.ActivateLanguge(preferenceManager?.LanguageCode ?? "ar-EG");
        
    }

    protected override void OnInitialized() => Context = new(
        entityName: L["Keyword"],
        entityNamePlural: L["Keywords"],
        entityResource: FSHResource.Keywords,
        fields: new()
            {
                new(keyword => keyword.Id, L["Id"], "Id"),
                new(keyword => keyword.IsOrganization, L["Organization"], "Organization"), null, new RenderFragment<KeywordDto>(dto => <div></div>)
            },
        idFunc: keyword => keyword.Id ?? Guid.NewGuid(),
        editFormInitializedFunc: OnEditFormInitialized,
        searchFunc: async filter => (await KeywordClient.SearchAsync(filter.Adapt<SearchKeywordRequest>()))
        .Adapt<PaginationResponse<KeywordDto>>(),
        createFunc: async keyword => await KeywordClient.CreateAsync(keyword.Adapt<CreateKeywordRequest>()),
        updateFunc: async (id, keyword) => await KeywordClient.UpdateAsync(id, keyword),
        deleteFunc: async id => await KeywordClient.DeleteAsync(id),
        exportAction: string.Empty);

    protected async Task OnEditFormInitialized()
        {
        //if(Context.AddEditModal.IsCreate || Context.AddEditModal?.RequestModel?.Locals == null)
        //{
        //    // PopulateLanguges(CurrentLanguage);
        //    Context.AddEditModal.RequestModel.Locals = new List<LocalizedKeyword>() { new LocalizedKeyword { CulturCode = LanguageManager.getSelectedLanguge().Code } };

        //    Context.AddEditModal.RequestModel.DefaultCultureCode = LanguageManager.getSelectedLanguge().Code;
        //}
    }

    private void SwitchLang(string lang) {
        // CurrentLanguage = lang;
        // CurrentLanguageIndex = LanguageItem.SelectLanguge(ActiveTranslations, lang);
        StateHasChanged();
    }

    private void ForceRender(EventArgs e) { /*Context.AddEditModal.ForceRender();*/ StateHasChanged(); }

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

public record LanguageItem(string Code, string Name, bool Active = false, bool Selected = false);
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
                _languages.Add(new LanguageItem(language.Code, language.DisplayName, language.Code == defaultLanguageCode));
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
