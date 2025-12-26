
using GridBox.Solar.Domain.IUnitOfWork;
using Sevval.Persistence.Context;

namespace Sevval.Persistence.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;



        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

        }



        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int Commit()
        {
            try
            {
                return _db.SaveChanges();
            }
            catch (Exception exception)
            {

                return 0;
            }
        }

        public ValueTask RollBackAsync()
        {
            return _db.DisposeAsync();
        }





        public void ClearTracking()
        {
            _db.ChangeTracker.Clear();
        }



    }

}
