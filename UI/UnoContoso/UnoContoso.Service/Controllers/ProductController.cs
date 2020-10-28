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
    public class ProductController : ControllerBase
    {
        private IProductRepository _repository;

        public ProductController(IProductRepository productRepository)
        {
            _repository = productRepository;
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
            var product = await _repository.GetAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(product);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest();
            }
            var products = await _repository.GetAsync(value);
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        //// POST api/<CustomerController>
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody]Product product)
        //{
        //    return Ok(await _repository.UpsertAsync(product));
        //}

        //// DELETE api/<CustomerController>/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    await _repository.DeleteAsync(id);
        //    return Ok();
        //}
    }
}
