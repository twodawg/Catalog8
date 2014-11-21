using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog1.Models;
using System.Collections.Generic;
using System.Net;

namespace Catalog1.Controllers
{
    public class ProductController : Controller
    {
        WidgetModel db = new WidgetModel();

        // GET: Product
        [SiteMap]
        public ActionResult Index()
        {
            ViewBag.UserName = "Lord User";

            var products = db.Products.Include("Reviews").Take(10);

            foreach (Product product in products)
            {
                product.Price -= Decimal.Round(product.Price * product.PriceModifier, 2);
            }

            return View(products);
        }
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //RedirectToAction("Index");
            }
            Product product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [Authorize]
        public ActionResult ShoppingCart(int? productID, int amount = 0)
        {
            Cart userShoppingCart;

            Repository repository = new Repository(db);

            userShoppingCart = repository.FindOrCreateCart(User.Identity.Name);

            // Add a Shopping Cart Item (CartItems)
            repository.AddShoppingCart(productID, amount, userShoppingCart);
            
            //ViewBag.PriceTotal = userShoppingCart.PriceTotal;
            
            return View(userShoppingCart);
        }

        

        

        //[HttpGet]
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}