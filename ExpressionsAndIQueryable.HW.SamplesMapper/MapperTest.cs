using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionsAndIQueryable.HW.Mapper
{
    public class Foo {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name} FullName:{FullName}";
        }
    }

    public class Bar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name} LastName:{LastName}";
        }
    }



    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void TestMappingFooToBar()
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();

            Foo foo = new Foo() {Id = 42, Name = "From foo", FullName = "Full foo name"};
            Bar bar = mapper.Map(foo);

            Console.WriteLine("foo: {0}",foo);
            Console.WriteLine("bar: {0}", bar);
        }
    }


}
