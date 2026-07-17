using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesSyncService.Config;
using SalesSyncService.Models;

namespace SalesSyncService.Services
{
    /// <summary>
    /// Service CRUD complet sur les Sales Headers BC avec filtre DocumentType
    /// </summary>
    public class SalesHeaderService
    {
        private readonly BcApiClient _bcClient;
        private const string Endpoint = "salesDocuments";

        public SalesHeaderService(BcApiConfig config)
        {
            _bcClient = new BcApiClient(config);
        }

        // ────────────────────────────────────────────────────────────────
        // GET — lire tous les documents (sans filtre)
        // ────────────────────────────────────────────────────────────────
        public async Task<List<BcSalesHeaderResponse>> GetAllAsync()
        {
            var result = await _bcClient.GetListAsync<BcSalesHeaderResponse>(Endpoint);
            return result.value;
        }

        // ────────────────────────────────────────────────────────────────
        // GET — filtrer par type de document (OData $filter)
        // Ex: GetByDocumentTypeAsync("Order")
        //     GetByDocumentTypeAsync(BcDocumentType.Invoice)
        // ────────────────────────────────────────────────────────────────
        public async Task<List<BcSalesHeaderResponse>> GetByDocumentTypeAsync(string docType)
        {
            // OData filter: documentType eq 'Order'
            var filter = $"documentType eq '{docType}'";
            var result = await _bcClient.GetListAsync<BcSalesHeaderResponse>(Endpoint, filter);
            return result.value;
        }

        // ────────────────────────────────────────────────────────────────
        // GET — filtrer par type ET par client
        // ────────────────────────────────────────────────────────────────
        public async Task<List<BcSalesHeaderResponse>> GetByDocTypeAndCustomerAsync(
            string docType, string customerNo)
        {
            var filter = $"documentType eq '{docType}' and customerNo eq '{customerNo}'";
            var result = await _bcClient.GetListAsync<BcSalesHeaderResponse>(Endpoint, filter);
            return result.value;
        }

        // ────────────────────────────────────────────────────────────────
        // GET — par SystemId
        // ────────────────────────────────────────────────────────────────
        public async Task<BcSalesHeaderResponse> GetByIdAsync(string systemId)
        {
            return await _bcClient.GetByIdAsync<BcSalesHeaderResponse>(Endpoint, systemId);
        }

        // ────────────────────────────────────────────────────────────────
        // POST — créer un document vente
        // ────────────────────────────────────────────────────────────────
        public async Task<BcSalesHeaderResponse> CreateAsync(BcSalesHeaderRequest request)
        {
            if (string.IsNullOrEmpty(request.documentType))
                throw new ArgumentException("documentType est obligatoire.");
            if (string.IsNullOrEmpty(request.customerNo))
                throw new ArgumentException("customerNo est obligatoire.");

            Console.WriteLine($"[POST] Création {request.documentType} pour client {request.customerNo}");
            return await _bcClient.PostAsync<BcSalesHeaderResponse>(Endpoint, request);
        }

        // ────────────────────────────────────────────────────────────────
        // PATCH — mise à jour partielle
        // ────────────────────────────────────────────────────────────────
        public async Task<BcSalesHeaderResponse> PatchAsync(
            string systemId, BcSalesHeaderPatchRequest patch, string etag = "*")
        {
            Console.WriteLine($"[PATCH] Mise à jour document SystemId={systemId}");
            return await _bcClient.PatchAsync<BcSalesHeaderResponse>(Endpoint, systemId, patch, etag);
        }

        // ────────────────────────────────────────────────────────────────
        // DELETE
        // ────────────────────────────────────────────────────────────────
        public async Task DeleteAsync(string systemId, string etag = "*")
        {
            Console.WriteLine($"[DELETE] Suppression document SystemId={systemId}");
            await _bcClient.DeleteAsync(Endpoint, systemId, etag);
        }
    }
}
