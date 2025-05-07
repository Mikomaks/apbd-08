﻿using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
    Task<List<ClientTripDTO>> GetTrip(int id);
    Task<int> AddClient(ClientDTO client);
    Task<int> AddClientToTrip(int clientID, int tripID);
    Task<int> RemoveClientFromTrip(int id, int tripID);
}