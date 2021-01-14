﻿using System;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class Map
    {
        public Map()
        {
            if (Bin.ProductMapCount <= 0)
            {
                using SqlConnection con = new SqlConnection(Global.ConnectionString);
                con.Open();
                Bin.ProductMapCount = Bin.GetDataRequestsBinColumns(con);
            }

            ProductMap = new uint[Bin.ProductMapCount];
            ProductMapOld = new uint[Bin.ProductMapCount];
        }

        public uint[] ProductMap { get; set; }
        public uint[] ProductMapOld { get; set; }

        public uint LengthMap { get; set; }

        public static Map SetLengthMap(Map map, SqlDataReader ReaderLengths)
        {
            map.LengthMap |= Convert.ToUInt32(Math.Pow(2, Global.GetValue<int>(ReaderLengths, "LengthID")));
            return map;
        }

        public static void SetProductMap(Map map, SqlDataReader ReaderProducts)
        {
            int ProdID = Global.GetValue<int>(ReaderProducts, "ProdID");
            map.ProductMap[ProdID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(ProdID.ToString()) - (32 * (ProdID / 32))));
        }

        public static uint ZeroProductLengthMap(int ProductMapCount, Map map)
        {
            for (int index = 0; index < ProductMapCount; index++)
            {
                map.ProductMap[index] = 0;
                map.ProductMapOld[index] = 0;
            }

            return 0;
        }

        public static void GetDBProductMapBin(SqlConnection con, Bin Item, Map map)
        {
            // Product Map
            ProductLengths PL = new ProductLengths();
            using (SqlCommand cmd = new SqlCommand(ProductLengths.BinProdMapSql, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    try
                    {
                        PL.PopulateProductList(reader);

                        // Use only selected
                        foreach (ProductLengths.Products P in PL.ProductsList.Where(p => p.Selected).ToList())
                        {
                            map.ProductMap[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                            map.ProductMapOld[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        throw;
                    }
                }
            }

            // Length Map
            using (SqlCommand cmd = new SqlCommand(ProductLengths.BinLengthMapSql, con))
            {
                cmd.Parameters.AddWithValue("@BinID", Item.BinID);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    try
                    {
                        PL.PopulateLengthList(reader);

                        // Use only selected
                        foreach (ProductLengths.Lengths L in PL.LengthsList.Where(l => l.Selected).ToList())
                        {
                            map.LengthMap |= Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.LogError(ex);
                        throw;
                    }
                }
            }
        }

        public static void GetSelectedProductMapBin(Bin Item, Map map)
        {
            ProductLengths SelectedProductLengths = new ProductLengths()
            {
                ProductsList = Item.ProdLen.ProductsList.Where(p => p.Selected).ToList(),
                LengthsList = Item.ProdLen.LengthsList.Where(l => l.Selected).ToList()
            };

            // Product Map New
            try
            {
                foreach (ProductLengths.Products P in SelectedProductLengths.ProductsList)
                {
                    map.ProductMap[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                }

                // Length Map
                foreach (ProductLengths.Lengths L in SelectedProductLengths.LengthsList)
                {
                    map.LengthMap |= Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(L.ID)));
                }
            }
            catch (Exception ex)
            {
                Global.LogError(ex);
                throw;
            }
        }

        public static void GetProductMapOldBin(SqlConnection con, Bin Item, Map map)
        {
            ProductLengths PL = new ProductLengths();

            using SqlCommand cmd = new SqlCommand(ProductLengths.BinProdMapSql, con);
            cmd.Parameters.AddWithValue("@BinID", Item.SortID);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                try
                {
                    PL.PopulateProductList(reader);

                    // Use only selected
                    foreach (ProductLengths.Products P in PL.ProductsList.Where(p => p.Selected).ToList())
                    {
                        map.ProductMapOld[P.ID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(P.ID.ToString()) - (32 * (P.ID / 32))));
                    }
                }
                catch (Exception ex)
                {
                    Global.LogError(ex);
                    throw;
                }
            }
        }

        public static void GetProductLengthMapDBSort(SqlConnection con, int SortID, Map map, int RecipeID)
        {
            using (SqlCommand cmdInner = new SqlCommand($"select ProdID from sortproducts where sortID = {SortID} and recipeid= {RecipeID}", con))
            using (SqlDataReader readerSortProducts = cmdInner.ExecuteReader())
            {
                if (readerSortProducts.HasRows)
                {
                    while (readerSortProducts.Read())
                    {
                        try
                        {
                            SetProductMap(map, readerSortProducts);
                        }
                        catch (Exception ex)
                        {
                            Global.LogError(ex);
                            throw;
                        }
                    }
                }
            }

            using (SqlCommand cmdInner = new SqlCommand($"select LengthID from sortlengths where sortID = {SortID} and recipeid= {RecipeID}", con))
            using (SqlDataReader readerSortLengths = cmdInner.ExecuteReader())
            {
                if (readerSortLengths.HasRows)
                {
                    while (readerSortLengths.Read())
                    {
                        try
                        {
                            SetLengthMap(map, readerSortLengths);
                        }
                        catch (Exception ex)
                        {
                            Global.LogError(ex);
                            throw;
                        }
                    }
                }
            }
        }
    }
}