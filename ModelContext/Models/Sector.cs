using System;
using Utils.Interfaces;

namespace ModelContext.Models
{
    public class Sector: IEntityModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
