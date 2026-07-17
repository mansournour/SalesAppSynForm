# Intégration dans ton projet SalesApp (Visual Studio)

## Fichiers à copier dans ton projet Visual Studio

### 1. Remplacer / Mettre à jour BusinessLayer\SalesHeaderBC.cs
→ Copier le contenu de `SalesHeaderBC.cs` (ajoute les méthodes PushClientsBcCloud et PushCommandesBcCloud)

### 2. Ajouter un nouveau fichier BusinessLayer\BcSyncService.cs
→ Copier `BcSyncService.cs` dans ton dossier BusinessLayer dans Visual Studio

### 3. Mettre à jour Form1.cs
→ Ajouter les deux boutons et leurs handlers (btnPushClients, btnPushCommandes)

---

## Dans le Form1.Designer.cs — Ajouter 2 boutons

Dans le designer, ajouter ces boutons (ou faire glisser depuis la Boîte à outils) :

```csharp
// Dans InitializeComponent() :
this.btnPushClients = new System.Windows.Forms.Button();
this.btnPushCommandes = new System.Windows.Forms.Button();

// Propriétés btnPushClients
this.btnPushClients.Location = new System.Drawing.Point(12, 350);
this.btnPushClients.Name = "btnPushClients";
this.btnPushClients.Size = new System.Drawing.Size(200, 35);
this.btnPushClients.Text = "Pousser clients → BC Cloud";
this.btnPushClients.Click += new System.EventHandler(this.btnPushClients_Click);

// Propriétés btnPushCommandes
this.btnPushCommandes.Location = new System.Drawing.Point(220, 350);
this.btnPushCommandes.Name = "btnPushCommandes";
this.btnPushCommandes.Size = new System.Drawing.Size(220, 35);
this.btnPushCommandes.Text = "Pousser commandes → BC Cloud";
this.btnPushCommandes.Click += new System.EventHandler(this.btnPushCommandes_Click);
```

---

## Dans BcSyncService.cs — Remplir les constantes BC

```csharp
private const string BC_TENANT_ID   = "VOTRE_TENANT_ID";     // Azure AD
private const string BC_ENVIRONMENT = "production";           // ou "sandbox"
private const string BC_COMPANY_ID  = "VOTRE_COMPANY_ID";    // GUID dans l'URL BC
private const string BC_USERNAME    = "VOTRE_USERNAME";       // Utilisateur BC
private const string BC_WEB_KEY     = "VOTRE_WEB_SERVICE_KEY"; // Clé Web Service BC
```

### Comment trouver le COMPANY_ID :
1. Connexion BC Cloud → Paramètres → Informations société
2. Copier le GUID depuis l'URL du navigateur : `.../companies(XXXXXXXX-XXXX-...)/`

### Comment obtenir la Web Service Key :
1. BC → Rechercher "Utilisateurs"
2. Ouvrir ton utilisateur → section "Clés service Web"
3. Générer une nouvelle clé

---

## Vérification — Tester sans pousser

Pour tester la connexion BC avant le push réel, ajouter ce bouton test :

```csharp
private void btnTestBC_Click(object sender, EventArgs e)
{
    // Simple GET pour vérifier la connexion
    try
    {
        var http = new System.Net.Http.HttpClient();
        var cred = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes("USERNAME:WEBKEY"));
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", cred);

        var url = "https://api.businesscentral.dynamics.com/v2.0/TENANT/production/api/Nour/salesSync/v1.0/companies(COMPANYID)/customers";
        var result = http.GetAsync(url).Result;
        MessageBox.Show("Status : " + result.StatusCode);
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erreur : " + ex.Message);
    }
}
```
