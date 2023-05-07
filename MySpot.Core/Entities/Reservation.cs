using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public abstract class Reservation
{
    public Reservation(ReservationId id, Capacity capacity, Date date)
    {
        Id = id;
        Capacity = Capacity;
        Date = date;
    }

    public Capacity Capacity { get; set; }
    public ReservationId Id { get; }
    public Date Date { get; private set; }


}