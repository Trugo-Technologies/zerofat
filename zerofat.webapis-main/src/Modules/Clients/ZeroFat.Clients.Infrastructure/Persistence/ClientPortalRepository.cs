using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence; 
internal sealed class ClientPortalRepository<T>(ClientPortalContext context) : RepositoryBase<T>(context), IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig? config, CancellationToken cancellationToken = default)
    {
        var queryResult = specification.Selector is not null ? await base.ApplySpecification(specification).ToListAsync(cancellationToken) : await ApplySpecification(specification, false).ProjectToType<TResult>(config).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig? config, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, false).ProjectToType<TResult>(config).FirstOrDefaultAsync(cancellationToken);
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    // This is only done when no Selector is defined, so regular specifications with a selector also still work.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();

}
