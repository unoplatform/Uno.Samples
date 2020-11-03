//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using UnoContoso.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ImTools;

namespace UnoContoso.Repository.Sql
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using 
    /// SQL via Entity Framework Core 2.0.
    /// </summary>
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly ContosoContext _db; 

        public SqlCustomerRepository(ContosoContext db)
        {
            _db = db; 
        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
            return await _db.Customers
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Customer> GetAsync(Guid id)
        {
            return await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetAsync(string value)
        {
            value = value.ToLower();
            //Fix to search only with the first word because it cannot be complicated due to an Entity Framework error.
            string[] parameters = value.Split(' ');

            try
            {
                var customers = await _db.Customers
                    .AsNoTracking()
                    .Where(customer =>
                            customer.FirstName.ToLower().StartsWith(parameters[0]) ||
                            customer.LastName.ToLower().StartsWith(parameters[0]) ||
                            customer.Email.ToLower().StartsWith(parameters[0]) ||
                            customer.Phone.ToLower().StartsWith(parameters[0]) ||
                            customer.Company.ToLower().StartsWith(parameters[0]) ||
                            customer.Address.ToLower().StartsWith(parameters[0]))
                    .ToListAsync();
                return customers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Customer> UpsertAsync(Customer customer)
        {
            var current = await _db.Customers.FirstOrDefaultAsync(_customer => _customer.Id == customer.Id);
            if (null == current)
            {
                _db.Customers.Add(customer);
            }
            else
            {
                _db.Entry(current).CurrentValues.SetValues(customer);
            }
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(_customer => _customer.Id == id);
            if (null != customer)
            {
                var orders = await _db.Orders.Where(order => order.CustomerId == id).ToListAsync();
                _db.Orders.RemoveRange(orders);
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();
            }
        }
    }
}
