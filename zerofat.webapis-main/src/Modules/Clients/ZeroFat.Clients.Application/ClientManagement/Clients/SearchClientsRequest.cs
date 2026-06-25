using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class SearchClientsRequest : PaginationFilter, IQuery<PaginationResponse<ClientDto>>
{
    public bool? IsActive { get; set; }
    public Gender? Gender { get; set; }
}


public class SearchClientsRequestHandler(IReadRepository<Client> repository, IReadRepository<Allergen> allergenRepo) : IRequestHandler<SearchClientsRequest, PaginationResponse<ClientDto>>
{
    private readonly IReadRepository<Client> _repository = repository;
    private readonly IReadRepository<Allergen> _allergenRepo = allergenRepo;

    public async Task<PaginationResponse<ClientDto>> Handle(SearchClientsRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.PaginatedListAsync(new ClientsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
        foreach (var item in result.Data)
        {
            if (item.ClientAllergicIds.Count != 0)
                item.ClientAllergics = await _allergenRepo.ListAsync(new ExpressionSpecificationProjecting<Allergen, ClientAllergen>(x => item.ClientAllergicIds.Contains(x.Id)), cancellationToken);
        }

        return result;
    }

}
