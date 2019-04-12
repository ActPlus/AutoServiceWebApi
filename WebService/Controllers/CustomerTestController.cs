using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Models;

namespace WebService.Controllers
{
    /// <summary>
    /// Only for Debug simulation of external api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTestController : ControllerBase
    {
        

        /// <summary>
        /// Only for debug
        /// </summary>
        /// <returns></returns>
        // GET: api/CustomerTest
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetOrder()
        {
            return Ok(new string[1] { "running"});
        }

        /// <summary>
        /// Simulation of validation customers from other api
        /// validapi represents customer in database of other api
        /// </summary>
        /// <param name="id">id of customer to be validated</param>
        /// <returns></returns>
        // GET: api/CustomerTest/5
        [HttpGet("{id}")]

        [Produces("application/json")]
        public ActionResult<Guid?> GetOrder(Guid? id)
        {
            if (id != null)
            {
                string validId = "259806db-436e-450e-b57d-b5b1b3b955ba";
                if (id.ToString().Equals(validId))
                {
                    return Ok(id);
                } else
                {
                    return NotFound(id);
                }
            }
            return BadRequest();

        }
    }
}