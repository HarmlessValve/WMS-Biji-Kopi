namespace CoffeeWMS.Models
{
    public interface IDisplayableModel
    {
        string GetDisplayName();
    }

    public abstract class MasterDataEntity : IDisplayableModel
    {
        public bool IsActive { get; set; }

        public virtual string StatusText => IsActive ? "Aktif" : "Nonaktif";

        public abstract string GetDisplayName();
    }
}
