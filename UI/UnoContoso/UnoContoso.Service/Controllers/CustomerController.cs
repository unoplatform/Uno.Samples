using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UnoContoso.Models;
using UnoContoso.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UnoContoso.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomerRepository _repository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _repository = customerRepository;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repository.GetAsync());
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if(id == Guid.Empty)
            {
                return BadRequest();
            }
            var customer = await _repository.GetAsync(id);
            if(customer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(customer);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest();
            }
            var customers = await _repository.GetAsync(value);
            if (customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Customer customer)
        {
            return Ok(await _repository.UpsertAsync(customer));
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}
