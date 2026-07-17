using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SalesApp.BusinessLayer
{
    /// <summary>
    /// Couche métier : accès SQL Server local + appel API BC Cloud
    /// </summary>
    public class SalesHeaderBC
    {
        // ── Connexion SQL Server locale ──────────────────────────────────
        // Adapter : remplacer SERVER, DATABASE par tes vraies valeurs
        // Exemple avec auth Windows : "Server=.\SQLEXPRESS;Database=SalesDB;Integrated Security=True;"
        // Exemple avec auth SQL     : "Server=localhost;Database=SalesDB;User Id=sa;Password=xxx;"
        private readonly string _sqlConnStr =
            "Server=.\\SQLEXPRESS;Database=SalesDB;Integrated Security=True;";

        // ── Service BC API ───────────────────────────────────────────────
        private readonly BcSyncService _bcService;

        public SalesHeaderBC()
        {
            _bcService = new BcSyncService();
        }

        // ────────────────────────────────────────────────────────────────
        // Lecture SQL Server local — affichage dans DataGridView
        // ────────────────────────────────────────────────────────────────
        public DataTable GetAllCommandes()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_sqlConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT NumCommande, NumClient, DateCommande FROM SalesHeader ORDER BY NumCommande",
                conn);

            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetAllClients()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_sqlConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT id, nom, prenom, telephone, email FROM Clients ORDER BY id",
                conn);

            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        // ────────────────────────────────────────────────────────────────
        // Lecture SQL Server → retourne liste typée pour sync BC
        // ────────────────────────────────────────────────────────────────
        private List<LocalClient> GetClientsLocaux()
        {
            var list = new List<LocalClient>();
            using var conn = new SqlConnection(_sqlConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT id, nom, prenom, telephone, email FROM Clients",
                conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new LocalClient
                {
                    Id        = reader.GetInt32(0),
                    Nom       = reader.GetString(1),
                    Prenom    = reader.GetString(2),
                    Telephone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Email     = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
            return list;
        }

        private List<LocalCommande> GetCommandesLocales()
        {
            var list = new List<LocalCommande>();
            using var conn = new SqlConnection(_sqlConnStr);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT NumCommande, NumClient, DateCommande FROM SalesHeader",
                conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new LocalCommande
                {
                    NumCommande  = reader.GetInt32(0),
                    NumClient    = reader.GetInt32(1),
                    DateCommande = reader.GetDateTime(2)
                });
            }
            return list;
        }

        // ────────────────────────────────────────────────────────────────
        // PUSH vers BC Cloud — méthode appelée depuis le bouton Form1
        // ────────────────────────────────────────────────────────────────
        public SyncResult PushClientsBcCloud()
        {
            var clients = GetClientsLocaux();
            return _bcService.SyncClients(clients);
        }

        public SyncResult PushCommandesBcCloud()
        {
            var commandes = GetCommandesLocales();
            return _bcService.SyncCommandes(commandes);
        }
    }

    // ── Modèles locaux (internes à la couche métier) ─────────────────────
    internal class LocalClient
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }

    internal class LocalCommande
    {
        public int NumCommande { get; set; }
        public int NumClient { get; set; }
        public DateTime DateCommande { get; set; }
    }

    // ── Résultat de synchronisation ──────────────────────────────────────
    public class SyncResult
    {
        public int Crees { get; set; }
        public int MisAJour { get; set; }
        public int Erreurs { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

        public override string ToString() =>
            $"✓ Créés: {Crees} | Mis à jour: {MisAJour} | Erreurs: {Erreurs}";
    }
}
