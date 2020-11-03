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
using System.Security.Cryptography;

namespace UnoContoso.Repository.Sql
{
    /// <summary>
    /// Contains methods for interacting with the orders backend using 
    /// SQL via Entity Framework Core 2.0.
    /// </summary>
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly ContosoContext _db; 

        public SqlOrderRepository(ContosoContext db) => _db = db;

        public async Task<IEnumerable<Order>> GetAsync() =>
            await _db.Orders
                .Include(order => order.LineItems)
                .ThenInclude(lineItem => lineItem.Product)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Order> GetAsync(Guid id) =>
            await _db.Orders
                .Include(order => order.LineItems)
                .ThenInclude(lineItem => lineItem.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id);

        public async Task<IEnumerable<Order>> GetForCustomerAsync(Guid id)
        {
            var orders = await _db.Orders
                            .Where(order => order.CustomerId == id)
                            .Include(order => order.LineItems)
                            .ThenInclude(lineItem => lineItem.Product)
                            .AsNoTracking()
                            .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetAsync(string value)
        {
            try
            {
                //Queries that are too complex will cause an Entity Framework error.
                return await _db.Orders
                    .Include(order => order.Customer)
                    .Include(order => order.LineItems)
                    .ThenInclude(lineItem => lineItem.Product)
                    .Where(order => 
                            order.Address.ToLower().StartsWith(value) ||
                            order.Customer.FirstName.ToLower().StartsWith(value) ||
                            order.Customer.LastName.ToLower().StartsWith(value))
                    .OrderByDescending(order => order.InvoiceNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Order> UpsertAsync(Order order)
        {
            var existing = await _db.Orders
                                .Where(_order => _order.Id == order.Id)
                                .Include(_order => _order.LineItems)
                                .FirstOrDefaultAsync();
            if (existing == null)
            {
                order.InvoiceNumber = _db.Orders.Max(_order => _order.InvoiceNumber) + 1;
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(order);

                //remove lineitem
                var remove = (from line1 in existing.LineItems
                              join line2 in order.LineItems
                              on line1.Id equals line2.Id into joiner
                              from line in joiner.DefaultIfEmpty()
                              where line == null
                              select line1).ToList();
                if(remove.Any())
                {
                    remove.ForEach(r => existing.LineItems.Remove(r));
                }
                //add lineitems
                var add = (from line1 in order.LineItems
                           join line2 in existing.LineItems
                           on line1.Id equals line2.Id into joiner
                           from line in joiner.DefaultIfEmpty()
                           where line == null
                           select line1).ToList();
                if(add.Any())
                {
                    add.ForEach(a => existing.LineItems.Add(a));
                }

                await _db.SaveChangesAsync();
            }
            return order;
        }

        public async Task DeleteAsync(Guid orderId)
        {
            var match = await _db.Orders.FindAsync(orderId);
            if (match != null)
            {
                _db.Orders.Remove(match);
            }
            await _db.SaveChangesAsync();
        }
    }
}
