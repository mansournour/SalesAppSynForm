page 50151 "Customer API"
{
    PageType = API;
    APIPublisher = 'Nour';
    APIGroup = 'salesSync';
    APIVersion = 'v1.0';
    EntityName = 'customer';
    EntitySetName = 'customers';
    SourceTable = Customer;
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
            field(customerNo; Rec."No.")
            {
                Caption = 'customerNo';
            }
            field(name; Rec.Name)
            {
                Caption = 'name';
            }
            // ── Champs supplémentaires ───────────────────────────
            field(phoneNo; Rec."Phone No.")
            {
                Caption = 'phoneNo';
            }
            field(email; Rec."E-Mail")
            {
                Caption = 'email';
            }
            field(address; Rec.Address)
            {
                Caption = 'address';
            }
            field(city; Rec.City)
            {
                Caption = 'city';
            }
            field(countryRegionCode; Rec."Country/Region Code")
            {
                Caption = 'countryRegionCode';
            }
            field(currencyCode; Rec."Currency Code")
            {
                Caption = 'currencyCode';
            }
            field(blocked; Rec.Blocked)
            {
                Caption = 'blocked';
            }
            field(balance; Rec."Balance (LCY)")
            {
                Caption = 'balance';
            }
            field(lastModifiedDateTime; Rec.SystemModifiedAt)
            {
                Caption = 'lastModifiedDateTime';
            }
        }
    }
}
