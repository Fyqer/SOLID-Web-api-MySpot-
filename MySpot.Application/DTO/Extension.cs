using MySpot.Core.Entities;

namespace MySpot.Application.DTO
{
    public static class Extension
    {
        public static WeeklyParkingSpotDto AsDto(this WeeklyParkingSpot enttiy)
            => new()
        {
            Id = enttiy.Id.ToString(),
            Name = enttiy?.Name,
            Capacity = enttiy.Capacity,
            From = enttiy.Week.From.Value.DateTime,
            To = enttiy.Week.To.Value.DateTime,
            Reservations = enttiy.Reservations.Select(x => new ReservationDto
            {
                Id = x.Id,
                EmployeeName = x is VehicleReservation vr ? vr.EmployeeName : null,
                Date = x.Date.Value.Date
            })
        };
    }
}
