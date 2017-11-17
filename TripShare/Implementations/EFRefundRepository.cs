using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripShare.Models;
using TripShare.Abstract;

namespace TripShare.Implementations
{
    public class EFRefundRepository : IRefundRepository
    {
        private readonly TripsDbContext db;

        public EFRefundRepository(TripsDbContext _db)
        {
            db = _db;
        }

        private IEnumerable<Refund> RefundList
        {
            get
            {
                return db.Refunds;
            }
        }

        public IEnumerable<Refund> FindRefundsOnAddress(string Address)
        {
            return RefundList.Where(x => x.Address == Address);
        }

        public bool AddRefund(Refund refund)
        {
            if (refund.Address == null || refund.Amount < 1 || refund.Done != false)
                return false;

            db.Add(refund);
            db.SaveChanges();
            return true;
        }
    }
}
