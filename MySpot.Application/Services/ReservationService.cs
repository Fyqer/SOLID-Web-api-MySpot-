
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Application.Exceptions;
using MySpot.Core.Abstractions;
using MySpot.Core.DomainServices;
using MySpot.Core.Entities;
using MySpot.Core.Exceptions;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;


namespace MySpot.Application.Services
{
    public sealed class ReservationService : IReservationService
    {
        private readonly IClock _clock;
        private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
        private readonly IParkingReservationService _parkingReservationService;
        public ReservationService(IClock clock, IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
             IParkingReservationService parkingReservationService)
        {
            _clock = clock;
            _parkingReservationService = parkingReservationService;
            _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllWeeklyAsync()
        
           => (await _weeklyParkingSpotRepository
                .GetAllAsync())
                .SelectMany(x => x.Reservations)
                .Select(x => new ReservationDto
                {
                    Id = x.Id,
                    EmployeeName = x is VehicleReservation vr ? vr.EmployeeName : null,
                    Date = x.Date.Value.Date
                });
        

        public async Task<ReservationDto> GetAsync(Guid id)
       =>   ( await GetAllWeeklyAsync()).SingleOrDefault(x => x.Id == id);
        

        public async Task ReserveForVehniceAsync (ReserveParkingSpotForVehicle command)
        {
                var (spotId, reservationId, employeeName, licensePlate, capacity, date) = command;
                var week = new Week(_clock.Current());
                var parkingSpotId = new ParkingSpotId(spotId);
                var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();

                var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
            
                if (parkingSpotToReserve == null)
                {
                throw new WeeklyParkingSpotNotFoundException(spotId);
                }


                var reservation = new VehicleReservation(reservationId, employeeName, licensePlate, capacity, new Date(date));


                _parkingReservationService.ReserveSpotForVehicle(weeklyParkingSpots, JobTitle.Employee,
                    parkingSpotToReserve, reservation);
                await _weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);
        }
        public async Task ReserveForVehniceCleaningAsync(ReserveParkingSpotForCleaning command)
        {
            var week = new Week(command.Date);
            var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();

            _parkingReservationService.ReserveParkingForCleaning(weeklyParkingSpots, new Date(command.Date));
         
            foreach(var parkingSpot in weeklyParkingSpots)
            {
                await _weeklyParkingSpotRepository.UpdateAsync(parkingSpot);
            }
        }
        public async Task ChangeReservationLicensePlate(ChangeReservationLicensePlate command)
        {
                var weeklyParkingSpot = await GetWeeklyParkingSpotByReservation(command.ReservationId);

                if (weeklyParkingSpot is null)
                {
                    throw new WeeklyParkingSpotNotFoundException();
                }

                var reservationId = new ReservationId(command.ReservationId);
                var resevation = weeklyParkingSpot.Reservations
                .OfType<VehicleReservation>()
                    .SingleOrDefault(x => x.Id == reservationId);


                if (resevation is null)
                {
                    throw new ReservationNotFoundException (command.ReservationId);
                }
                resevation.ChangeLicencePlate(command.LicencePlate);
            await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
        }

        public async Task DeleteAsync(DeleteReservation command)
        {
            var weeklyParkingSpot = await GetWeeklyParkingSpotByReservation(command.ReservationId);
            if (weeklyParkingSpot == null)
            {
                throw new WeeklyParkingSpotNotFoundException();
            }

            weeklyParkingSpot.RemoveReservation(command.ReservationId);
            await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);

        }

        private async Task<WeeklyParkingSpot> GetWeeklyParkingSpotByReservation(ReservationId id)
        {
            return  (await _weeklyParkingSpotRepository.GetAllAsync())
                .SingleOrDefault(x => x.Reservations.Any(r => r.Id == id));
        }

        private DateTime CurrentDate() => _clock.Current();



    }
}
