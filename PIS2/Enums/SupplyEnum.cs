namespace PIS2.Enums
{
    public enum SupplierInvoiceStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        PartiallyPaid = 3,
        Paid = 4,
        Cancelled = 5,
        Reversed = 6
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