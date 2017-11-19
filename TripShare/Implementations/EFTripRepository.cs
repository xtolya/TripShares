using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripShare.Models;
using TripShare.Abstract;
using System.Data;

namespace TripShare.Implementations
{
    public class EFTripRepository : ITripRepository
    {
        /*public event Action<Trip> OnTripAdded;
        public event Action<Trip> OnTripCanceled;
        public event Action<Trip> OnSeatBought;
        public event Action<Trip> OnSeatCanceled;*/

        private readonly TripsDbContext db;

        private IEnumerable<Trip> TripsList
        {
            get
            {
                return db.Trips;
            }
        }

        private IEnumerable<Passenger> PassengersList
        {
            get
            {
                return db.Passengers;
            }
        }

        public EFTripRepository(TripsDbContext _db)
        {
            db = _db;
        }

        public Trip FindTrip(string id)
        {
            return TripsList.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Trip> FindPassengersTrips(string passengerId)
        {
            HashSet<string> tripIds = new HashSet<string>();
            foreach (Passenger p in PassengersList)
            {
                if (p.UserId == passengerId)
                    tripIds.Add(p.TripId);
            }

            return TripsList
                .Where(x => tripIds.Contains(x.Id));
        }

        public IEnumerable<Trip> FindTrip(string from, string to, DateTime date)
        {
            return TripsList
                .Where(t => t.From == from && t.To == to && t.Date.Date == date.Date);
        }

        public IEnumerable<Trip> FindTripBySh(string userSh)
        {
            return TripsList
                .Where(t => t.DriverId == userSh); ;
        }

        public IEnumerable<Passenger> FindPassenger(Trip trip)
        {
            return PassengersList
                .Where(t => t.TripId == trip.Id);
        }

        public bool AddTrip(Trip trip)
        {
            if (trip.From == null || trip.Id == null || trip.To == null || trip.Price == 0 ||
                trip.Date.ToTimestamp() < DateTime.Now.ToTimestamp()
                || trip.SeatsCount == 0 || trip.DriverId == null)
                return false;
            db.Add(trip);
            db.SaveChanges();
            return true;
        }

        public bool CancelTrip(string id, string driverId)
        {
            Trip trip = FindTrip(id);
            if (trip == null || driverId != trip.DriverId)
                return false;
            db.Remove(trip);
            db.SaveChanges();
            return true;
        }

        public bool BuySeat(string id, string passengerId)
        {
            Trip trip = FindTrip(id);
            if (trip == null) return false;
            Passenger passenger = new Passenger()
            {
                UserId = passengerId,
                TripId = id
            };

            if (trip.Passengers == null)
            {
                trip.Passengers = new List<Passenger>();
                trip.Passengers.Add(passenger);
            }
            else
            {
                if (trip.SeatsCount - trip.Passengers.Count() < 1)
                    return false;
                trip.Passengers.Add(passenger);
            }
            db.Attach(trip);
            db.Entry(trip).Collection(p => p.Passengers).IsModified = true;
            db.SaveChanges();
            return true;
        }

        public bool CancelSeat(string id, string passengerId)
        {
            Trip trip = FindTrip(id);
            trip.Passengers = FindPassenger(trip).ToList();
            if (trip.Passengers == null) return false;
            foreach (var p in trip.Passengers)
            {
                if (p.UserId == passengerId)
                {
                    trip.Passengers.Remove(p);
                    db.Attach(trip);
                    db.Entry(trip).Collection(x => x.Passengers).IsModified = true;
                    db.SaveChanges();
                    return true;
                }

            }
            return false;
        }
    }
}
