using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SharedModel.Models.General;
using System.Collections.Concurrent;

namespace Tools
{
    public class AfipPadronHelper
    {
        public DataTable GenerateRows(string inputFile)
        {
            if (!System.IO.File.Exists(inputFile))
            {
                throw new Exception("File was not found: " + inputFile);
            }
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ProcessDate", typeof(long));
            dataTable.Columns.Add("CUIT", typeof(long));
            dataTable.Columns.Add("Alias");
            dataTable.Columns.Add("IncomeTax");
            dataTable.Columns.Add("VatTax");
            dataTable.Columns.Add("MonoTax");
            dataTable.Columns.Add("SocialMember");
            dataTable.Columns.Add("Employer");
            dataTable.Columns.Add("MonoTaxActivity");
            dataTable.Columns.Add("Active", typeof(short));


            ConcurrentBag<RegisteredEntityAfip> concurrentBag = new ConcurrentBag<RegisteredEntityAfip>();
            Parallel.ForEach(System.IO.File.ReadLines(inputFile, Encoding.Default), (line, _, lineNumber) =>
            {
                if(line.Length == 51) {
                    concurrentBag.Add(new RegisteredEntityAfip
                    {
                        Cuit = long.Parse(line.Substring(0, 11)),
                        Alias = line.Substring(11, 30),
                        IncomeTax = line.Substring(41, 2),
                        VatTax = line.Substring(43, 2),
                        MonoTax = line.Substring(45, 2),
                        SocialMember = line.Substring(47, 1),
                        Employer = line.Substring(48, 1),
                        MonoTaxActivity = line.Substring(49, 2),
                        Active = 1
                    });
                }
            });
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            while(concurrentBag.Count > 0)
            {
                if(concurrentBag.TryTake(out RegisteredEntityAfip entity))
                {
                    dataTable.Rows.Add(
                        unixTimestamp,
                        entity.Cuit,
                        entity.Alias, 
                        entity.IncomeTax, 
                        entity.VatTax, 
                        entity.MonoTax, 
                        entity.SocialMember, 
                        entity.Employer, 
                        entity.MonoTaxActivity, 
                        entity.Active
                    );
                }
            }

            return dataTable;
        }
    }
}
