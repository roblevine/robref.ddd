namespace RobRef.DDD.Domain.Users;

public readonly record struct UserId(Ulid Value)
{
    public static UserId NewId() => new(Ulid.NewUlid());

    public static UserId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("UserId string cannot be null or empty.");

        return new UserId(Ulid.Parse(value));
    }

    public static bool TryParse(string? value, out UserId userId)
    {
        if (string.IsNullOrWhiteSpace(value) || !Ulid.TryParse(value, out var ulid))
        {
            userId = default;
            return false;
        }
        
        userId = new UserId(ulid);
        return true;
    }

    public const int Length = 26; // ULID string length

    public override string ToString() => Value.ToString();

    public static implicit operator Ulid(UserId userId) => userId.Value;
    public static implicit operator UserId(Ulid ulid) => new(ulid);
}