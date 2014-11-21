using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalog1.Models
{
    public class Repository
    {
        WidgetModel db;

        public Repository(WidgetModel model)
        {
            db = model;
        }

        public Cart FindOrCreateCart(string username)
        {
            Cart userShoppingCart;
            if (GetCartByName(username)
                .Count() > 0)
            {
                userShoppingCart = GetCartByName(username)
                    .First();
            }
            else
            {
                userShoppingCart = new Cart()
                {
                    Carts = new List<CartItem>(),
                    UserName = username,
                };
                db.Carts.Add(userShoppingCart);
                db.SaveChanges();
            }
            return userShoppingCart;
        }

        public IQueryable<Cart> GetCartByName(string username)
        {
            return db.Carts.Where(q => q.UserName == username);
        }
        public void AddShoppingCart(int? productID, int amount, Cart userShoppingCart)
        {
            if (productID != null)
            {
                Product product = db.Products.Find(productID);
                product.Quantity -= amount;

                if (GetCartItemByName(userShoppingCart, product)
                    .Count() > 0)
                {
                    GetCartItemByName(userShoppingCart, product)
                        .First().Quantity += amount;
                }
                else
                {
                    var cartItem = new CartItem()
                    {
                        Name = product.Name,
                        Quantity = amount,
                        Price = product.Price
                    };
                    userShoppingCart.Carts.Add(cartItem);
                }
                db.SaveChanges();
            }
        }

        private static IEnumerable<CartItem> GetCartItemByName(Cart userShoppingCart, Product product)
        {
            return userShoppingCart.Carts.Where(q => q.Name == product.Name);
        }
    }
}