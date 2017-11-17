using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripShare.Models;

namespace TripShare.Abstract
{
    public interface IRefundRepository
    {
        IEnumerable<Refund> FindRefundsOnAddress(string Address);
        bool AddRefund(Refund refund);
    }
}
