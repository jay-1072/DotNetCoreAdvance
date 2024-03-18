using System.ComponentModel.DataAnnotations;

namespace DotNetCoreAdvance.Interfaces
{
    public interface IBaseEntity
    {
        [Key]
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime LastEdited { get; set; }
    }
}
