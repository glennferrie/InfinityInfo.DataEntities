using InfinityInfo.DataEntities.Entities;

namespace InfinityInfo.DataEntities.BusinessRules
{
    public delegate bool DataFieldValidator(DataField field);

    public delegate bool DataEntityValidator(DataEntity entity);
}
