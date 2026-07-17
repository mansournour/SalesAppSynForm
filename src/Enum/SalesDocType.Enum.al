enum 50150 "Sales Doc Type"
{
    Extensible = true;

    value(0; Quote) { Caption = 'Devis'; }
    value(1; "Order") { Caption = 'Commande'; }
    value(2; Invoice) { Caption = 'Facture'; }
    value(3; "Credit Memo") { Caption = 'Avoir'; }
    value(4; "Blanket Order") { Caption = 'Commande Ouverte'; }
    value(5; "Return Order") { Caption = 'Retour'; }
}
