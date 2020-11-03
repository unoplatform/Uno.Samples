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
    public class OrderController : ControllerBase
    {
        private IOrderRepository _repository;

        public OrderController(IOrderRepository orderRepository)
        {
            _repository = orderRepository;
        }

        // GET: api/<OrderController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repository.GetAsync());
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if(id == Guid.Empty)
            {
                return BadRequest();
            }
            var order = await _repository.GetAsync(id);
            if(order == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(order);
            }
        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerOrders(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            var orders = await _repository.GetForCustomerAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(orders);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest();
            }
            var orders = await _repository.GetAsync(value);
            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Order order)
        {
            return Ok(await _repository.UpsertAsync(order));
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}
