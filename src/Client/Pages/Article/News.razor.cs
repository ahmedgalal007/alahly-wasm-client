using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using FSH.WebApi.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using System.Net.WebSockets;

namespace FSH.BlazorWebAssembly.Client.Pages.Article;

public partial class News
{
    [Inject]
    protected INewsClient NewsClient { get; set; } = default!;

    [Inject]
    protected IClientPreferenceManager ClientPreferenceManager { get; set; } = default!;

    public string? _currentLanguage { get; private set; }

    [Parameter]
    public FormModel Model { get; set; } = new FormModel();

    // [Inject]
    // protected IClientPreferenceManager ClientPreferences { get; set; } = default!;

    // protected EntityServerTableContext<NewsDto, Guid, UpdateNewsRequest> Context { get; set; } = default!;
    protected EntityServerTableContext<NewsDto, Guid, NewsViewModel> Context { get; set; } = default!;
    private EntityTable<NewsDto, Guid, NewsViewModel> _newsTable = default!;
    protected override void OnInitialized()
    {
        Context = new(
            entityName: L["News"],
            entityNamePlural: L["News"],
            entityResource: FSHResource.News,
            fields: new()
            {
                new(news => news.Id, L["Id"], "Id"),
                new(news => news.Title, L["Title"], "Title"),
                new(news => news.Description, L["Description"], "Description"),
                /* new(news => news.CultureCode, L["CultureCode"], "CultureCode"), */
            },

            enableAdvancedSearch: true,
            idFunc: news => news.Id,

            /*
             // ? Client Context
             // TODO: The client context loadDataFunc
             loadDataFunc: async () =>
             {
                 var result = await NewsClient.SearchAsync(new SearchNewsRequest());
                 return result.Adapt<PaginationResponse<NewsDto>>().Data;
             },

             // TODO: The client context searchFunc
             searchFunc: (searchString, news) =>
             string.IsNullOrWhiteSpace(searchString)
                     || news.Title?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
             */

            // TODO: The server context searchFunc
            searchFunc: async filter =>
            {
                SearchNewsRequest searchNewsRequest = filter.Adapt<SearchNewsRequest>();
                ClientPreference? pref = (await ClientPreferenceManager.GetPreference()) as ClientPreference;
                _currentLanguage = pref == null ? null : pref.LanguageCode;
                searchNewsRequest.CultureCode = _currentLanguage?[..2] ?? "ar";
                return (await NewsClient.SearchAsync(searchNewsRequest)).Adapt<PaginationResponse<NewsDto>>();
            },
            createFunc: async news =>
            {
                if (!string.IsNullOrEmpty(news.ImageInBytes))
                {
                    news.MainImage = new FileUploadRequest() { Data = news.ImageInBytes, Extension = news.ImageExtension ?? string.Empty, Name = $"{news.Title}_{Guid.NewGuid():N}" };
                }
                news.CultureCode = await getCurrentLanguage();
                var req = news.Adapt<CreateNewsRequest>();
                await NewsClient.CreateAsync(req);
                news.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, news) => {
                if (!string.IsNullOrEmpty(news.ImageInBytes))
                {
                    news.DeleteCurrentImage = true;
                    news.MainImage = new FileUploadRequest() { Data = news.ImageInBytes, Extension = news.ImageExtension ?? string.Empty, Name = $"{news.Title}_{Guid.NewGuid():N}" };
                }
                await NewsClient.UpdateAsync(id, news.Adapt<UpdateNewsRequest>());
                news.ImageInBytes = string.Empty;
            },
            deleteFunc: async id => await NewsClient.DeleteAsync(id),
            exportAction: string.Empty);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // ClientPreference? clientPreference = await ClientPreferences.GetPreference() as ClientPreference;
        // CurrentLanguage = clientPreference?.LanguageCode[..2] ?? "ar";

        // await _newsTable.ReloadDataAsync();
    }

    protected virtual async Task<string> getCurrentLanguage()
    {
        if(_currentLanguage == null)
            _currentLanguage = ((ClientPreference) await ClientPreferenceManager.GetPreference()).LanguageCode;
        return _currentLanguage;
    }

    protected virtual async Task SetCurrentLanguage(string currentLanguage, bool changeGlobally = false)
    {
        _currentLanguage = currentLanguage;
        if (changeGlobally)
        {
            ClientPreferenceManager.ChangeLanguageAsync(currentLanguage);
        }
    }
}

public class FormModel
{
    // [Required]
    // [MinLength(20, ErrorMessage = "Please enter at least 20 characters.")]
    public string Body { get; set; } = "Test Html Editor From Page";
}


public class NewsViewModel : UpdateNewsRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}