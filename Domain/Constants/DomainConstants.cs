namespace Domain.Constants;

public static class DomainConstants
{
    public const string EgyptTimeZoneId = "Africa/Cairo";
    public const string DefaultCurrency = "EGP";

    public static class IdentityRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string SuperAdminId = "8f7a6e9d-9c10-4d39-9f02-19fd92b928c1";
        public const string Customer = "Customer";
        public const string CustomerId = "9f1da086-d445-4c43-92e7-c1631e8732c5";
    }

    public static class TenantRoles
    {
        public const string OwnerId = "cc1066e7-9396-4e16-ae94-6c0a0946ccee";
        public const string AdminId = "6bcadbf7-efdb-4a4a-8313-c26ea2208688";
        public const string AccountantId = "e9bf9042-922c-4f34-86ca-c5e1a2156147";
    }
}
