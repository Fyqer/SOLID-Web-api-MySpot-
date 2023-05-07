namespace MySpot.Application.DTO
{
    public class ReservationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
    }
}
