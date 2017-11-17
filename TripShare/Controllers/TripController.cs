using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TripShare.Models;
using Microsoft.AspNetCore.Identity;
using TripShare.Abstract;
using TripShare.Implementations;
using TripShare.Models.TripViewModels;
using System.Security.Cryptography;
using System.Text;

namespace TripShare.Controllers
{
    [Authorize]
    public class TripController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITripRepository tripRepository;
        private readonly IBlockchainRepository blockchain;

        public TripController(UserManager<ApplicationUser> _userManager, ITripRepository _tripRepository)
        {
            userManager = _userManager;
            tripRepository = _tripRepository;
        }

        [HttpGet]
        public IActionResult RegisterTripForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTripForm(RegisterTripFormViewModel trip)
        {
            ViewData["Message"] = "";
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            Trip t = new Trip
            {
                Id = RNGhelper.GetRandomAlphanumericString(14),
                Date = trip.Date,
                DriverId = user.ScriptHash,
                From = trip.From,
                To = trip.To,
                AdditionalWay = trip.AdditionalWay,
                Price = trip.Price,
                SeatsCount = trip.SeatsCount,
                Deposit = trip.Deposit,
                CancelDate = trip.CancelDate
            };

            if (true) //blockchain.AddTrip
            {
                if (tripRepository.AddTrip(t))
                {
                    ViewData["Message"] = "Your trip was registered";
                    return View("RegisterTrip");
                }
                ViewData["Message"] = "registered to blockchain, failed to database";
                return View("RegisterTrip");
            }
            ViewData["Message"] = "Failed to register to our database and blockchain";
            return View("RegisterTrip");
        }

        public async Task<IActionResult> YourTrips()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            List<Trip> trips = tripRepository.FindTripBySh(user.ScriptHash).ToList();
            foreach(var trip in trips)
            {
                trip.Passengers = tripRepository.FindPassenger(trip).ToList();
            }

            return View(trips);
        }

        public async Task<IActionResult> TripPersonalInfo(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            
            Trip trip = tripRepository.FindTrip(id);
            if (trip.DriverId != user.ScriptHash)
            {
                return RedirectToAction("TripCheater");
            }

            return View(trip);
        }

        public IActionResult TripCheater()
        {
            return View();
        }

        public async Task<IActionResult> CancelTrip(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (true)  //blockchain cancel trip
            {
                if (tripRepository.CancelTrip(id, user.ScriptHash))
                {
                    ViewData["Message"] = "Cancelled from bc and db";
                    return View("CancelTripSucc");
                }
                ViewData["Message"] = "Cancelled from bc not db";
                return View("CancelTripSucc");
            }

            ViewData["Message"] = "Failed to cancel";
            return View();
        }

        public IActionResult CancelTripSucc()
        {
            return View();
        }

        public async Task<IActionResult> YourSeats()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }


            List<Trip> trips = tripRepository.FindPassengersTrips(user.Id).ToList();
            foreach (var trip in trips)
            {
                trip.Passengers = tripRepository.FindPassenger(trip).ToList();
            }

            return View(trips);
        }

        public async Task<IActionResult> SeatsPersonalInfo(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            Trip trip = tripRepository.FindTrip(id);

            return View(trip);
        }

        public async Task<IActionResult> CancelSeat(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (true) //blockchain cancelseat method call
            {
                if (tripRepository.CancelSeat(id, user.Id))
                {
                    ViewData["Message"] = "Cancelled from db and bc";
                    return View("CancelSeatSucc");
                }
                ViewData["Message"] = "Cancelled from bc not db";
                return View("CancelSeatSucc");
            }
            ViewData["Message"] = "Failed to cancel";
            return View("CancelSeatSucc");
        }
    }
}
