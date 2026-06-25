using Ardalis.Specification;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using Mapster;

namespace ZeroFat.Application.Common.Models;

public static class PaginationResponseExtensions
{
    public static async Task<PaginationResponse<TDestination>> PaginatedListAsync<T, TDestination>(
        this IReadRepositoryBase<T> repository, ISpecification<T, TDestination> spec, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        where T : class
        where TDestination : class, IDto
    {
        var list = await repository.ListAsync(spec, cancellationToken);
        int count = await repository.CountAsync(spec, cancellationToken);

        return new PaginationResponse<TDestination>(list, count, pageNumber, pageSize);
    }

    public static async Task<PaginationResponse<TDestination>> PaginatedListAsync<T, TDestination>(
        this IReadRepository<T> repository, ISpecification<T, TDestination> spec, int pageNumber, int pageSize, TypeAdapterConfig config, CancellationToken cancellationToken = default)
        where T : class, IAggregateRoot
        where TDestination : class, IDto
    {
        var list = await repository.ListAsync(spec, config, cancellationToken);
        int count = await repository.CountAsync(spec, cancellationToken);

        return new PaginationResponse<TDestination>(list, count, pageNumber, pageSize);
    }
}
