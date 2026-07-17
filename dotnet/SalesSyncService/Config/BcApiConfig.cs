namespace SalesSyncService.Config
{
    /// <summary>
    /// Configuration de connexion à l'environnement BC Cloud
    /// Remplacer les valeurs par celles de l'entreprise cible
    /// </summary>
    public class BcApiConfig
    {
        // ── Paramètres BC Cloud ──────────────────────────────────────────
        // Format: https://api.businesscentral.dynamics.com/v2.0/{tenantId}/{environment}/api/{publisher}/{group}/{version}/companies({companyId})/
        public string TenantId { get; set; }       = "1b51ba7c-63c6-405a-91ca-5f147adab3f3";
        public string Environment { get; set; }    = "Production";       // Environnement Production
        public string CompanyId { get; set; }      = "VOTRE_COMPANY_ID"; // GUID — copier depuis l'URL BC

        // ── Identifiants API (Basic Auth) ────────────────────────────────
        public string Username { get; set; }       = "VOTRE_USERNAME";
        public string WebServiceKey { get; set; }  = "VOTRE_WEB_SERVICE_KEY";

        // ── Paramètres de l'extension ────────────────────────────────────
        public string ApiPublisher { get; set; }   = "Nour";
        public string ApiGroup { get; set; }       = "salesSync";
        public string ApiVersion { get; set; }     = "v1.0";

        // ── URL de base calculée ─────────────────────────────────────────
        public string BaseUrl =>
            $"https://api.businesscentral.dynamics.com/v2.0/{TenantId}/{Environment}" +
            $"/api/{ApiPublisher}/{ApiGroup}/{ApiVersion}" +
            $"/companies({CompanyId})";

        // ── Connexion Oracle locale ──────────────────────────────────────
        public string OracleConnectionString { get; set; } =
            "User Id=VOTRE_USER;Password=VOTRE_MDP;Data Source=localhost:1521/XEPDB1;";
    }
}
