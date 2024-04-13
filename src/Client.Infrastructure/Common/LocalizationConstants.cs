namespace FSH.BlazorWebAssembly.Client.Infrastructure.Common;

public record LanguageCode(string Code, string DisplayName, int Order=0, bool IsRTL = false);

public static class LocalizationConstants
{
    public static readonly LanguageCode[] SupportedLanguages =
    {
        new("ar-EG", "العربية-(مصر)", 1, true),
        new("en-US", "English", 2),
        new("fr-FR", "French", 3),
        //new("km_KH", "Khmer"),
        //new("de-DE", "German"),
        //new("nl-NL", "Dutch - Netherlands"),
        //new("es-ES", "Spanish"),
        //new("ru-RU", "Russian"),
        //new("sv-SE", "Swedish"),
        //new("id-ID", "Indonesia"),
        //new("it-IT", "Italian"),
        //new("pt-BR", "Portugues")
    };
}