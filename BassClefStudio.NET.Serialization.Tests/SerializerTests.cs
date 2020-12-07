using BassClefStudio.NET.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BassClefStudio.NET.Serialization.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void TestCircular()
        {
            Base a = new Base();
            Derived b = new Derived() { Child = a, Name = "Fred" };
            a.Child = b;

            var serializer = new SerializationService(typeof(SerializerTests).Assembly);
            string json = serializer.Serialize(a);
            Console.WriteLine(json);
            Base newA = serializer.Deserialize<Base>(json);
            Assert.AreEqual(newA.Child.Child, newA);
            Assert.IsInstanceOfType(newA.Child, typeof(Derived));
            Assert.AreEqual(((Derived)newA.Child).Name, "Fred");
        }

        [TestMethod]
        public void TestUntrustedOut()
        {
            Derived d = new Derived();

            var serializer = new SerializationService(typeof(Base));
            Assert.ThrowsException<GraphTypeException>(() => serializer.Serialize(d));
        }

        [TestMethod]
        public void TestUntrustedIn()
        {
            string json = $"[{{\"$type\":\"node\", \"Link\":{{\"Id\":0}}, \"TypeName\":\"{typeof(System.IO.File).AssemblyQualifiedName}\", \"Properties\":{{}}}}]";

            var serializer = new SerializationService(typeof(Base));
            Assert.ThrowsException<GraphTypeException>(() => serializer.Deserialize<Base>(json));
        }

        [TestMethod]
        public void TestUntrustedInConst()
        {
            string json = $"[{{\"$type\":\"node\", \"Link\":{{\"Id\":0}}, \"TypeName\":\"{typeof(BadDerived).AssemblyQualifiedName}\", \"Properties\":{{}}}}]";

            var serializer = new SerializationService(typeof(Base));
            Assert.ThrowsException<GraphTypeException>(() => serializer.Deserialize<Base>(json));
        }

        [TestMethod]
        public void TestCollection()
        {
            Base a = new Base();
            Base b = new Base();
            Base c = new Base();
            Base d = new Base();
            ListDerived e = new ListDerived() { Child = a, Parents = new List<Base>() { a, b, c, d } };

            var serializer = new SerializationService(new Assembly[] { typeof(SerializerTests).Assembly }, new Type[] { typeof(List<>) });
            string json = serializer.Serialize(e);
            Console.WriteLine(json);
            Base newE = serializer.Deserialize<Base>(json);
            Assert.IsInstanceOfType(newE, typeof(ListDerived));
            var listE = (ListDerived)newE;
            Assert.IsNotNull(listE.Parents);
            Assert.AreEqual(4, listE.Parents.Count);
            Assert.AreEqual(listE.Child, listE.Parents[0]);
        }
    }

    public class Base
    {
        public Base Child { get; set; }
    }

    public class Derived : Base
    {
        public string Name { get; set; }
    }

    public class BadDerived : Base
    {
        public BadDerived()
        {
            throw new Exception("BadDerived class constructor should never be called.");
        }
    }

    public class ListDerived : Base
    {
        public List<Base> Parents { get; set; }
    }
}
