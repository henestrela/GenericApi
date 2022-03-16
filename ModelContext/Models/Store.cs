using System;
using Utils.Interfaces;

namespace ModelContext.Models
{
    public class Store : IEntityModel
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
