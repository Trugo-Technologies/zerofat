using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
public class UpdateDailySelectionRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public PreferredMealTime? DeliveryTime { get; set; } // The client's preferred delivery time for this meal
    public DefaultIdType? ClientLocationId { get; set; } // Foreign key to the client
    public bool? HasCutlery { get; set; } // Foreign key to the client
    public bool IsPaused { get; set; } // Foreign key to the client
    public DateOnly? ReplacementDate { get; set; } // Foreign key to the client
}

public class UpdateDailySelectionRequestHandler(
    IRepositoryWithEvents<DailySelection> repository,
    IStringLocalizer<UpdateDailySelectionRequestHandler> localizer) : IRequestHandler<UpdateDailySelectionRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<DailySelection> _repository = repository;
    private readonly IStringLocalizer<UpdateDailySelectionRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(UpdateDailySelectionRequest request, CancellationToken cancellationToken)
    {
        DailySelection? entity = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["DailySelection not found"]);

        if (request.ClientLocationId.HasValue)
        {
            entity.ClientLocationId = request.ClientLocationId.Value;
        }

        if (request.DeliveryTime.HasValue)
        {
            entity.DeliveryTime = request.DeliveryTime.Value;
        }

        if (request.HasCutlery.HasValue)
        {
            entity.HasCutlery = request.HasCutlery.Value;
        }

        if (request.IsPaused)
        {
            entity.DailySelectionStatus = DailySelectionStatus.Paused;
            entity.ReplacementDate = request.ReplacementDate;
        }
        else
        {
            entity.DailySelectionStatus = DailySelectionStatus.Pending;
            entity.ReplacementDate = null;
        }

        await _repository.UpdateAsync(entity, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(entity.Id);
    }
}

