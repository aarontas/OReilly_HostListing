using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OReilly.Data;
using OReilly.IRepository;
using OReilly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)//FromQuery comes from the querystring in the url. This have to fit with our requestParams class
        {

            var countries = await _unitOfWork.Countries.GetAll(requestParams); //Override the GetAll to limited for our query
            var results = _mapper.Map<IList<CountryDTO>>(countries);//We doing a map because we want to work with the DTO and no with de domain
            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels" });
            var result = _mapper.Map<CountryDTO>(country);//We doing a map because we want to work with the DTO and no with de domain
            return Ok(result);
        }


        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO) //FromBody comes frome body and is not possible to see in the URL
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attemp in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

            var country = _mapper.Map<Country>(countryDTO);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
        }

        //[Authorize(Roles = "Administrator")]//Only administrator users can create a hotel
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attemp in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }
            var country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid UPDATE attemp in {nameof(UpdateCountry)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(countryDTO, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();

            return NoContent();

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attemp in {nameof(DeleteCountry)}");
                return BadRequest();
            }

            var country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid DELETE attemp in {nameof(DeleteCountry)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }

    }
}
