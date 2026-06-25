using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZeroFat.Users.Infrastructure.Persistence.Configurations;

public class ApplicationUserRoleConfig : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder
            .ToTable("UserRoles");
    }

}
