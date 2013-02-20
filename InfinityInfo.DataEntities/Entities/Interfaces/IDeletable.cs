
namespace InfinityInfo.DataEntities.Entities
{   
    /// <summary>
    /// Interface implemented by SLXEntity to allow deletes.
    /// </summary>
    public interface IDeletable
    {
        /// <summary>
        /// Delete method
        /// </summary>
        void Delete();
    }
}
