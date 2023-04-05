using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.VisualBasic;
using Odev.Model;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;

namespace Odev
{
    public partial class Form1 : Form
    {
        NorthwndContext _db;
        public Form1()
        {
            InitializeComponent();
            _db = new NorthwndContext();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn1_Click(object sender, EventArgs e)
        {
            //1-) Fiyat�20 ile 50 aras�nda olan �r�nlerimin, ID, �r�n Ad�, Fiyat�, Stok Miktar�ve Kategori Ad�bilgilerini listeleyiniz.
            var Tables = _db.Orders.Join(
                _db.OrderDetails,
                orders => orders.OrderId,
                orderDetails => orderDetails.OrderId,
                (_order, _orderDetails) => new
                {
                    _order.OrderId,
                    _orderDetails.UnitPrice,
                    _orderDetails.ProductId,


                }).Join(
                _db.Products,
                orderDetails => orderDetails.ProductId,
                products => products.ProductId,
                (_orderDetails, _products) => new
                {
                    _orderDetails.OrderId,
                    _orderDetails.UnitPrice,
                    _products.CategoryId,
                    _products.ProductId,
                    _products.ProductName,
                    _products.UnitsInStock
                }).Join(
                _db.Categories,
                products => products.CategoryId,
                categories => categories.CategoryId,
                (_products, _categories) => new
                {
                    _products.CategoryId,
                    _products.ProductId,
                    _products.ProductName,
                    _products.UnitsInStock,
                    _products.UnitPrice,
                    _products.OrderId,
                    _categories.CategoryName,

                }).Where(od => od.UnitPrice >= 20 && od.UnitPrice <= 50).Select(r => new

                {
                    ProductID = r.ProductId,
                    ProductName = r.ProductName,
                    UnitPrice = r.UnitPrice,
                    UnitsInStock = r.UnitsInStock,
                    CategoryName = r.CategoryName
                }).Distinct().ToList();

            dgwResult.DataSource = Tables;
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            //2-) Sipari�ler tablosundan kolon isimleri M�steriSirketAdi, CalisanAdiSoyadi, SiparisID, SiparisTarihi ve KargoSirketAdi olacak �ekilde verileri listeleyiniz.
            var Tables = _db.Orders.Join(
               _db.Employees,
               orders => orders.EmployeeId,
               employees => employees.EmployeeId,
               (_orders, _employees) => new
               {
                   _orders.OrderId,
                   _employees.FirstName,
                   _employees.LastName,
                   _orders.OrderDate,
                   _orders.ShipVia,
                   _orders.CustomerId
               }).Join(
               _db.Shippers,
               orders => orders.ShipVia,
               shippers => shippers.ShipperId,
               (_orders, _shippers) => new
               {
                   _orders.OrderId,
                   _orders.FirstName,
                   _orders.LastName,
                   _orders.OrderDate,
                   _orders.ShipVia,
                   _shippers.CompanyName,
                   _orders.CustomerId
               }).Join(
                _db.Customers,
                orders => orders.CustomerId,
                customers => customers.CustomerId,
                (_orders, _customers) => new
                {
                    _orders.OrderId,
                    _orders.FirstName,
                    _orders.LastName,
                    _orders.OrderDate,
                    _orders.ShipVia,
                    ShippersCompanyName = _orders.CompanyName,
                    _orders.CustomerId,
                    CustomersCompanyName = _customers.CompanyName
                }).Select(r => new
                {
                    CustomersCompanyName = r.CustomersCompanyName,
                    EmployeeFullName = r.FirstName + " " + r.FirstName,
                    OrderID = r.OrderId,
                    OrderDate = r.OrderDate,
                    ShippersCompanyName = r.ShippersCompanyName,
                }).ToList();

            dgwResult.DataSource = Tables;
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            //3-) �irket ad�nda restaurant ge�en m��terileri listeleyiniz...
            var tables = _db.Customers.Where(x => x.CompanyName.Contains("restaurant")).ToList();
            dgwResult.DataSource = tables;
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            //4-) Beverages kategorisine sahip olan ve �r�n ad�Kola, fiyat�5, stok miktar�500 olan bir �r�n ekleyiniz...
            Product product = new Product();
            product.ProductName = "Cola";
            product.CategoryId = 1;
            product.UnitPrice = 5;
            product.UnitsInStock = 500;

            _db.Products.Add(product);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            //5-) �al��anlar�n ad�n�, soyad�n�, do�um tarihini ve y�l baz�nda ya��n�listeleyiniz...
            var tables = _db.Employees.Select(r => new
            {
                EmployeeFirstName = r.FirstName,
                EmployeeLastName = r.LastName,
                EmployeeBirthDate = r.BirthDate,
                EmployeeAge = DateTime.Today.Year - r.BirthDate.Value.Year
            }).ToList();
            dgwResult.DataSource = tables;
        }
        private void btn6_Click(object sender, EventArgs e)
        {
            //6-) Her bir kategorinin stoktaki toplam �r�n miktar�n� listeleyiniz...
            var table = _db.Products.GroupBy(x => x.CategoryId).Select(s => new
            {
                CategoryName = s.First().Category.CategoryName,
                TotalUnitsInStock = s.Count()


            }).ToList();
            dgwResult.DataSource = table;
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            //7-) Product lar��nce Stock say�s�na g�re sonra ProductName e g�re b�y�kten k����e s�ralay�n
            var tables = _db.Products.OrderByDescending(x => x.UnitsInStock);
            dgwResult.DataSource = tables;
            tables = _db.Products.OrderByDescending(x => x.ProductName);
            dgwResult.DataSource = tables;
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            //8-) Order detaillar'da order baz�nda toplam sat�� tutar� 500 ve alt�nda olan orderid'leri getir, toplam tutara g�re s�rala
            var tables = _db.OrderDetails.GroupBy(x => x.OrderId).Select(s => new
            {
                OrderID = s.Key,
                EarnedSellingPrice = s.Sum(x => x.UnitPrice * x.Quantity)
            }
            ).Where(x => x.EarnedSellingPrice <= 500).OrderBy(x=>x.EarnedSellingPrice).ToList();
            dgwResult.DataSource = tables;
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            //9-) T�m product lar�category leri ile birlikte listele
            var tables = _db.Products.Select(s => new
            {
                ProductID = s.ProductId,
                CategoryID = s.CategoryId
            }).ToList();
            dgwResult.DataSource = tables;
        }

        private void btn10_Click(object sender, EventArgs e)
        {
            //10-) ProductName, CategoryName, Suplier CompanyName ile birlikte yazd�r�n
            var tables = _db.Products.Select(s => new
            {
                ProductName = s.ProductName,
                CategoryName = s.Category.CategoryName,
                SupplierName = s.Supplier.CompanyName

            }).ToList();
            dgwResult.DataSource = tables;
        }
    }
}