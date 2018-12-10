using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsvHelper;
using ICTTriangle.Business.Models;
using System.IO;
using CsvHelper.Configuration;

namespace ICTTriangle.Business.UnitTests
{
    [TestClass]
    public class CsvReaderTest
    {
        [TestMethod]
        public void Read_Simple_Triangle_File_Test()
        {
            
            string data = @"Product, OriginYear, DevelopmentYear, IncrementalValue
                            P1,1995,1995,100";

            List<ICTOriginalFileRecord> records = new List<ICTOriginalFileRecord>();
            //ICTOriginalFileRecord record = null;

            var config = new Configuration
            {
               HasHeaderRecord = true,
               AllowComments = true,
               HeaderValidated = null,
               IgnoreBlankLines = true,
               BadDataFound = null,
               MissingFieldFound = null,
               Delimiter = ","
            };

            using (TextReader stream = new StringReader(data))
            {
                using (var csvReader = new CsvReader(stream, config))
                {
                    while (csvReader.Read())
                    {
                        ICTOriginalFileRecord ictRecord = GetFileRecord(csvReader);
                        if (IsValid(ictRecord))
                            records.Add(ictRecord);
                    }
                }  
            }

           
            ICTOriginalFileRecord record = records[0];

            Assert.IsNotNull(record);
            Assert.AreEqual(record.Product, "P1");
            Assert.AreEqual(record.OriginYear, 1995);
            Assert.AreEqual(record.DevelopmentYear, 1995);
            Assert.AreEqual(record.IncrementalValue, 100);
        }

        private ICTOriginalFileRecord GetFileRecord(CsvReader csvReader)
        {
            if(csvReader == null) return null;

            ICTOriginalFileRecord record = null;

            try
            {
                record = new ICTOriginalFileRecord
                {
                    Product = csvReader.GetField<string>(0)?.Trim(),
                    OriginYear = csvReader.GetField<int>(1),
                    DevelopmentYear = csvReader.GetField<int>(2),
                    IncrementalValue = csvReader.GetField<int>(3),
                };
            }
            catch
            {
            }

            return record;
        }

        protected bool IsValid(ICTOriginalFileRecord record)
        {
            if (record == null) return false;

            if (record.DevelopmentYear <= 1900 &&
                record.DevelopmentYear > 10000)
                return false;

            if (record.OriginYear <= 1900 &&
               record.OriginYear > 10000)
                return false;

            if (record.IncrementalValue < 0)
                return false;

            return true;
        }
    }
}
