using System;
using Utils.Interfaces;

namespace ModelContext.Models
{
    public class Product : IEntityModel
    {
        public Guid Id { get; set; }
        public Guid SectorId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
