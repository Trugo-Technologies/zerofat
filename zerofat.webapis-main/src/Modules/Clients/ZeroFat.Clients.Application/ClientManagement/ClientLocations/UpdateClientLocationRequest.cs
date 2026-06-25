using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement;
public class UpdateClientLocationRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? FullAddressEn { get; set; }
    public string? FullAddressAr { get; set; }
    public string? Area { get; set; }
    public string? Building { get; set; }
    public string? Block { get; set; }
    public string? Street { get; set; }
    public string? Flat { get; set; }
    public string? Office { get; set; }
    public AddressType? Type { get; set; }
    public string? PostalCode { get; set; }
    public string? NotesFromClient { get; set; }
    public string? NotesFromZeroFat { get; set; }
}

public class UpdateClientLocationRequestValidator : CustomValidator<UpdateClientLocationRequest>
{
    public UpdateClientLocationRequestValidator(IReadRepository<ClientLocation> repository, IStringLocalizer<UpdateClientLocationRequestValidator> localaizer)
    {


        RuleFor(u => u.Type)
           .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}

public class UpdateClientLocationRequestHandler(IRepository<ClientLocation> repository, IStringLocalizer<UpdateClientLocationRequestHandler> localizer) : IRequestHandler<UpdateClientLocationRequest, Result<Guid>>
{
    private readonly IRepository<ClientLocation> _repository = repository;
    private readonly IStringLocalizer<UpdateClientLocationRequestHandler> _localizer = localizer;


    public async Task<Result<Guid>> Handle(UpdateClientLocationRequest request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = location ?? throw new NotFoundException(_localizer["Location not found"]);
       
        location.Longitude = request.Longitude;
        location.Latitude = request.Latitude;
        location.FullAddressEn = request.FullAddressEn;
        location.FullAddressAr = request.FullAddressAr;
        location.Building = request.Building;
        location.Area = request.Area;
        location.Street = request.Street;
        location.Office = request.Office;
        location.Type = request.Type;
        location.PostalCode = request.PostalCode;
        location.NotesFromClient = request.NotesFromClient;
        location.NotesFromZeroFat = request.NotesFromZeroFat;

        await _repository.UpdateAsync(location, cancellationToken);

        return await Result<Guid>.SuccessAsync(location.Id);
    }
}

