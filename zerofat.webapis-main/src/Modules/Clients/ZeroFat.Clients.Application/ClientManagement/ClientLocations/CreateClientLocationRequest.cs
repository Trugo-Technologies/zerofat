using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement;
public class CreateClientLocationRequest : ICommand<Result<Guid>>
{
    public Guid ClientId { get; set; }
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

public class CreateClientLocationRequestValidator : CustomValidator<CreateClientLocationRequest>
{
    public CreateClientLocationRequestValidator(IReadRepository<ClientLocation> repository, IStringLocalizer<CreateClientLocationRequestValidator> localaizer)
    {

        RuleFor(u => u.Type)
           .Cascade(CascadeMode.Stop)
            .NotEmpty();

    }
}


public class CreateClientLocationRequestHandler(IRepository<ClientLocation> repo) : IRequestHandler<CreateClientLocationRequest, Result<Guid>>
{
    private readonly IRepository<ClientLocation> _repo = repo;


    public async Task<Result<Guid>> Handle(CreateClientLocationRequest request, CancellationToken cancellationToken)
    {
        var location = new ClientLocation
        {
            ClientId = request.ClientId,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            FullAddressEn = request.FullAddressEn,
            FullAddressAr = request.FullAddressAr,
            Building = request.Building,
            Area = request.Area,
            Street = request.Street,
            Office = request.Office,
            Type = request.Type,
            NotesFromClient = request.NotesFromClient,
            NotesFromZeroFat = request.NotesFromZeroFat,
            PostalCode = request.PostalCode,
        };

        await _repo.AddAsync(location, cancellationToken);

        return await Result<Guid>.SuccessAsync(location.Id);
    }

}
