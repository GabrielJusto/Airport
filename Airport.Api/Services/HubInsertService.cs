

using Airport.Api.DTOs;
using Airport.Api.Exceptions;
using Airport.Api.Models;
using Airport.Api.Repositories;
using Airport.Api.Validations;

namespace Airport.Api.Services;

public class HubInsertService(HubRepository hubRepository)
{

    private readonly HubRepository _hubRepository = hubRepository;


    public async Task<int> InsertHub(HubInsertDto hubData)
    {
        Validator validator = new([new HubCodeValidation(hubData)]);
        if(!validator.Validate())
        {
            throw new ValidationException(validator.GetErrors());
        }

        Hub hub = new(hubData);

        return await _hubRepository.InsertHub(hub);
    }



}