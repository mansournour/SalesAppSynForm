namespace SalesSyncService.Models
{
    /// <summary>
    /// Types de document BC (miroir de l'enum AL)
    /// </summary>
    public static class BcDocumentType
    {
        public const string Quote       = "Quote";        // Devis
        public const string Order       = "Order";        // Commande
        public const string Invoice     = "Invoice";      // Facture
        public const string CreditMemo  = "Credit Memo";  // Avoir
        public const string BlanketOrder = "Blanket Order";
        public const string ReturnOrder = "Return Order";
    }

    /// <summary>
    /// Payload JSON pour créer un Sales Header dans BC
    /// </summary>
    public class BcSalesHeaderRequest
    {
        public string documentType { get; set; }   // Obligatoire — ex: "Order", "Invoice"
        public string customerNo { get; set; }     // Obligatoire — Code client BC
        public string externalDocumentNo { get; set; }  // Numéro commande local (101, 102…)
        public string postingDate { get; set; }    // Format: "2026-06-25"
        public string currencyCode { get; set; }
        public string salespersonCode { get; set; }
        public string locationCode { get; set; }
    }

    /// <summary>
    /// Payload JSON pour PATCH (mise à jour partielle)
    /// </summary>
    public class BcSalesHeaderPatchRequest
    {
        public string customerNo { get; set; }
        public string externalDocumentNo { get; set; }
        public string postingDate { get; set; }
        public string currencyCode { get; set; }
    }

    /// <summary>
    /// Réponse retournée par l'API BC
    /// </summary>
    public class BcSalesHeaderResponse
    {
        public string id { get; set; }                 // SystemId (GUID)
        public string documentNo { get; set; }         // N° document BC
        public string documentType { get; set; }
        public string customerNo { get; set; }
        public string customerName { get; set; }
        public string postingDate { get; set; }
        public decimal amount { get; set; }
        public decimal amountIncludingVAT { get; set; }
        public string status { get; set; }
        public string externalDocumentNo { get; set; }
        public string lastModifiedDateTime { get; set; }
    }

    /// <summary>
    /// Enveloppe OData retournée par BC pour les listes
    /// </summary>
    public class ODataResponse<T>
    {
        public string odataContext { get; set; }
        public System.Collections.Generic.List<T> value { get; set; }
    }
}
