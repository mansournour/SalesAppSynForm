using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalesApp.BusinessLayer
{
    /// <summary>
    /// Service qui pousse les données SQL Server locales vers BC Cloud via API
    /// Remplacer les constantes BC_* par les vraies valeurs de l'entreprise
    /// </summary>
    public class BcSyncService
    {
        // ── Paramètres BC Cloud — À REMPLIR ─────────────────────────────
        private const string BC_TENANT_ID      = "VOTRE_TENANT_ID";
        private const string BC_ENVIRONMENT    = "production";          // ou "sandbox"
        private const string BC_COMPANY_ID     = "VOTRE_COMPANY_ID";   // GUID société
        private const string BC_USERNAME       = "VOTRE_USERNAME";
        private const string BC_WEB_KEY        = "VOTRE_WEB_SERVICE_KEY";
        private const string BC_API_PUBLISHER  = "Nour";
        private const string BC_API_GROUP      = "salesSync";
        private const string BC_API_VERSION    = "v1.0";
        // ─────────────────────────────────────────────────────────────────

        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented               = false
        };

        private string BaseUrl =>
            $"https://api.businesscentral.dynamics.com/v2.0/{BC_TENANT_ID}/{BC_ENVIRONMENT}" +
            $"/api/{BC_API_PUBLISHER}/{BC_API_GROUP}/{BC_API_VERSION}" +
            $"/companies({BC_COMPANY_ID})";

        private HttpClient BuildClient()
        {
            var client = new HttpClient();
            var cred   = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{BC_USERNAME}:{BC_WEB_KEY}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", cred);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // ────────────────────────────────────────────────────────────────
        // SYNC CLIENTS : Oracle → BC
        // ────────────────────────────────────────────────────────────────
        public SyncResult SyncClienwhts(List<LocalClient> clients)
        {
            var result = new SyncResult();
            using var http = BuildClient();

            // 1. Récupérer tous les clients BC existants
            var bcClients = GetBcCustomers(http);

            foreach (var local in clients)
            {
                try
                {
                    var custNo = local.Id.ToString();
                    var payload = new
                    {
                        customerNo = custNo,
                        name       = $"{local.Nom} {local.Prenom}".Trim(),
                        phoneNo    = local.Telephone,
                        email      = local.Email
                    };

                    if (bcClients.TryGetValue(custNo, out var existingId))
                    {
                        // PATCH — client existe déjà dans BC
                        PatchBc(http, $"customers({existingId})", new
                        {
                            name    = payload.name,
                            phoneNo = payload.phoneNo,
                            email   = payload.email
                        });
                        result.MisAJour++;
                        result.Messages.Add($"PATCH client {custNo}");
                    }
                    else
                    {
                        // POST — nouveau client
                        PostBc(http, "customers", payload);
                        result.Crees++;
                        result.Messages.Add($"POST client {custNo}");
                    }
                }
                catch (Exception ex)
                {
                    result.Erreurs++;
                    result.Messages.Add($"ERREUR client {local.Id}: {ex.Message}");
                }
            }
            return result;
        }

        // ────────────────────────────────────────────────────────────────
        // SYNC COMMANDES : Oracle → BC (comme Sales Orders)
        // ────────────────────────────────────────────────────────────────
        public SyncResult SyncCommandes(List<LocalCommande> commandes)
        {
            var result = new SyncResult();
            using var http = BuildClient();

            // Récupérer les docs BC existants (filtrés par type Order)
            var bcDocs = GetBcSalesDocsByType(http, "Order");

            foreach (var cmd in commandes)
            {
                try
                {
                    var extNo = cmd.NumCommande.ToString();

                    var payload = new
                    {
                        documentType        = "Order",
                        customerNo          = cmd.NumClient.ToString(),
                        externalDocumentNo  = extNo,
                        postingDate         = cmd.DateCommande.ToString("yyyy-MM-dd")
                    };

                    if (bcDocs.TryGetValue(extNo, out var existingId))
                    {
                        // PATCH — commande existe (mise à jour date)
                        PatchBc(http, $"salesDocuments({existingId})", new
                        {
                            postingDate = payload.postingDate
                        });
                        result.MisAJour++;
                        result.Messages.Add($"PATCH commande {extNo}");
                    }
                    else
                    {
                        // POST — nouvelle commande
                        PostBc(http, "salesDocuments", payload);
                        result.Crees++;
                        result.Messages.Add($"POST commande {extNo}");
                    }
                }
                catch (Exception ex)
                {
                    result.Erreurs++;
                    result.Messages.Add($"ERREUR commande {cmd.NumCommande}: {ex.Message}");
                }
            }
            return result;
        }

        // ────────────────────────────────────────────────────────────────
        // Helpers HTTP
        // ────────────────────────────────────────────────────────────────

        private void PostBc(HttpClient http, string endpoint, object payload)
        {
            var url     = $"{BaseUrl}/{endpoint}";
            var content = new StringContent(
                JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

            var response = http.PostAsync(url, content).Result;
            EnsureSuccess(response);
        }

        private void PatchBc(HttpClient http, string endpoint, object payload)
        {
            var url = $"{BaseUrl}/{endpoint}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("If-Match", "*");   // BC exige If-Match pour PATCH

            var response = http.SendAsync(request).Result;
            EnsureSuccess(response);
        }

        // Retourne dictionnaire customerNo → SystemId pour tous les clients BC
        private Dictionary<string, string> GetBcCustomers(HttpClient http)
        {
            var url      = $"{BaseUrl}/customers";
            var response = http.GetAsync(url).Result;
            EnsureSuccess(response);

            var json   = response.Content.ReadAsStringAsync().Result;
            var parsed = JsonSerializer.Deserialize<ODataList<BcCustomer>>(json, _json);

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var c in parsed?.value ?? new List<BcCustomer>())
                dict[c.customerNo] = c.id;

            return dict;
        }

        // Retourne dictionnaire externalDocumentNo → SystemId pour un type de doc
        private Dictionary<string, string> GetBcSalesDocsByType(HttpClient http, string docType)
        {
            var filter   = Uri.EscapeDataString($"documentType eq '{docType}'");
            var url      = $"{BaseUrl}/salesDocuments?$filter={filter}";
            var response = http.GetAsync(url).Result;
            EnsureSuccess(response);

            var json   = response.Content.ReadAsStringAsync().Result;
            var parsed = JsonSerializer.Deserialize<ODataList<BcSalesDoc>>(json, _json);

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var d in parsed?.value ?? new List<BcSalesDoc>())
                if (!string.IsNullOrEmpty(d.externalDocumentNo))
                    dict[d.externalDocumentNo] = d.id;

            return dict;
        }

        private static void EnsureSuccess(HttpResponseMessage r)
        {
            if (!r.IsSuccessStatusCode)
            {
                var body = r.Content.ReadAsStringAsync().Result;
                throw new Exception($"BC API {(int)r.StatusCode}: {body}");
            }
        }

        // ── DTOs internes ────────────────────────────────────────────────
        private class ODataList<T>
        {
            public List<T> value { get; set; }
        }
        private class BcCustomer
        {
            public string id { get; set; }
            public string customerNo { get; set; }
        }
        private class BcSalesDoc
        {
            public string id { get; set; }
            public string externalDocumentNo { get; set; }
        }
    }
}
