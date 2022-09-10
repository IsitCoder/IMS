using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.CodeAnalysis;

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
            var applicationDbContext = _context.PurchaseOrder.Include(p => p.Bill).Include(p => p.Branch).Include(p => p.Product).Include(p => p.Supplier);
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
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchId");
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductCode");
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            return View();
        }
      
        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchId,SupplierId,ProductId,Quantity,Price,Amount,OrderDate,DeliveryDate,Remarks,BillId")] PurchaseOrder purchaseOrder)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            purchaseOrder.AdminId = user;
            if (ModelState.IsValid)
            {

                _context.Add(purchaseOrder);
                var UpdateProduct = await _context.Product.FindAsync(purchaseOrder.ProductId);
                if (UpdateProduct == null)
                    return NotFound();
                UpdateProduct.CurrentQuantity += purchaseOrder.Quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchId", purchaseOrder.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductCode", purchaseOrder.ProductId);
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
            ViewData["BillId"] = new SelectList(_context.Set<Bill>(), "BillId", "BillId", purchaseOrder.BillId);
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchId", purchaseOrder.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductCode", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PurchaseOrderId,int BranchId,int SupplierId,int ProductId,int Quantity,double Price,double Amount, DateTimeOffset OrderDate, DateTimeOffset DeliveryDate,string? Remarks,string? BillId,string AdminId)
        {
            if(PurchaseOrderId == null)
                return NotFound();

            //AdminId= User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                PurchaseOrder purchase = _context.PurchaseOrder.FirstOrDefault(item => item.PurchaseOrderId == PurchaseOrderId);
                if (purchase == null)
                    return NotFound();
                if (purchase.ProductId == ProductId)
                {
                    var UpdateProduct = await _context.Product.FindAsync(ProductId);
                    if (UpdateProduct == null)
                        return NotFound();
                    if (purchase.Quantity > Quantity)
                    {
                        var decreaseQuantity = purchase.Quantity - Quantity;
                        UpdateProduct.CurrentQuantity -= decreaseQuantity;
                    }
                    if (purchase.Quantity < Quantity)
                    {
                        var increaseQuantity = Quantity - purchase.Quantity;
                        UpdateProduct.CurrentQuantity += increaseQuantity;
                    }

                }
                else
                {
                    var UpdateProduct = await _context.Product.FindAsync(purchase.ProductId);
                    UpdateProduct.CurrentQuantity-=purchase.Quantity;

                    var NewUpdateProduct = await _context.Product.FindAsync(ProductId);
                    NewUpdateProduct.CurrentQuantity += Quantity;


                }

                purchase.BranchId = BranchId;
                purchase.SupplierId = SupplierId;
                purchase.ProductId = ProductId;
                purchase.Quantity = Quantity;
                purchase.Price = Price;
                purchase.Amount = Amount;
                purchase.OrderDate = OrderDate;
                purchase.DeliveryDate = DeliveryDate;
                purchase.Remarks = Remarks;
                purchase.AdminId = AdminId;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchase.PurchaseOrderId))
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
            PurchaseOrder po = new PurchaseOrder();
            po.PurchaseOrderId = PurchaseOrderId;
            po.BranchId = BranchId;
            po.SupplierId = SupplierId;
            po.ProductId = ProductId;
            po.Quantity = Quantity;
            po.Price = Price;
            po.Amount = Amount;
            po.OrderDate = OrderDate;
            po.DeliveryDate = DeliveryDate;
            po.Remarks = Remarks;
            po.AdminId = AdminId;
            ViewData["BillId"] = new SelectList(_context.Set<Bill>(), "BillId", "BillId", po.BillId);
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchId", po.BranchId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductCode", po.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", po.SupplierId);
            return View(po);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PurchaseOrder == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Bill)
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
                var UpdateProduct = await _context.Product.FindAsync(purchaseOrder.ProductId);
                if (UpdateProduct == null)
                    return NotFound();
                {
                    UpdateProduct.CurrentQuantity -= purchaseOrder.Quantity;
                }
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
