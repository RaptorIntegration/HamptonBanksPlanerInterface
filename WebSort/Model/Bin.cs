﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace WebSort.Model
{
    public class Bin
    {
        public int BinID { get; set; }
        public string BinLabel { get; set; }
        public int BinStatus { get; set; }
        public string BinStatusLabel { get; set; }
        public int BinSize { get; set; }
        public int BinCount { get; set; }
        public int BinPercent { get; set; }
        public int SortID { get; set; }
        public string ProductsLabel { get; set; }
        public bool Changed { get; set; }
        public ProductLengths ProdLen { get; set; }
        public List<Edit> EditsList { get; set; }

        public Bin()
        {
            EditsList = new List<Edit>();
        }

        public static string DataRequestBinSQL { get; set; }

        public static List<Bin> PopulateBinList(SqlDataReader reader)
        {
            List<Bin> BinList = new List<Bin>();
            while (reader.Read())
            {
                BinList.Add(new Bin
                {
                    BinID = Global.GetValue<int>(reader, "BinID"),
                    BinLabel = Global.GetValue<string>(reader, "BinLabel"),
                    BinStatus = Global.GetValue<int>(reader, "BinStatus"),
                    BinStatusLabel = Global.GetValue<string>(reader, "BinStatusLabel"),
                    BinSize = Global.GetValue<int>(reader, "BinSize"),
                    BinCount = Global.GetValue<int>(reader, "BinCount"),
                    BinPercent = Global.GetValue<int>(reader, "BinPercent"),
                    SortID = Global.GetValue<int>(reader, "SortID"),
                    ProductsLabel = Global.GetValue<string>(reader, "ProductsLabel"),
                    Changed = false
                });
            }

            return BinList;
        }

        public static uint ZeroProductLengthMap(int ProductMapCount, uint[] ProductMap, uint[] ProductMapOld)
        {
            for (int index = 0; index < ProductMapCount; index++)
            {
                ProductMap[index] = 0;
                ProductMapOld[index] = 0;
            }

            return 0;
        }

        public static uint GetProductLengthMap(uint LengthMap, SqlConnection con, uint[] ProductMap, int BinID)
        {
            using (SqlCommand cmdInner = new SqlCommand($"select ProdID from binproducts where binID = {BinID}", con))
            using (SqlDataReader ReaderBinProducts = cmdInner.ExecuteReader())
            {
                if (ReaderBinProducts.HasRows)
                {
                    while (ReaderBinProducts.Read())
                    {
                        SetProductMap(ProductMap, ReaderBinProducts);
                    }
                }
            }

            using (SqlCommand cmdInner = new SqlCommand($"select ProdID from binlengths where binID = {BinID}", con))
            using (SqlDataReader ReaderBinLengths = cmdInner.ExecuteReader())
            {
                if (ReaderBinLengths.HasRows)
                {
                    while (ReaderBinLengths.Read())
                    {
                        //length map per bin
                        LengthMap = SetLengthMap(LengthMap, ReaderBinLengths);
                    }
                }
            }

            return LengthMap;
        }

        public static uint SetLengthMap(uint LengthMap, SqlDataReader ReaderBinLengths)
        {
            LengthMap |= Convert.ToUInt32(Math.Pow(2, Global.GetValue<int>(ReaderBinLengths, "LengthID")));
            return LengthMap;
        }

        public static void SetProductMap(uint[] ProductMap, SqlDataReader ReaderBinProducts)
        {
            int ProdID = Global.GetValue<int>(ReaderBinProducts, "ProdID");
            ProductMap[ProdID / 32] |= Convert.ToUInt32(Math.Pow(2, double.Parse(ProdID.ToString()) - (32 * (ProdID / 32))));
        }

        public static void UpdateLabels(IEnumerable<Edit> edits, SqlConnection con)
        {
            if (edits.Any())
            {
                foreach (Edit edit in edits)
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Bins SET ProductsLabel = REPLACE(ProductsLabel, @Old, @New)", con))
                    {
                        cmd.Parameters.AddWithValue("@New", edit.EditedVal);
                        cmd.Parameters.AddWithValue("@Old", edit.Previous);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}