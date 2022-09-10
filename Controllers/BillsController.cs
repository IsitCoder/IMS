using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;
using IMS.Models.DTO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Humanizer;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Security.Claims;

namespace IMS.Controllers
{
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bill.Include(b => b.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.Bill == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }



        // GET: Bills/Create

        public async Task<IActionResult> Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int SupplierId, DateTimeOffset BillCreateDate, DateTimeOffset startdate, DateTimeOffset enddate)
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            if (enddate > startdate)
            {
             
                List<int> query = (from po in _context.PurchaseOrder.Include(b => b.Supplier)
                             where po.SupplierId == SupplierId && (po.OrderDate >= startdate && po.OrderDate <= enddate) && po.BillId == null
                             select po.PurchaseOrderId).ToList();

                DTOBillSearchPurchase purchaseSearch = new DTOBillSearchPurchase();
                purchaseSearch.SupplierId = SupplierId;
                purchaseSearch.BillCreateDate = BillCreateDate;
                purchaseSearch.startdate = startdate;
                purchaseSearch.enddate = enddate;

                if (query.Count>0) {

                    return RedirectToAction("CreateBill", purchaseSearch);
                }else
                {
                    string errmsg = "There are no purchase orders for this Supplier!";
                    ViewData["msg"] = errmsg;
                    return View(purchaseSearch);
                }
            }else
            {
                ViewData["msg"] = "ERROR: The End date is greater than Start date, Cannot filter the purchase order!";
                DTOBillSearchPurchase purchaseSearch = new DTOBillSearchPurchase();
                purchaseSearch.SupplierId = SupplierId;
                purchaseSearch.BillCreateDate =  BillCreateDate;
                purchaseSearch.startdate = startdate;
                purchaseSearch.enddate = enddate;
                return View(purchaseSearch);
            }
        }


        public async Task<IActionResult> CreateBill(DTOBillSearchPurchase purchaseSearch)
        {
            if (purchaseSearch == null)
                return NotFound();
            List<PurchaseOrder> purchases = new List<PurchaseOrder>();
            purchases = (from po in _context.PurchaseOrder.Include(b => b.Supplier).Include(b => b.Branch).Include(b => b.Product).Include(p => p.Supplier)
                         where po.SupplierId == purchaseSearch.SupplierId &&         (po.OrderDate >= purchaseSearch.startdate && po.OrderDate <=       purchaseSearch.enddate) && po.BillId == null
                         select po).ToList();
            

            Bill bill = new Bill();
            int billnumber = (from b in _context.Bill.Include(b=>b.Supplier)
                             where b.SupplierId==purchaseSearch.SupplierId
                             select b).Count();
            bill.SupplierId = purchaseSearch.SupplierId;
            bill.BillCreateDate = purchaseSearch.BillCreateDate;
            string date = purchaseSearch.BillCreateDate.ToString("yyyMMdd");
            string lastnumber = billnumber.ToString("D" + 4);
            bill.BillCode = String.Format("Bil{0}-{1}-{2}", date, purchaseSearch.SupplierId,lastnumber);
            ViewData["bill"]=bill;
            return View(purchases);
        }




        //POST: Bills/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBill(DateTimeOffset BillCreateDate,string BillCode,int SupplierId, int[] GenerateBill)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (GenerateBill == null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                Bill bill = new Bill();
                bill.BillCode = BillCode;
                bill.BillCreateDate = BillCreateDate;
                bill.SupplierId = SupplierId;
                bill.AdminId = user;
                _context.Add(bill);


                List<PurchaseOrder> purchases = new List<PurchaseOrder>();
                
                await _context.SaveChangesAsync();
                for (int i = 0; i < GenerateBill.Length; i++)
                {

                    purchases.Add(_context.PurchaseOrder.Find(GenerateBill[i]));
                }

                purchases.ForEach(a => a.BillId = bill.BillId);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Bill == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", bill.SupplierId);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,BillCode,AdminId,SupplierId")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
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
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierName", bill.SupplierId);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Bill == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bill == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bill'  is null.");
            }
            var bill = await _context.Bill.FindAsync(id);
            if (bill != null)
            {
                _context.Bill.Remove(bill);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
          return (_context.Bill?.Any(e => e.BillId == id)).GetValueOrDefault();
        }
    }
}
