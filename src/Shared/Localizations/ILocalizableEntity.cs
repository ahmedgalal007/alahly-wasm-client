namespace FSH.WebApi.Shared.Localizations;
public interface ILocalizableEntity //: IEntity<DefaultIdType>
{
    public string CulturCode { get; set; }

    public bool Enabled { get; }
    public bool IsDefault { get; }

    // abstract static ILocalizableEntity Create { get; }

    public abstract static ILocalizableEntity Create(string cultureCode);
}