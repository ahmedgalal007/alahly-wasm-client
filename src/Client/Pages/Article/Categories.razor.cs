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

namespace FSH.BlazorWebAssembly.Client.Pages.Article;

public partial class Categories
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
    protected ICategoriesClient CategoriesClient { get; set; } = default!;

    protected EntityServerTableContext<CategoryTableRow, Guid, UpdateCategoryRequest> Context { get; set; } = default!;
    private PaginationResponse<CategoryTableRow> _trails = new();
    // private ICollection<LocalizedKeyword> Locals ;
    // public string CurrentLanguage { get; set; } = "ar-EG";
    // public int CurrentLanguageIndex { get; set; }

    // [Parameter] public Dictionary<string, bool> ActiveTranslations { get; set; } = new Dictionary<string, bool>();
    // public List<LanguageItem> ActiveTranslations { get; set; } = LanguageItem.GetList(null);

    static Categories() =>
        TypeAdapterConfig<CategoryDto, CategoryTableRow>.NewConfig().Map(
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
                new(category => category.Id, L["Id"], "Id"),
                new(category => category.Locals.FirstOrDefault(e => e.IsDefault).Name, L["Category:Name"], Template: FieldTemplate)
            },
        idFunc: category => category.Id ?? Guid.NewGuid(),
        editFormInitializedFunc: OnEditFormInitialized,
        // loadDataFunc: async () => _trails = (await KeywordClient.GetAsync()).Adapt<List<KeywordTableRow>>(),
        searchFunc: async filter => _trails = (await CategoriesClient.SearchAsync(filter.Adapt<SearchCategoriesRequest>()))
        .Adapt<PaginationResponse<CategoryTableRow>>(),
        createFunc: async category => await CategoriesClient.CreateAsync(category.Adapt<CreateCategoryRequest>()),
        updateFunc: async (id, category) => await CategoriesClient.UpdateAsync(id, category),
        deleteFunc: async id => await CategoriesClient.DeleteAsync(id),
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

    void OnAddLanguage(LocalizedCategoryDto item)
    {

    }

    Task OnUpdateLocals(LocalizedCategoryDto item)
    {
        SyncWithContextLocals(Context, item);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task UpdateTextItem(LocalizedCategoryDto item, string value)
    {
        // item.Title = value;
        SyncWithContextLocals(Context, item);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private static void SyncWithContextLocals(EntityTableContext<CategoryTableRow, Guid, UpdateCategoryRequest> Context, LocalizedCategoryDto item)
    {
        AddEditModelLocalizer<CategoryTableRow, LocalizedCategoryDto, Guid, UpdateCategoryRequest>.SyncWithContextLocals(Context, item);
    }

    public class CategoryTableRow : CategoryDto
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