using System.Text;
using System.Text.RegularExpressions;
using Stocker.HBase;
using Stocker.HBase.Serialization;
using Xunit;

namespace Stocker.UnitTest
{
    public class HBaseSerializationTests
    {
        [Fact]
        public void ColumnSerialization()
        {
            var column = new HBaseColumn("family", "column");

            var json = HBaseSerializationHelper.SerializeObject(column);
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);
            
            Assert.Equal("\"ZmFtaWx5OmNvbHVtbg==\"", compactJson);
        }

        [Fact]
        public void ColumnCollectionSerialization()
        {
            var columns = new[]
            {
                new HBaseColumn("family1", "column1"),
                new HBaseColumn("family2", "column2"),
                new HBaseColumn("family3", "column3")
            };

            var json = HBaseSerializationHelper.SerializeObject(columns);
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);
            
            Assert.Equal("[\"ZmFtaWx5MTpjb2x1bW4x\",\"ZmFtaWx5Mjpjb2x1bW4y\",\"ZmFtaWx5Mzpjb2x1bW4z\"]", 
                         compactJson);
        }
        
        [Fact]
        public void CellSerialization()
        {
            var cell = new HBaseCell
            {
                Column = new HBaseColumn("family", "column"),
                Timestamp = 325,
                Data = Encoding.UTF8.GetBytes("hello world")
            };

            var json = HBaseSerializationHelper.SerializeObject(cell);
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);

            Assert.Equal("{\"column\":\"ZmFtaWx5OmNvbHVtbg==\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"}",
                         compactJson);
        }

        [Fact]
        public void CellCollectionSerialization()
        {
            var cells = new[]
            {
                new HBaseCell
                {
                    Column = new HBaseColumn("family1", "column1"),
                    Timestamp = 325,
                    Data = Encoding.UTF8.GetBytes("hello world")
                },
                new HBaseCell
                {
                    Column = new HBaseColumn("family2", "column2"),
                    Timestamp = 1015,
                    Data = Encoding.UTF8.GetBytes("goodbye world")
                }
            };

            var json = HBaseSerializationHelper.SerializeObject(cells);
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);
            
            Assert.Equal("[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]", 
                         compactJson);
        }

        [Fact]
        public void RowSerialization()
        {
            var row = new HBaseRow
            {
                Key = "row key"
            };
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family1", "column1"),
                Timestamp = 325,
                Data = Encoding.UTF8.GetBytes("hello world")
            });
            row.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family2", "column2"),
                Timestamp = 1015,
                Data = Encoding.UTF8.GetBytes("goodbye world")
            });

            var json = HBaseSerializationHelper.SerializeObject(row);
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);
            
            Assert.Equal("{\"key\":\"cm93IGtleQ==\",\"Cell\":[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]}", 
                         compactJson);
        }

        [Fact]
        public void RowCollectionSerialization()
        {
            var row1 = new HBaseRow
            {
                Key = "row key 1"
            };
            row1.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family1", "column1"),
                Timestamp = 325,
                Data = Encoding.UTF8.GetBytes("hello world")
            });
            row1.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family2", "column2"),
                Timestamp = 1015,
                Data = Encoding.UTF8.GetBytes("goodbye world")
            });

            var row2 = new HBaseRow
            {
                Key = "row key 2"
            };
            row2.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family3", "column3"),
                Timestamp = 76,
                Data = Encoding.UTF8.GetBytes("你好世界")
            });
            row2.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family4", "column4"),
                Timestamp = 1111,
                Data = Encoding.UTF8.GetBytes("再见世界")
            });

            var json = HBaseSerializationHelper.SerializeObject(new[] { row1, row2 });
            var compactJson = Regex.Replace(json, @"\s+", string.Empty);
            
            Assert.Equal("[{\"key\":\"cm93IGtleSAx\",\"Cell\":[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]},{\"key\":\"cm93IGtleSAy\",\"Cell\":[{\"column\":\"ZmFtaWx5Mzpjb2x1bW4z\",\"timestamp\":76,\"$\":\"5L2g5aW95LiW55WM\"},{\"column\":\"ZmFtaWx5NDpjb2x1bW40\",\"timestamp\":1111,\"$\":\"5YaN6KeB5LiW55WM\"}]}]", 
                         compactJson);
        }
    }
}
