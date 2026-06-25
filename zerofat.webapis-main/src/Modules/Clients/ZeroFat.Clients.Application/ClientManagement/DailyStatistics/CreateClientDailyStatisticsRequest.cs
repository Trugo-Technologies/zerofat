using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;
public class CreateClientDailyStatisticsRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType ClientId { get; set; }
    public double? WeightInKG { get; set; }
    public double? WaterInLiter { get; set; }
    public DateOnly Date { get; set; }
    public List<CaloriesDailyStatisticsRequest> CaloriesDailyStatistics { get; set; } = [];
}

public class CaloriesDailyStatisticsRequest
{
    public string? Name { get; set; }
    public double? Calories { get; set; }
    public bool? IsFood { get; set; }
}

public class CreateClientDailyStatisticsRequestValidator : CustomValidator<CreateClientDailyStatisticsRequest>
{
    public CreateClientDailyStatisticsRequestValidator(IReadRepository<ClientDailyStatistics> repository, IReadRepository<Client> clientRepo, IStringLocalizer<CreateClientDailyStatisticsRequestValidator> loc)
    {

        RuleFor(u => u.ClientId)
           .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        //RuleFor(u => u.ClientId)
        //   .Cascade(CascadeMode.Stop)
        //    .NotEmpty()
        //    .MustAsync(async (req, id, _) => !await repository.AnyAsync(new ExpressionSpecification<ClientDailyStatistics>(x => x.ClientId == req.ClientId && x.Date == req.Date), _))
        //    .WithMessage(loc["Statistics already exists"]);


    }
}


public class CreateClientDailyStatisticsRequestHandler(IRepository<ClientDailyStatistics> repo, IRepository<Client> clientRepo, IStringLocalizer<CreateClientDailyStatisticsRequestHandler> localizer) : IRequestHandler<CreateClientDailyStatisticsRequest, Result<DefaultIdType>>
{
    private readonly IRepository<ClientDailyStatistics> _repo = repo;
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly IStringLocalizer<CreateClientDailyStatisticsRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(CreateClientDailyStatisticsRequest request, CancellationToken cancellationToken)
    {
        var client = await _clientRepo.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
            throw new NotFoundException(_localizer["Client not found"]);

        var stat = await _repo.FirstOrDefaultAsync(new ExpressionSpecification<ClientDailyStatistics>(x => x.Date == request.Date && x.ClientId == request.ClientId), cancellationToken);
        if (stat != null)
        {
            bool updateClient = false;
            if (request.WeightInKG.HasValue)
            {
                stat.WeightInKG = request.WeightInKG.Value;
                client.CurrentWeightInKG = stat.WeightInKG.Value;
                updateClient = true;
            }

            if (request.WaterInLiter.HasValue)
                stat.WaterInLiter = request.WaterInLiter.Value;

            var items = request.CaloriesDailyStatistics.ConvertAll(x => new CaloriesDailyStatistics()
            {
                Calories = x.Calories,
                IsFood = x.IsFood,
                Name = x.Name,

            }).ToList();

            stat.CaloriesDailyStatistics.AddRange(items);
            await _repo.UpdateAsync(stat, cancellationToken);

            if (updateClient)
                await _clientRepo.UpdateAsync(client, cancellationToken);
        }
        else
        {
            stat = new ClientDailyStatistics
            {
                ClientId = request.ClientId,
                Date = request.Date,
                WeightInKG = request.WeightInKG,
                WaterInLiter = request.WaterInLiter,
                CaloriesDailyStatistics = request.CaloriesDailyStatistics.ConvertAll(x => new CaloriesDailyStatistics()
                {
                    Calories = x.Calories,
                    IsFood = x.IsFood,
                    Name = x.Name,

                })
            };


            await _repo.AddAsync(stat, cancellationToken);
            if (stat.WeightInKG.HasValue)
            {
                client.CurrentWeightInKG = stat.WeightInKG.Value;
                await _clientRepo.UpdateAsync(client, cancellationToken);
            }
        }


        return await Result<DefaultIdType>.SuccessAsync(stat.Id);
    }

}
