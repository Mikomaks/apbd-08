using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    
    //1
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT Trip.IdTrip, Trip.Name,Description,DateFrom,DateTo,MaxPeople,Country.Name AS nazwa_kraju FROM Trip,Country,Country_Trip WHERE Country.IdCountry = Country_Trip.IdCountry AND Trip.IdTrip = Country_Trip.IdCountry";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");

                    var trip = new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        Countries = new List<CountryDTO>()
                    };
                    
                    trips.Add(trip);
                    
                    var countryName = reader.GetString(reader.GetOrdinal("nazwa_kraju"));
                    trip.Countries.Add(new CountryDTO()
                    {
                        Name = countryName
                    });
                    
                    

                }
            }
        }
        

        return trips;
    }

    
    //2
    public async Task<List<ClientTripDTO>> GetTrip(int id)
    {
        var trips = new List<ClientTripDTO>();

        string command = $"SELECT Trip.IdTrip,Name,Description,MaxPeople,RegisteredAt,PaymentDate FROM Client,Trip,Client_Trip WHERE Trip.IdTrip = Client_Trip.IdTrip AND Client.IdClient = Client_Trip.IdClient AND Client.IdClient = {id}" ;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var trip = new ClientTripDTO()
                    {
                        IdTrip = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? null : reader.GetInt32(reader.GetOrdinal("PaymentDate"))
                    };
                    trips.Add(trip);

                }
            }
        }

        return trips;
    }
    
    
    //3
    public async Task<int> AddClient(ClientDTO client)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var command = new SqlCommand("IF NOT EXISTS(SELECT * FROM Client WHERE Pesel = @Pesel)\nBEGIN\n    INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)\n OUTPUT INSERTED.IdClient   VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)\nEND", conn);
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Telephone", client.Telephone);
        command.Parameters.AddWithValue("@Pesel", client.Pesel);


        var wynik = await command.ExecuteScalarAsync();

        if (wynik != null)
        {
            return Convert.ToInt32(wynik);
        }
        else
        {
            return 0;
        }


    }
    
    
    //4
        public async Task<int> AddClientToTrip(int clientID, int tripID)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var command = new SqlCommand("BEGIN \n    --client nie istnieje\n    IF NOT EXISTS(SELECT * FROM Client WHERE IdClient = @client_id)\n    BEGIN \n        SELECT 1;\n    end\n    \n    --wycieczka nie istnieje\n    IF NOT EXISTS(SELECT * FROM Trip WHERE Trip.IdTrip = @trip_id) BEGIN \n        SELECT 2;\n    end\n    \n    --maks uzytkownikow\n    DECLARE @usercount integer;\n    DECLARE @today VARCHAR(8);\n    SET @today = CONVERT(VARCHAR(8), GETDATE(), 112);\n    \n    SET @usercount = (SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @trip_id)\n    \n    IF @usercount + 1 > (SELECT trip.MaxPeople FROM Trip where IdTrip = @trip_id) BEGIN \n        SELECT 3;\n        END \n    ELSE\n        BEGIN \n        INSERT INTO Client_Trip(idclient, idtrip,RegisteredAt)\n        VALUES (@client_id,@trip_id,@today);\n    end\nend",conn);
            
            command.Parameters.AddWithValue("@trip_id", tripID);
            command.Parameters.AddWithValue("@client_id", clientID);
            
            var wynik = await command.ExecuteScalarAsync();

            if (wynik != null)
            {
                return Convert.ToInt32(wynik);
            }
            else
            {
                return 0;
            }

        }
        
    //5
    public async Task<int> RemoveClientFromTrip(int id, int tripID)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var command = new SqlCommand("BEGIN \n   IF NOT EXISTS(SELECT * FROM Client_Trip WHERE IdTrip = @trip_id AND IdClient = @client_id)\n    BEGIN \n        SELECT 1;\n    end\n    \n    DELETE FROM Client_Trip\n    WHERE IdClient = @client_id AND IdTrip = @trip_id;\n   \n   SELECT 2;\n    \n    \nend",conn);
        
        
        command.Parameters.AddWithValue("@client_id", id);
        command.Parameters.AddWithValue("@trip_id", tripID);
        
        var wynik = await command.ExecuteScalarAsync();
        
        if (wynik != null)
        {
            return Convert.ToInt32(wynik);
        }
        else
        {
            return 0;
        }
    }
}
