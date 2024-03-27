using SqlSugar;

namespace CoreWebAPI.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        SqlSugarClient GetDbClient();

        void BeginTran();
        void CommitTran();
        void RollbackTran();
        void Dispose();
    }
}
