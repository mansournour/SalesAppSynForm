page 50153 "Item API"
{
    PageType = API;
    APIPublisher = 'Nour';
    APIGroup = 'salesSync';
    APIVersion = 'v1.0';
    EntityName = 'item';
    EntitySetName = 'items';
    SourceTable = Item;
    DelayedInsert = true;
    ODataKeyFields = SystemId;
    InsertAllowed = false;
    ModifyAllowed = false;
    DeleteAllowed = false;

    layout
    {
        area(Content)
        {
            field(id; Rec.SystemId)
            {
                Caption = 'id';
            }
            field(no; Rec."No.")
            {
                Caption = 'no';
            }
            field(description; Rec.Description)
            {
                Caption = 'description';
            }
            field(unitPrice; Rec."Unit Price")
            {
                Caption = 'unitPrice';
            }
            field(unitCost; Rec."Unit Cost (LCY)")
            {
                Caption = 'unitCost';
            }
            field(blocked; Rec.Blocked)
            {
                Caption = 'blocked';
            }
            field(baseUnitOfMeasure; Rec."Base Unit of Measure")
            {
                Caption = 'baseUnitOfMeasure';
            }
            field(vatProdPostingGroup; Rec."VAT Prod. Posting Group")
            {
                Caption = 'vatProdPostingGroup';
            }
            field(lastModifiedDateTime; Rec.SystemModifiedAt)
            {
                Caption = 'lastModifiedDateTime';
            }
        }
    }
}
