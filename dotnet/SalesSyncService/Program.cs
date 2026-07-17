using System;
using System.Threading.Tasks;
using SalesSyncService.Config;
using SalesSyncService.Models;
using SalesSyncService.Services;

namespace SalesSyncService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // ── 1. Configuration ─────────────────────────────────────────
            var config = new BcApiConfig
            {
                TenantId         = "VOTRE_TENANT_ID",       // Azure AD Tenant
                Environment      = "production",             // ou "sandbox"
                CompanyId        = "VOTRE_COMPANY_ID",       // GUID société BC
                Username         = "VOTRE_USERNAME",
                WebServiceKey    = "VOTRE_WEB_SERVICE_KEY",
                OracleConnectionString =
                    "User Id=VOTRE_USER;Password=VOTRE_MDP;Data Source=localhost:1521/XEPDB1;"
            };

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║   SalesSyncService — Oracle → BC Cloud       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Choisir une action :");
                Console.WriteLine("  1. Synchroniser tous les clients Oracle → BC");
                Console.WriteLine("  2. Lister les Sales Headers BC par type");
                Console.WriteLine("  3. Créer un Sales Header (Commande)");
                Console.WriteLine("  4. Patcher un Sales Header");
                Console.WriteLine("  5. Supprimer un Sales Header");
                Console.WriteLine("  0. Quitter");
                Console.Write("Choix : ");

                switch (Console.ReadLine()?.Trim())
                {
                    // ── Sync clients ──────────────────────────────────────
                    case "1":
                        var syncSvc = new CustomerSyncService(config);
                        await syncSvc.SyncAllCustomersAsync();
                        break;

                    // ── Lister par type ───────────────────────────────────
                    case "2":
                        Console.Write("Type (Quote/Order/Invoice/Credit Memo) : ");
                        var docType = Console.ReadLine()?.Trim() ?? BcDocumentType.Order;

                        var salesSvc = new SalesHeaderService(config);
                        var docs = await salesSvc.GetByDocumentTypeAsync(docType);

                        Console.WriteLine($"\n── {docs.Count} documents de type '{docType}' ──");
                        foreach (var d in docs)
                            Console.WriteLine(
                                $"  N°{d.documentNo,-12} Client:{d.customerNo,-10} " +
                                $"Montant:{d.amount,10:F2}  Statut:{d.status}");
                        Console.WriteLine();
                        break;

                    // ── Créer une commande ────────────────────────────────
                    case "3":
                        Console.Write("Code client BC : ");
                        var custNo = Console.ReadLine()?.Trim();
                        Console.Write("N° doc externe (ex: 101) : ");
                        var extNo = Console.ReadLine()?.Trim();

                        var created = await new SalesHeaderService(config).CreateAsync(
                            new BcSalesHeaderRequest
                            {
                                documentType      = BcDocumentType.Order,
                                customerNo        = custNo,
                                externalDocumentNo = extNo,
                                postingDate       = DateTime.Today.ToString("yyyy-MM-dd")
                            });
                        Console.WriteLine($"  ✓ Créé : {created.documentNo} (id={created.id})\n");
                        break;

                    // ── Patcher ───────────────────────────────────────────
                    case "4":
                        Console.Write("SystemId (GUID) du document : ");
                        var patchId = Console.ReadLine()?.Trim();
                        Console.Write("Nouveau code client : ");
                        var newCust = Console.ReadLine()?.Trim();

                        var patched = await new SalesHeaderService(config).PatchAsync(
                            patchId,
                            new BcSalesHeaderPatchRequest { customerNo = newCust });
                        Console.WriteLine($"  ✓ Patché : {patched.documentNo}\n");
                        break;

                    // ── Supprimer ─────────────────────────────────────────
                    case "5":
                        Console.Write("SystemId (GUID) du document à supprimer : ");
                        var delId = Console.ReadLine()?.Trim();
                        await new SalesHeaderService(config).DeleteAsync(delId);
                        Console.WriteLine("  ✓ Supprimé\n");
                        break;

                    case "0":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("  Choix invalide.\n");
                        break;
                }
            }
        }
    }
}
