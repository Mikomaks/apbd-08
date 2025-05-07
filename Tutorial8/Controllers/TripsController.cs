using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        [HttpGet("trips")]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }

        [HttpGet("clients/{id}/trips")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trips = await _tripsService.GetTrip(id);
            return Ok(trips);
        }

        [HttpPost("clients")]
        public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
        {
            if (!client.Email.Contains("@"))
            {
                return BadRequest("Email zły");
            }

            if (client.Telephone.Length != 9)
            {
                return BadRequest("Zły numer");
            }

            if (client.Pesel.Length != 11)
            {
                return BadRequest("Zły pesel");
            }

            //pola czy sa
            if (string.IsNullOrEmpty(client.FirstName) || string.IsNullOrEmpty(client.LastName) ||
                string.IsNullOrEmpty(client.Email) || string.IsNullOrEmpty(client.Telephone) ||
                string.IsNullOrEmpty(client.Pesel))
            {
                return BadRequest("Brak wymaganej ktorejs danej");
            }


            var status = await _tripsService.AddClient(client);
            if (status != 0)
            {
                return Ok($"Udało sie dodać clienta: {status}");
            }

            if (status == 0)
            {
                return BadRequest("Client istnieje w bazie");
            }

            return BadRequest();
        }


        [HttpPut("clients/{id}/trips/{tripID}")]
        public async Task<IActionResult> AddClientToTrip(int id, int tripID)
        {
            var status = await _tripsService.AddClientToTrip(id, tripID);

            if (status == 0)
            {
                return Ok($"Dodałem clienta {id} do wycieczki o ID: {tripID}");
            }

            if (status == 1)
            {
                return BadRequest("Client nie istnieje!");
            }

            if (status == 2)
            {
                return BadRequest("Wycieczka nie istnieje!");
            }

            if (status == 3)
            {
                return BadRequest("Przekroczono limit użytkowników!");
            }

            return BadRequest();
        }


        [HttpDelete("clients/{id}/trips/{tripId}")]
        public async Task<IActionResult> RemoveClientFromTrip(int id, int tripID)
        {
            var wynik = await _tripsService.RemoveClientFromTrip(id, tripID);

            if (wynik == 1)
            {
                return BadRequest("Taki client na takiej wycieczce nie istnieje!");
            }
            if (wynik == 2)
            {
                return Ok($"Usunalem clienta: {id} z wycieczki: {tripID}");
            }

            return BadRequest();
        }
    }
}
