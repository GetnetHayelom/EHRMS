namespace PIS2.Enums
{
    public enum SupplierInvoiceStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Posted = 3,
        PartiallyPaid = 4,
        Paid = 5,
        Cancelled = 6,
        Reversed = 7
    }

    public enum SupplierPaymentStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Posted = 3,
        Cancelled = 4,
        Reversed = 5
    }
}