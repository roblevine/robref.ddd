using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.Infrastructure.Persistence;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        // Primary key - convert UserId (Ulid) to string for storage
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value.ToString(),
                value => new UserId(Ulid.Parse(value)))
            .HasColumnName("Id")
            .HasMaxLength(UserId.Length);

        // Email value object - convert to string
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasColumnName("Email")
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        // Create unique index on Email
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // FirstName value object - convert to string
        builder.Property(u => u.FirstName)
            .HasConversion(
                firstName => firstName.Value,
                value => new FirstName(value))
            .HasColumnName("FirstName")
            .HasMaxLength(FirstName.MaxLength)
            .IsRequired();

        // LastName value object - convert to string
        builder.Property(u => u.LastName)
            .HasConversion(
                lastName => lastName.Value,
                value => new LastName(value))
            .HasColumnName("LastName")
            .HasMaxLength(LastName.MaxLength)
            .IsRequired();

        // Title value object - convert to nullable string
        builder.Property(u => u.Title)
            .HasConversion(
                title => title != null ? title.Value : null,
                value => value != null ? new Title(value) : null)
            .HasColumnName("Title")
            .HasMaxLength(Title.MaxLength)
            .IsRequired(false);
    }
}