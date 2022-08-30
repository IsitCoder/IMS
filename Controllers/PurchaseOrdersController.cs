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
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PurchaseOrder.Include(p => p.Branch).Include(p => p.Product).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PurchaseOrder == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Branch)
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName");
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName");
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderId,SupplierId,BranchId,ProductId,OrderDate,DeliveryDate,Price,Quantity,Total,Remarks")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", purchaseOrder.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PurchaseOrder == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", purchaseOrder.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PurchaseOrderId,SupplierId,BranchId,ProductId,OrderDate,DeliveryDate,Price,Quantity,Total,Remarks")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.PurchaseOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.PurchaseOrderId))
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
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", purchaseOrder.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PurchaseOrder == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Branch)
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchaseOrder == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchaseOrder'  is null.");
            }
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrder.Remove(purchaseOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(int id)
        {
          return (_context.PurchaseOrder?.Any(e => e.PurchaseOrderId == id)).GetValueOrDefault();
        }
    }
}
