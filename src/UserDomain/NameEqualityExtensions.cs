using System;
using System.Globalization;
using System.Text;

namespace UserDomain;

internal static class NameEqualityExtensions
{
    private static bool IsLetterOrMark(Rune rune)
    {
        var cat = Rune.GetUnicodeCategory(rune);
        return cat is UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or UnicodeCategory.NonSpacingMark
            or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark;
    }

    public static bool IsAllowedNameRune(Rune rune, bool allowPeriod = false)
    {
        if (IsLetterOrMark(rune)) return true;
        if (rune.Value == ' ' || rune.Value == '-' || rune.Value == '\'' || rune.Value == '’') return true;
        if (allowPeriod && rune.Value == '.') return true;
        return false;
    }
}