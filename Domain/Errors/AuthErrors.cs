namespace Domain.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "بيانات تسجيل الدخول غير صحيحة.");

    public static readonly Error EmailAlreadyRegistered =
        new("Auth.EmailAlreadyRegistered", "البريد الإلكتروني مسجل بالفعل.");

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "المستخدم غير موجود.");

    public static readonly Error TenantAccessDenied =
        new("Auth.TenantAccessDenied", "ليس لديك صلاحية الوصول إلى هذه المطبعة.");

    public static readonly Error AccountNotActive =
        new("Auth.AccountNotActive", "الحساب غير نشط.");

    public static readonly Error InvalidRefreshToken =
        new("Auth.InvalidRefreshToken", "رمز التجديد غير صالح أو منتهي الصلاحية.");

    public static readonly Error TokenGenerationFailed =
        new("Auth.TokenGenerationFailed", "تعذر إنشاء رموز الدخول.");
}
