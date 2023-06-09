using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public class VehicleReservation : Reservation
{
    public EmployeeName EmployeeName { get; private set; }
    public LicencePlate LicencePlate { get; private set; }


    public VehicleReservation(ReservationId Id, EmployeeName employeeName,
        LicencePlate licencePlate, Capacity capacity, Date date) : base(Id, capacity, date)
    {
        EmployeeName = employeeName;
        LicencePlate = licencePlate;
    }

    public void ChangeLicencePlate(LicencePlate licencePlate)
        => LicencePlate = licencePlate;
}