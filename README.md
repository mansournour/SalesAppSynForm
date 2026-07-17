# SalesDocSyncApp

Application de synchronisation des données de vente entre une base de données Oracle locale et Microsoft Business Central Cloud via API REST OAuth 2.0.

---

## Architecture

```
SalesDocSyncApp/
├── src/                          ← Extension AL (Business Central)
│   ├── API/
│   │   ├── SalesHeaderAPI.Page.al   ← API Page Sales Documents (GET/POST/PATCH/DELETE)
│   │   └── CustomerAPI.Page.al      ← API Page Customers (CRUD complet)
│   └── Enum/
│       └── SalesDocType.Enum.al     ← Enum types de document (Devis, Commande, Facture, Avoir)
│
├── dotnet/
│   ├── SalesSyncService/         ← Service console .NET 8 (Oracle → BC Cloud)
│   │   ├── Config/
│   │   │   └── BcApiConfig.cs       ← Configuration connexion BC Cloud
│   │   ├── Models/
│   │   │   ├── CustomerModel.cs     ← Modèles client (local + BC)
│   │   │   └── SalesHeaderModel.cs  ← Modèles document vente
│   │   ├── Services/
│   │   │   ├── BcApiClient.cs       ← Client HTTP générique (GET/POST/PATCH/DELETE)
│   │   │   ├── CustomerSyncService.cs ← Sync clients Oracle → BC
│   │   │   └── SalesHeaderService.cs  ← CRUD Sales Headers avec filtre documentType
│   │   └── Program.cs               ← Menu interactif console
│   │
│   └── SalesApp_Integration/     ← Intégration WinForms Visual Studio
│       ├── BusinessLayer/
│       │   ├── SalesHeaderBC.cs     ← Couche métier : Oracle + OAuth + Push BC
│       │   └── BcSyncService.cs     ← Service HTTP vers API BC
│       └── Form1.cs                 ← Formulaire WinForms avec boutons Push
│
└── SalesApp/                     ← Projet WinForms Visual Studio (C# .NET Framework 4.7.2)
    ├── SalesApp.sln
    └── SalesApp/
        ├── BusinessLayer/
        │   └── SalesHeaderBC.cs     ← Méthodes GET Oracle + POST/PATCH BC (OAuth 2.0)
        ├── Form1.cs                 ← UI : Charger données + Pousser vers BC
        └── Form1.Designer.cs        ← 3 boutons : Charger / Push clients / Push commandes
```

---

## Fonctionnalités

### Extension Business Central (AL)
- **API Page `salesDocuments`** — expose les Sales Headers BC avec filtre OData sur `documentType`
  - Types supportés : `Quote` (Devis), `Order` (Commande), `Invoice` (Facture), `Credit Memo` (Avoir)
  - Champs obligatoires : `documentNo`, `customerNo`
  - Filtre exemple : `?$filter=documentType eq 'Order'`
- **API Page `customers`** — CRUD complet sur les fiches clients BC
- **Enum `Sales Doc Type`** — 6 valeurs : Quote, Order, Invoice, Credit Memo, Blanket Order, Return Order

### Service .NET (SalesSyncService)
- Lecture des clients et commandes depuis Oracle local
- Synchronisation Oracle → BC Cloud :
  - **POST** si le client/document n'existe pas dans BC
  - **PATCH** si il existe déjà (mise à jour partielle avec `If-Match: *`)
- Menu interactif : lister, créer, patcher, supprimer des Sales Headers par type
- Cache du token OAuth (renouvellement automatique)

### Application WinForms (SalesApp)
- Affichage des commandes Oracle dans un `DataGridView`
- Bouton **"Pousser clients → BC"** — sync tous les clients Oracle vers BC
- Bouton **"Pousser commandes → BC"** — sync toutes les commandes comme Sales Orders

---

## Prérequis

### Business Central
- Environnement BC Cloud (Sandbox ou Production)
- Extension AL déployée (F5 dans VS Code)

### .NET / Visual Studio
- .NET Framework 4.7.2 (projet WinForms)
- .NET 8.0 (service console)
- Oracle.ManagedDataAccess NuGet package
- Oracle Database local avec tables `Clients` et `SalesHeader`

### Azure AD
- App Registration avec permissions `Dynamics 365 Business Central` → `user_impersonation`
- Consentement administrateur accordé

---

## Configuration

### 1. Déployer l'extension AL

Ouvrir le dossier `SalesDocSyncApp` dans **VS Code** puis :

```
Ctrl+Shift+P → AL: Download Symbols
F5           → Publier dans BC
```

### 2. Configurer les credentials BC

Dans `dotnet/SalesSyncService/Config/BcApiConfig.cs` :

```csharp
TenantId      = "VOTRE_TENANT_ID";       // Azure AD Tenant ID
Environment   = "DEV";                   // ou "production"
CompanyId     = "VOTRE_COMPANY_GUID";    // GUID visible dans l'URL BC
Username      = "user@domaine.com";
WebServiceKey = "VOTRE_WEB_SERVICE_KEY"; // BC → Utilisateurs → Clés service Web
```

Dans `SalesApp/SalesApp/BusinessLayer/SalesHeaderBC.cs` :

```csharp
BC_TENANT_ID     = "VOTRE_TENANT_ID";
BC_CLIENT_ID     = "VOTRE_CLIENT_ID";      // Azure App Registration
BC_CLIENT_SECRET = "VOTRE_CLIENT_SECRET";
BC_USERNAME      = "user@domaine.com";
BC_PASSWORD      = "VOTRE_MOT_DE_PASSE";
```

### 3. Lancer le service console

```bash
cd dotnet/SalesSyncService
dotnet run
```

### 4. Lancer l'application WinForms

Ouvrir `SalesApp/SalesApp.sln` dans Visual Studio → **F5**

---

## Structure de la base de données Oracle locale

```sql
CREATE TABLE Clients (
    id        NUMBER PRIMARY KEY,
    nom       VARCHAR2(50),
    prenom    VARCHAR2(50),
    telephone VARCHAR2(20),
    email     VARCHAR2(100)
);

CREATE TABLE SalesHeader (
    NumCommande  NUMBER PRIMARY KEY,
    NumClient    NUMBER,
    DateCommande DATE,
    CONSTRAINT fk_client FOREIGN KEY (NumClient) REFERENCES Clients(id)
);
```

---

## Endpoints API BC (après déploiement extension)

Base URL :
```
https://api.businesscentral.dynamics.com/v2.0/{tenantId}/{environment}/api/Nour/salesSync/v1.0/companies({companyId})
```

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/salesDocuments` | Tous les documents |
| GET | `/salesDocuments?$filter=documentType eq 'Order'` | Filtrer par type |
| POST | `/salesDocuments` | Créer un document |
| PATCH | `/salesDocuments({id})` | Modifier un document |
| DELETE | `/salesDocuments({id})` | Supprimer un document |
| GET | `/customers` | Tous les clients |
| POST | `/customers` | Créer un client |
| PATCH | `/customers({id})` | Modifier un client |

---

## Auteur

**Nour** — Publisher: `Nour`, API Group: `salesSync`, Version: `v1.0`
