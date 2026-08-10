page 50152 "Sales Line API"
{
    PageType = API;
    APIPublisher = 'Nour';
    APIGroup = 'salesSync';
    APIVersion = 'v1.0';
    EntityName = 'salesDocumentLine';
    EntitySetName = 'salesDocumentLines';
    SourceTable = "Sales Line";
    DelayedInsert = true;
    ODataKeyFields = SystemId;
    InsertAllowed = true;
    ModifyAllowed = true;
    DeleteAllowed = true;

    layout
    {
        area(Content)
        {
            // ── Clés ─────────────────────────────────────────────
            field(id; Rec.SystemId)
            {
                Caption = 'id';
            }
            field(documentId; Rec."Document No.")
            {
                Caption = 'documentId';
            }
            field(sequence; Rec."Line No.")
            {
                Caption = 'sequence';
            }

            // ── Type de ligne ─────────────────────────────────────
            field(lineType; Rec.Type)
            {
                Caption = 'lineType';
            }
            field(itemNo; Rec."No.")
            {
                Caption = 'itemNo';
            }
            field(description; Rec.Description)
            {
                Caption = 'description';
            }

            // ── Quantité / Prix ───────────────────────────────────
            field(quantity; Rec.Quantity)
            {
                Caption = 'quantity';
            }
            field(unitPrice; Rec."Unit Price")
            {
                Caption = 'unitPrice';
            }
            field(unitCost; Rec."Unit Cost (LCY)")
            {
                Caption = 'unitCost';
            }
            field(unitOfMeasureCode; Rec."Unit of Measure Code")
            {
                Caption = 'unitOfMeasureCode';
            }

            // ── Remise ────────────────────────────────────────────
            field(discountPercent; Rec."Line Discount %")
            {
                Caption = 'discountPercent';
            }
            field(discountAmount; Rec."Line Discount Amount")
            {
                Caption = 'discountAmount';
            }

            // ── Montants ──────────────────────────────────────────
            field(amountExcludingTax; Rec.Amount)
            {
                Caption = 'amountExcludingTax';
            }
            field(amountIncludingTax; Rec."Amount Including VAT")
            {
                Caption = 'amountIncludingTax';
            }
            field(taxPercent; Rec."VAT %")
            {
                Caption = 'taxPercent';
            }
            field(lineAmount; Rec."Line Amount")
            {
                Caption = 'lineAmount';
            }

            // ── Livraison ─────────────────────────────────────────
            field(locationCode; Rec."Location Code")
            {
                Caption = 'locationCode';
            }
            field(shipmentDate; Rec."Shipment Date")
            {
                Caption = 'shipmentDate';
            }
            field(quantityShipped; Rec."Quantity Shipped")
            {
                Caption = 'quantityShipped';
            }
            field(quantityInvoiced; Rec."Quantity Invoiced")
            {
                Caption = 'quantityInvoiced';
            }

            // ── Méta ──────────────────────────────────────────────
            field(lastModifiedDateTime; Rec.SystemModifiedAt)
            {
                Caption = 'lastModifiedDateTime';
            }
        }
    }
}
