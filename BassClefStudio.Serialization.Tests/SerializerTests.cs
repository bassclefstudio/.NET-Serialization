using BassClefStudio.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BassClefStudio.Serialization.Tests
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
            string json = $"[{{\"Link\":{{\"Id\":0}}, \"TypeName\":\"{typeof(System.IO.File).AssemblyQualifiedName}\", \"Properties\":{{}}}}]";

            var serializer = new SerializationService(typeof(Base));
            Assert.ThrowsException<GraphTypeException>(() => serializer.Deserialize<Base>(json));
        }

        [TestMethod]
        public void TestUntrustedInConst()
        {
            string json = $"[{{\"Link\":{{\"Id\":0}}, \"TypeName\":\"{typeof(BadDerived).AssemblyQualifiedName}\", \"Properties\":{{}}}}]";

            var serializer = new SerializationService(typeof(Base));
            Assert.ThrowsException<GraphTypeException>(() => serializer.Deserialize<Base>(json));
        }
    }

    public class Base : ISerializable
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
}
