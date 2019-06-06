using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stocker.HBase;
using Stocker.HBase.Serialization;
using Xunit;

namespace Stocker.UnitTest
{
    public class HBaseDeserializationTests
    {
        private sealed class HBaseColumnEqualityComparer : IEqualityComparer<HBaseColumn>
        {
            public bool Equals(HBaseColumn x, HBaseColumn y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.ColumnFamilyName == y.ColumnFamilyName && x.ColumnName == y.ColumnName;
            }

            public int GetHashCode(HBaseColumn obj)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class HBaseCellEqualityComparer : IEqualityComparer<HBaseCell>
        {
            public bool Equals(HBaseCell x, HBaseCell y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }
                
                var columnComparer = new HBaseColumnEqualityComparer();
                if (!columnComparer.Equals(x.Column, y.Column))
                {
                    return false;
                }

                if (x.Timestamp != y.Timestamp)
                {
                    return false;
                }

                if (x.Data == null && y.Data == null)
                {
                    return true;
                }

                if (x.Data == null || y.Data == null)
                {
                    return false;
                }

                return x.Data.SequenceEqual(y.Data);
            }

            public int GetHashCode(HBaseCell obj)
            {
                throw new System.NotImplementedException();
            }
        }

        private sealed class HBaseRowEqualityComparer : IEqualityComparer<HBaseRow>
        {
            public bool Equals(HBaseRow x, HBaseRow y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                if (x.Key != y.Key)
                {
                    return false;
                }

                return x.Cells.SequenceEqual(y.Cells, new HBaseCellEqualityComparer());
            }

            public int GetHashCode(HBaseRow obj)
            {
                throw new NotImplementedException();
            }
        }
        
        [Fact]
        public void ColumnDeserialization()
        {
            var json = "\"ZmFtaWx5OmNvbHVtbg==\"";
            var column = HBaseSerializationHelper.DeserializeObject<HBaseColumn>(json);
            
            Assert.Equal("family", column.ColumnFamilyName);
            Assert.Equal("column", column.ColumnName);
        }

        [Fact]
        public void ColumnCollectionDeserialization()
        {
            var json = "[\"ZmFtaWx5MTpjb2x1bW4x\",\"ZmFtaWx5Mjpjb2x1bW4y\",\"ZmFtaWx5Mzpjb2x1bW4z\"]";
            var columns = HBaseSerializationHelper.DeserializeObject<HBaseColumn[]>(json);
            
            Assert.Equal(new []
            {
                new HBaseColumn("family1", "column1"),
                new HBaseColumn("family2", "column2"),
                new HBaseColumn("family3", "column3"), 
            }, columns, new HBaseColumnEqualityComparer());
        }

        [Fact]
        public void CellDeserialization()
        {
            var json = "{\"column\":\"ZmFtaWx5OmNvbHVtbg==\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"}";
            var cell = HBaseSerializationHelper.DeserializeObject<HBaseCell>(json);

            Assert.Equal(new HBaseColumn("family", "column"), cell.Column, new HBaseColumnEqualityComparer());
            Assert.Equal(325, cell.Timestamp);
            Assert.Equal(Encoding.UTF8.GetBytes("hello world"), cell.Data);
        }

        [Fact]
        public void CellCollectionDeserialization()
        {
            var json = "[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]";
            var cells = HBaseSerializationHelper.DeserializeObject<HBaseCell[]>(json);
            
            Assert.Equal(new []
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
            }, cells, new HBaseCellEqualityComparer());
        }

        [Fact]
        public void RowDeserialization()
        {
            var json = "{\"key\":\"cm93IGtleQ==\",\"Cell\":[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]}";
            var row = HBaseSerializationHelper.DeserializeObject<HBaseRow>(json);

            var expectedRow = new HBaseRow { Key = "row key" };
            expectedRow.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family1", "column1"),
                Timestamp = 325,
                Data = Encoding.UTF8.GetBytes("hello world")
            });
            expectedRow.Cells.Add(new HBaseCell
            {
                Column = new HBaseColumn("family2", "column2"),
                Timestamp = 1015,
                Data = Encoding.UTF8.GetBytes("goodbye world")
            });
            
            Assert.Equal(expectedRow, row, new HBaseRowEqualityComparer());
        }

        [Fact]
        public void RowCollectionDeserialization()
        {
            var json = "[{\"key\":\"cm93IGtleSAx\",\"Cell\":[{\"column\":\"ZmFtaWx5MTpjb2x1bW4x\",\"timestamp\":325,\"$\":\"aGVsbG8gd29ybGQ=\"},{\"column\":\"ZmFtaWx5Mjpjb2x1bW4y\",\"timestamp\":1015,\"$\":\"Z29vZGJ5ZSB3b3JsZA==\"}]},{\"key\":\"cm93IGtleSAy\",\"Cell\":[{\"column\":\"ZmFtaWx5Mzpjb2x1bW4z\",\"timestamp\":76,\"$\":\"5L2g5aW95LiW55WM\"},{\"column\":\"ZmFtaWx5NDpjb2x1bW40\",\"timestamp\":1111,\"$\":\"5YaN6KeB5LiW55WM\"}]}]";
            var rows = HBaseSerializationHelper.DeserializeObject<HBaseRow[]>(json);
            
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
            
            Assert.Equal(new [] { row1, row2 }, rows, new HBaseRowEqualityComparer());
        }
    }
}
