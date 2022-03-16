using System;

namespace Utils.Interfaces
{
    public interface IEntityModel : IEntityIdModel<Guid>
    {
        public DateTime CreationDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
