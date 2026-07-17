using System;
using System.Windows.Forms;
using SalesApp.BusinessLayer;

namespace SalesApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Chargement initial si nécessaire
        }

        // ── Bouton existant : Charger les commandes depuis SQL Server ────
        private void btnChargerClients_Click(object sender, EventArgs e)
        {
            try
            {
                SalesHeaderBC bc = new SalesHeaderBC();
                dataGridView1.DataSource = bc.GetAllCommandes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        // ── Nouveau bouton : Pousser les clients Oracle → BC Cloud ───────
        private void btnPushClients_Click(object sender, EventArgs e)
        {
            try
            {
                btnPushClients.Enabled = false;
                btnPushClients.Text    = "Synchronisation...";
                this.Cursor            = Cursors.WaitCursor;

                SalesHeaderBC bc = new SalesHeaderBC();
                SyncResult result = bc.PushClientsBcCloud();

                MessageBox.Show(
                    $"Synchronisation clients terminée !\n\n{result}",
                    "BC Cloud — Clients",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du push BC :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnPushClients.Enabled = true;
                btnPushClients.Text    = "Pousser clients → BC Cloud";
                this.Cursor            = Cursors.Default;
            }
        }

        // ── Nouveau bouton : Pousser les commandes Oracle → BC Cloud ─────
        private void btnPushCommandes_Click(object sender, EventArgs e)
        {
            try
            {
                btnPushCommandes.Enabled = false;
                btnPushCommandes.Text    = "Synchronisation...";
                this.Cursor              = Cursors.WaitCursor;

                SalesHeaderBC bc = new SalesHeaderBC();
                SyncResult result = bc.PushCommandesBcCloud();

                MessageBox.Show(
                    $"Synchronisation commandes terminée !\n\n{result}",
                    "BC Cloud — Commandes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du push BC :\n" + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnPushCommandes.Enabled = true;
                btnPushCommandes.Text    = "Pousser commandes → BC Cloud";
                this.Cursor              = Cursors.Default;
            }
        }
    }
}
