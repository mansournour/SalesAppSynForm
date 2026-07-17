page 50150 "Sales Header API"
{
    PageType = API;
    APIPublisher = 'Nour';
    APIGroup = 'salesSync';
    APIVersion = 'v1.0';
    EntityName = 'salesDocument';
    EntitySetName = 'salesDocuments';
    SourceTable = "Sales Header";
    DelayedInsert = true;
    ODataKeyFields = SystemId;
    InsertAllowed = true;
    ModifyAllowed = true;
    DeleteAllowed = true;

    layout
    {
        area(Content)
        {
            // ── Champs obligatoires ──────────────────────────────
            field(id; Rec.SystemId)
            {
                Caption = 'id';
            }
            field(documentNo; Rec."No.")
            {
                Caption = 'documentNo';
            }
            field(documentType; Rec."Document Type")
            {
                Caption = 'documentType';
            }
            field(customerNo; Rec."Sell-to Customer No.")
            {
                Caption = 'customerNo';
            }
            // ── Champs supplémentaires ───────────────────────────
            field(customerName; Rec."Sell-to Customer Name")
            {
                Caption = 'customerName';
            }
            field(postingDate; Rec."Posting Date")
            {
                Caption = 'postingDate';
            }
            field(dueDate; Rec."Due Date")
            {
                Caption = 'dueDate';
            }
            field(amount; Rec.Amount)
            {
                Caption = 'amount';
            }
            field(amountIncludingVAT; Rec."Amount Including VAT")
            {
                Caption = 'amountIncludingVAT';
            }
            field(currencyCode; Rec."Currency Code")
            {
                Caption = 'currencyCode';
            }
            field(status; Rec.Status)
            {
                Caption = 'status';
            }
            field(externalDocumentNo; Rec."External Document No.")
            {
                Caption = 'externalDocumentNo';
            }
            field(salespersonCode; Rec."Salesperson Code")
            {
                Caption = 'salespersonCode';
            }
            field(locationCode; Rec."Location Code")
            {
                Caption = 'locationCode';
            }
            field(lastModifiedDateTime; Rec.SystemModifiedAt)
            {
                Caption = 'lastModifiedDateTime';
            }
        }
    }

    // Filtre dynamique sur DocumentType via query string : ?$filter=documentType eq 'Order'
    // BC gère le filtre OData nativement sur les champs exposés.
}
