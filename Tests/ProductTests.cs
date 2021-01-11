using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebSort;
using WebSort.Model;

namespace Tests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod, TestCategory("Get")]
        public void GetAtID()
        {
            Product prod = Product.GetAtID(1);
            Assert.IsNotNull(prod);
        }

        [TestMethod, TestCategory("Get")]
        public void GetAll()
        {
            IEnumerable<Product> products = Product.GetAll();

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());
        }

        [TestMethod, TestCategory("Get")]
        public void GetAllByThick()
        {
            Product product = Product.GetAtID(1);
            Thickness thick = Thickness.GetAtID(product.ThicknessID);
            Assert.IsNotNull(thick);

            IEnumerable<Product> products = Product.GetAllByThick(thick);

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());
        }

        [TestMethod, TestCategory("Get")]
        public void GetAllByWidth()
        {
            Product product = Product.GetAtID(1);
            Width width = Width.GetAtID(product.WidthID);
            Assert.IsNotNull(width);

            IEnumerable<Product> products = Product.GetAllByWidth(width);

            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());
        }

        [TestMethod, TestCategory("CRUD")]
        public void CRUD()
        {
            Product testProd = new Product()
            {
                Deleted = 0,
                Active = true,
                ProdLabel = "TestProduct",
                GradeID = 0,
                MoistureID = 0,
                SpecID = 0
            };

            // Create
            int ProdID = Product.Insert(testProd);
            Assert.IsTrue(ProdID > 0);
            Assert.IsTrue(testProd.ProdID > 0);

            // Read
            testProd = Product.GetAtID(ProdID);

            // Update
            testProd.ProdLabel = "Test Product";
            Product.Save(testProd);
            testProd = Product.GetAtID(ProdID);
            Assert.AreEqual("Test Product", testProd.ProdLabel);

            // Delete
            Product.Delete(testProd);
            testProd = Product.GetAtID(ProdID);
            Assert.IsNull(testProd);
        }

        [TestMethod, TestCategory("CRUD")]
        public void UpdateAtThickness()
        {
            Product product = Product.GetAtID(1);
            Thickness thick = Thickness.GetAtID(product.ThicknessID);

            float oldNom = thick.Nominal;
            thick.Nominal = -1;

            Product.UpdateAtThickness(thick);
            IEnumerable<Product> products = Product.GetAllByThick(thick);
            Assert.IsTrue(products.Any());
            Assert.IsTrue(products.First().ThickNominal == -1);

            thick.Nominal = oldNom;
            Product.UpdateAtThickness(thick);
            products = Product.GetAllByThick(thick);
            Assert.IsTrue(products.Any());
            Assert.IsTrue(products.First().ThickNominal == oldNom);

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ThickNominal = -1", con))
                    Assert.IsTrue((int)cmd.ExecuteScalar() == 0);
            }
        }

        [TestMethod, TestCategory("CRUD")]
        public void UpdateAtWidth()
        {
            Product product = Product.GetAtID(1);
            Width width = Width.GetAtID(product.WidthID);

            float oldNom = width.Nominal;
            width.Nominal = -1;

            Product.UpdateAtWidth(width);
            IEnumerable<Product> products = Product.GetAllByWidth(width);
            Assert.IsTrue(products.Any());
            Assert.IsTrue(products.First().WidthNominal == -1);

            width.Nominal = oldNom;
            Product.UpdateAtWidth(width);
            products = Product.GetAllByWidth(width);
            Assert.IsTrue(products.Any());
            Assert.IsTrue(products.First().WidthNominal == oldNom);

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE WidthNominal = -1", con))
                    Assert.IsTrue((int)cmd.ExecuteScalar() == 0);
            }
        }

        [TestMethod, TestCategory("CRUD")]
        public void DataRequestInsert()
        {
            Product prod = Product.GetAtID(1);
            Assert.IsNotNull(prod);

            using (SqlConnection con = new SqlConnection(Global.ConnectionString))
            {
                con.Open();

                bool success = Product.DataRequestInsert(con, prod, false, ZeroOut: false);
                Assert.IsFalse(success);

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM DataRequestsProduct WHERE Processed = 0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Assert.IsTrue(reader.HasRows);
                    while (reader.Read())
                    {
                        Assert.AreEqual(prod.ProdID, Convert.ToInt32(reader["ProductID"].ToString()));
                        Assert.AreEqual(prod.ThicknessID, Convert.ToInt32(reader["ThicknessID"].ToString()));
                        Assert.AreEqual(prod.WidthID, Convert.ToInt32(reader["WidthID"].ToString()));
                        Assert.AreEqual(prod.GradeID, Convert.ToInt32(reader["GradeID"].ToString()));
                    }
                }

                using (SqlCommand cmd = new SqlCommand("DELETE FROM DataRequestsProduct WHERE Processed = 0", con))
                    Assert.IsTrue(cmd.ExecuteNonQuery() > 0);

                success = Product.DataRequestInsert(con, prod, false, ZeroOut: true);
                Assert.IsFalse(success);

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM DataRequestsProduct WHERE Processed = 0", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Assert.IsTrue(reader.HasRows);
                    while (reader.Read())
                    {
                        Assert.AreEqual(prod.ProdID, Convert.ToInt32(reader["ProductID"].ToString()));
                        Assert.AreEqual(0, Convert.ToInt32(reader["ThicknessID"].ToString()));
                        Assert.AreEqual(0, Convert.ToInt32(reader["WidthID"].ToString()));
                        Assert.AreEqual(0, Convert.ToInt32(reader["GradeID"].ToString()));
                    }
                }

                using (SqlCommand cmd = new SqlCommand("DELETE FROM DataRequestsProduct WHERE Processed = 0", con))
                    Assert.IsTrue(cmd.ExecuteNonQuery() > 0);
            }
        }
    }
}