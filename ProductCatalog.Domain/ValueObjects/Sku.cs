using System.Text.RegularExpressions;
using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.ValueObjects;

public sealed record Sku
{
    private static readonly Regex Allowed = new(@"^[A-Z0-9]{3,32}$", RegexOptions.Compiled);

    public string Value { get; }

    public Sku(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvariantViolationException("SKU cannot be empty.");

        var normalized = Normalize(raw);

        if (!Allowed.IsMatch(normalized))
            throw new InvariantViolationException("SKU has invalid format. Use 3-32 alphanumeric characters.");

        Value = normalized;
    }

    public static string Normalize(string raw)
    {
        var s = raw.Trim().ToUpperInvariant();

        s = s.Replace(" ", "")
             .Replace("-", "")
             .Replace("_", "");

        s = NormalizeZeroVsO(s);

        return s;
    }

    private static string NormalizeZeroVsO(string s)
    {
        var hasDigit = s.Any(char.IsDigit);
        return hasDigit ? s.Replace('O', '0') : s;
    }

    public override string ToString() => Value;
}
