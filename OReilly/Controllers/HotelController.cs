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
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _unitOfWork.Hotels.GetAll();
            var results = _mapper.Map<IList<HotelDTO>>(hotels);
            return Ok(results);

        }

        //[Authorize]//With this only this whose have authoritation can retrive the hotel
        [HttpGet("{id:int}", Name = "GetHotel")] //Is the same put it here or alone (like the first line). Name = is for a sibling can call with this name
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> { "Country" });
            var result = _mapper.Map<HotelDTO>(hotel);
            return Ok(result);

        }

        [Authorize(Roles = "Administrator")]//Only administrator users can create a hotel
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]//We use 200 to OK and 201 to created something is ok
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)//Its means that if the inserted data does not fit with the model (CreateHotelDTO) this is not valid
            {
                _logger.LogError($"Invalid POST attemp in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            var hotel = _mapper.Map<Hotel>(hotelDTO);
            await _unitOfWork.Hotels.Insert(hotel);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);//We return an action with the other function


        }

        //Put is for create or update something that does not exist
        //[Authorize(Roles = "Administrator")]//Only administrator users can create a hotel
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO) //Could be duplicated if we send the id in the body of the request
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attemp in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }
            var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
            if (hotel == null)
            {
                _logger.LogError($"Invalid UPDATE attemp in {nameof(UpdateHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(hotelDTO, hotel);
            _unitOfWork.Hotels.Update(hotel);
            await _unitOfWork.Save();

            return NoContent();

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attemp in {nameof(DeleteHotel)}");
                return BadRequest();
            }

            var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
            if (hotel == null)
            {
                _logger.LogError($"Invalid DELETE attemp in {nameof(DeleteHotel)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Hotels.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }

    }

}
