using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Shared;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;
public class SearchAllergensRequest : PaginationFilter, IQuery<PaginationResponse<AllergenDto>>
{
}

public class SearchAllergensRequestHandler(IReadRepository<Allergen> repository, IClientService clientService, ICurrentUser currentUser) : IRequestHandler<SearchAllergensRequest, PaginationResponse<AllergenDto>>
{
    private readonly IReadRepository<Allergen> _repository = repository;

    public async Task<PaginationResponse<AllergenDto>> Handle(SearchAllergensRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.PaginatedListAsync(new AllergensBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (result.Data.Count > 0 && isClient) 
        {
            var clientAllergicIds = await clientService.GetClientAllergicIdsByClientId(currentUser.GetUserId());
            if (clientAllergicIds?.Count > 0)
            {
                foreach (var allergen in result.Data)
                {
                    allergen.IsAllergic = clientAllergicIds.Contains(allergen.Id);
                }
            }
        }

        return result;
    }
}
