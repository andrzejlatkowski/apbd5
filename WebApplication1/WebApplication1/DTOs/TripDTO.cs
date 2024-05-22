namespace WebApplication1.DTOs;

public class TripDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<ClientTripDTO> ClientTrip { get; set; }
    public IEnumerable<CountryDTO> Country { get; set; }
}

public class ClientTripDTO
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class CountryDTO
{
    public int IdCountry { get; set; }
    public string Name { get; set; }
}
