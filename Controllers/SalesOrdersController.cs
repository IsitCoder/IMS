using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;

namespace IMS.Controllers
{
    public class SalesOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalesOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SalesOrder.Include(s => s.Branch).Include(s => s.Customer).Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SalesOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Branch)
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: SalesOrders/Create
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName");
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName");
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName");
            return View();
        }

        // POST: SalesOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,CustomerId,BranchId,ProductId,OrderDate,DeliveryDate,Price,Quantity,Total,Remarks")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // POST: SalesOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalesOrderId,CustomerId,BranchId,ProductId,OrderDate,DeliveryDate,Price,Quantity,Total,Remarks")] SalesOrder salesOrder)
        {
            if (id != salesOrder.SalesOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(salesOrder.SalesOrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Branch)
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SalesOrder == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalesOrder'  is null.");
            }
            var salesOrder = await _context.SalesOrder.FindAsync(id);
            if (salesOrder != null)
            {
                _context.SalesOrder.Remove(salesOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(int id)
        {
          return (_context.SalesOrder?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
