using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
namespace Slouch.Tests
{
    [TestClass()]
    public class CabinetTests
    {
        private String _path = @"C:\temp\cabinet-testing";

        [TestMethod()]
        public void RoundtripTest1()
        {
            Directory.CreateDirectory(_path);

            Cabinet.Initialise(_path);

            var x = new TestClass1();
            x.Value1 = "Hey!";
            x.Value2 = "You!";
            Cabinet.SetAsync(x, "Series X", "Season Y", 5).Wait();

            var y = Cabinet.GetAsync<TestClass1>("Series X", "Season Y", 5).Result;
            Cabinet.SetAsync(y, "asdf!").Wait();

            var z = Cabinet.GetAsync<TestClass1>("asdf!").Result;

            Assert.AreEqual(x, z);
        }

        //[TestMethod()]
        //public void RoundtripTest2()
        //{
        //    //Directory.CreateDirectory(_path);

        //    //var cabinet = new Cabinet(_path);
        //}

        public class TestClass1
        {
            public String Value1 { get; set; }
            public String Value2 { get; set; }
            public String Value3 { get; set; }
            public String Value4 { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is TestClass1))
                    return false;

                var x = (TestClass1)obj;

                return x.Value1 == Value1 && x.Value2 == Value2 && x.Value3 == Value3 && x.Value4 == Value4;
            }
        }
    }
}
