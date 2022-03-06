using AutoMapper;
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
    /// <summary>
    /// Version 2 of our controller to test the versioning services
    /// </summary>
   // [ApiVersion("2.0")]//The call has the same name and we need to said wich version we are working
    [ApiVersion("2.0", Deprecated = true)] //Our postman said us that is deprecated
    [Route("api/country")]//We want that in the new version de api call been the same that in the older one
    //[Route("api/{v:apiversion}/country")]//we can use this and is not necesary to use the url parameter, only the version
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private DataBaseContext _context;

        public CountryV2Controller(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(_context.Countries);
        }

    }


}
