using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripShare.Models;

namespace TripShare.Abstract
{
    public interface ITripRepository
    {
        /*event Action<Trip> OnTripAdded;
        event Action<Trip> OnTripCanceled;
        event Action<Trip> OnSeatBought;
        event Action<Trip> OnSeatCanceled;*/

        Trip FindTrip(string id);
        IEnumerable<Trip> FindTrip(string from, string to, DateTime date);
        IEnumerable<Trip> FindTripBySh(string userId);
        IEnumerable<Passenger> FindPassenger(Trip trip);
        IEnumerable<Trip> FindPassengersTrips(string passengerId);
        bool AddTrip(Trip trip);
        bool CancelTrip(string id, string driverId);
        bool BuySeat(string id, string passengerId);
        bool CancelSeat(string id, string passengerId);
    }
}
