using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesSyncService.Config;
using SalesSyncService.Models;

namespace SalesSyncService.Services
{
    /// <summary>
    /// Service de synchronisation : lit les clients Oracle locaux
    /// et les pousse vers BC Cloud via l'API Customer
    /// </summary>
    public class CustomerSyncService
    {
        private readonly BcApiClient _bcClient;
        private readonly BcApiConfig _config;
        private const string Endpoint = "customers";

        public CustomerSyncService(BcApiConfig config)
        {
            _config = config;
            _bcClient = new BcApiClient(config);
        }

        // ────────────────────────────────────────────────────────────────
        // LECTURE Oracle local
        // ────────────────────────────────────────────────────────────────
        public List<LocalCustomer> GetLocalCustomers()
        {
            var clients = new List<LocalCustomer>();

            using var conn = new OracleConnection(_config.OracleConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nom, prenom, telephone, email FROM Clients";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                clients.Add(new LocalCustomer
                {
                    Id        = reader.GetInt32(0),
                    Nom       = reader.GetString(1),
                    Prenom    = reader.GetString(2),
                    Telephone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Email     = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            return clients;
        }

        // ────────────────────────────────────────────────────────────────
        // GET — lire tous les clients BC
        // ────────────────────────────────────────────────────────────────
        public async Task<List<BcCustomerResponse>> GetBcCustomersAsync()
        {
            var result = await _bcClient.GetListAsync<BcCustomerResponse>(Endpoint);
            return result.value;
        }

        // ────────────────────────────────────────────────────────────────
        // POST — créer un client dans BC
        // ────────────────────────────────────────────────────────────────
        public async Task<BcCustomerResponse> CreateCustomerAsync(LocalCustomer local)
        {
            var payload = new BcCustomerRequest
            {
                customerNo = local.Id.ToString(),           // N° client = ID Oracle
                name       = $"{local.Nom} {local.Prenom}",
                phoneNo    = local.Telephone,
                email      = local.Email
            };

            Console.WriteLine($"[POST] Création client {payload.customerNo} – {payload.name}");
            return await _bcClient.PostAsync<BcCustomerResponse>(Endpoint, payload);
        }

        // ────────────────────────────────────────────────────────────────
        // PATCH — mettre à jour un client BC existant
        // ────────────────────────────────────────────────────────────────
        public async Task<BcCustomerResponse> UpdateCustomerAsync(string bcSystemId, LocalCustomer local)
        {
            var payload = new BcCustomerRequest
            {
                name    = $"{local.Nom} {local.Prenom}",
                phoneNo = local.Telephone,
                email   = local.Email
            };

            Console.WriteLine($"[PATCH] Mise à jour client {local.Id} (SystemId={bcSystemId})");
            return await _bcClient.PatchAsync<BcCustomerResponse>(Endpoint, bcSystemId, payload);
        }

        // ────────────────────────────────────────────────────────────────
        // DELETE — supprimer un client BC
        // ────────────────────────────────────────────────────────────────
        public async Task DeleteCustomerAsync(string bcSystemId)
        {
            Console.WriteLine($"[DELETE] Suppression client SystemId={bcSystemId}");
            await _bcClient.DeleteAsync(Endpoint, bcSystemId);
        }

        // ────────────────────────────────────────────────────────────────
        // SYNC — push tous les clients locaux vers BC
        //   • Si le client n'existe pas dans BC → POST
        //   • Si le client existe déjà (même customerNo) → PATCH
        // ────────────────────────────────────────────────────────────────
        public async Task SyncAllCustomersAsync()
        {
            Console.WriteLine("=== Démarrage synchronisation clients Oracle → BC ===");

            // 1. Lire clients locaux Oracle
            var localCustomers = GetLocalCustomers();
            Console.WriteLine($"  {localCustomers.Count} clients locaux trouvés.");

            // 2. Lire clients existants dans BC (pour détecter doublons)
            var bcCustomers = await GetBcCustomersAsync();
            var bcByNo = new Dictionary<string, BcCustomerResponse>(StringComparer.OrdinalIgnoreCase);
            foreach (var bc in bcCustomers)
                bcByNo[bc.customerNo] = bc;

            int created = 0, updated = 0, errors = 0;

            foreach (var local in localCustomers)
            {
                try
                {
                    if (bcByNo.TryGetValue(local.Id.ToString(), out var existing))
                    {
                        // Client existe → PATCH
                        await UpdateCustomerAsync(existing.id, local);
                        updated++;
                    }
                    else
                    {
                        // Client nouveau → POST
                        await CreateCustomerAsync(local);
                        created++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [ERREUR] Client {local.Id}: {ex.Message}");
                    errors++;
                }
            }

            Console.WriteLine($"=== Synchronisation terminée : {created} créés, {updated} mis à jour, {errors} erreurs ===");
        }
    }
}
