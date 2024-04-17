using FSH.BlazorWebAssembly.Client.Infrastructure.Common;

namespace FSH.BlazorWebAssembly.Client.Components.Localization;


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
