namespace SalesSyncService.Models
{
    /// <summary>
    /// Représente un client côté Oracle local (SQL Developer)
    /// </summary>
    public class LocalCustomer
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Payload JSON envoyé à l'API BC pour créer/modifier un client
    /// </summary>
    public class BcCustomerRequest
    {
        public string customerNo { get; set; }   // Obligatoire
        public string name { get; set; }          // Obligatoire
        public string phoneNo { get; set; }
        public string email { get; set; }
    }

    /// <summary>
    /// Réponse retournée par l'API BC après création/lecture client
    /// </summary>
    public class BcCustomerResponse
    {
        public string id { get; set; }            // SystemId (GUID)
        public string customerNo { get; set; }
        public string name { get; set; }
        public string phoneNo { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public decimal balance { get; set; }
        public string lastModifiedDateTime { get; set; }
    }
}
