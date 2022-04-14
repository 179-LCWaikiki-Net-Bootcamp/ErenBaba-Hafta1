using ErenBaba_Hafta1.Context;
using ErenBaba_Hafta1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ErenBaba_Hafta1.Dtos;

namespace ErenBaba_Hafta1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            if (_dbContext.Products.Count() == 0)
            {
                _dbContext.Products.Add(new Product { Name = "Product1", Category = "A Kategorisi", Price = 300 });
                _dbContext.Products.Add(new Product { Name = "Product2", Category = "B Kategorisi", Price = 200 });
                _dbContext.Products.Add(new Product { Name = "Product3", Category = "A Kategorisi", Price = 400 });
                _dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] QueryDto search)
        {
            IQueryable<Product> query = _dbContext.Products;

            if (!string.IsNullOrEmpty(search.Name))
            {
                query = query.Where(x => x.Name.Contains(search.Name));
            }
            if (!string.IsNullOrEmpty(search.Category))
            {
                query = query.Where(x => x.Category == search.Category);
            }
            if (search.Price > 0)
            {
                query = query.Where(x => x.Price >= search.Price);
            }

            return await query.ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            var obj = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            if (obj != null)
            {
                obj.Name = product.Name;
                obj.Price = product.Price;
                obj.Category = product.Category;
            }
            else
            {

                _dbContext.Products.Add(new Product { Id = product.Id, Category = product.Category, Name = product.Name, Price = product.Price });
            }
            _dbContext.SaveChanges();
            return Ok(_dbContext.Products.ToList());
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(_dbContext.Products.ToList());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}