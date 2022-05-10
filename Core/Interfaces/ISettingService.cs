namespace Core.Interfaces
{
    public interface ISettingService
    {
        string GetApiApplicationId();

        string GetApiScopeName();

        string GetAuthorityUrl();

        string GetGraphClientId();

        string GetGraphClientSecret();

        string GetGraphTenantId();

        string GetSiteName();
    }
}