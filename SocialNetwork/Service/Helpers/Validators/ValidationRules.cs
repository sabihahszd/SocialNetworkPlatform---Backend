using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Service.Helpers.Validators
{
    internal static partial class ValidationRules
    {
        public static string Trimmed(string? value) => value?.Trim() ?? string.Empty;

        public const string StrongPasswordMessage = "Use at least 8 characters, one capital letter, and one symbol.";

        public static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);

        public static bool IsStrongPassword(string? value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 8)
                return false;

            var hasCapital = false;
            var hasSymbol = false;
            foreach (var character in value)
            {
                if (char.IsUpper(character))
                    hasCapital = true;
                else if (!char.IsLetterOrDigit(character))
                    hasSymbol = true;
            }

            return hasCapital && hasSymbol;
        }

        public static bool Within(string? value, int max) => Trimmed(value).Length <= max;

        public static bool PresentWithin(string? value, int max)
        {
            var text = Trimmed(value);
            return text.Length > 0 && text.Length <= max;
        }

        public static bool IsEmail(string? value)
        {
            var email = Trimmed(value);
            return MailAddress.TryCreate(email, out var address)
                && string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPrivacy(string? value)
        {
            var privacy = Trimmed(value).ToLowerInvariant();
            return privacy is "public" or "friends" or "communities";
        }

        public static bool IsOptionalPrivacy(string? value) => IsBlank(value) || IsPrivacy(value);

        public static bool IsTheme(string? value)
        {
            var theme = Trimmed(value).ToLowerInvariant();
            return theme is "system" or "light" or "dark";
        }

        public static bool IsRole(string? value)
        {
            var role = Trimmed(value).ToLowerInvariant();
            return role is "member" or "admin" or "superadmin";
        }

        public static bool IsAssignableRole(string? value)
        {
            var role = Trimmed(value).ToLowerInvariant();
            return role is "member" or "admin";
        }

        public static bool IsHexColor(string? value) => HexColor().IsMatch(Trimmed(value));

        public static bool IsOptionalHexColor(string? value) => IsBlank(value) || IsHexColor(value);

        public static bool IsReportStatus(string? value)
        {
            var status = Trimmed(value).ToLowerInvariant();
            return status is "open" or "dismissed" or "removed";
        }

        public static bool IsRequestStatus(string? value)
        {
            var status = Trimmed(value).ToLowerInvariant();
            return status is "pending" or "accepted" or "declined" or "done";
        }

        public static bool IsNotificationType(string? value)
        {
            var type = Trimmed(value).ToLowerInvariant();
            return type is "like" or "comment" or "friend-request" or "repost" or "community" or "mention";
        }

        [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")]
        private static partial Regex HexColor();
    }
}
