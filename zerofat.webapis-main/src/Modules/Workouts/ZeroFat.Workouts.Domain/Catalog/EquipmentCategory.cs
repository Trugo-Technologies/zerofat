namespace ZeroFat.GymUp.Domain.Catalog;

public class EquipmentCategory : ActivationEntity, IAggregateRoot
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? IconUrl { get; set; }

    public virtual ICollection<Equipment> Equipments { get; set; }
    public EquipmentCategory()
    {
        Equipments = new HashSet<Equipment>();
    }
}
