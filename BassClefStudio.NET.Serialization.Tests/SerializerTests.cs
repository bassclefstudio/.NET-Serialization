using BassClefStudio.NET.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

            var serializer = new SerializationService(typeof(SerializerTests).Assembly);
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
            CollectionDerived e = new CollectionDerived() { Child = a, Parents = new List<Base>() { a, b, c, d } };

            var serializer = new SerializationService(typeof(SerializerTests).Assembly);
            string json = serializer.Serialize(e);
            Console.WriteLine(json);
            Base newE = serializer.Deserialize<Base>(json);
            Assert.IsInstanceOfType(newE, typeof(CollectionDerived));
            var listE = (CollectionDerived)newE;
            Assert.IsNotNull(listE.Parents);
            Assert.AreEqual(4, listE.Parents.Count());
            Assert.AreEqual(listE.Child, listE.Parents.ElementAt(0));
        }

        [TestMethod]
        public void TestArray()
        {
            Base a = new Base();
            Base b = new Base();
            Base c = new Base();
            Base d = new Base();
            CollectionDerived e = new CollectionDerived() { Child = a, Parents = new Base[] { a, b, c, d } };

            var serializer = new SerializationService(typeof(SerializerTests).Assembly);
            string json = serializer.Serialize(e);
            Console.WriteLine(json);
            Base newE = serializer.Deserialize<Base>(json);
            Assert.IsInstanceOfType(newE, typeof(CollectionDerived));
            var listE = (CollectionDerived)newE;
            Assert.IsNotNull(listE.Parents);
            Assert.AreEqual(4, listE.Parents.Count());
            Assert.AreEqual(listE.Child, listE.Parents.ElementAt(0));
        }

        [TestMethod]
        public void TestNoConstructor()
        {
            Base a = new Base();
            DerivedNoConst b = new DerivedNoConst(a, "Fred");
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
        public void ExplicitValueSerialize()
        {
            Vector2 vector = new Vector2(10, 40);
            var serializer = new SerializationService(GraphBehaviour.IncludeFields | GraphBehaviour.SetFields, typeof(Vector2));
            string json = serializer.Serialize(vector);
            Console.WriteLine(json);
            Vector2 newVector = serializer.Deserialize<Vector2>(json);
            Assert.AreEqual(vector, newVector);
        }

        [TestMethod]
        public void CustomSerializer()
        {
            GuidDerived a = new GuidDerived() { Child = null, Id = Guid.NewGuid() };
            Base b = new Base() { Child = a };
            var serializer = new SerializationService(new Assembly[] { typeof(SerializerTests).Assembly }, new Type[] { typeof(Guid) });
            serializer.AddCustomSerializer(new GuidSerializer());
            string json = serializer.Serialize(b);
            Console.WriteLine(json);
            Base newB = serializer.Deserialize<Base>(json);
            Assert.IsInstanceOfType(newB.Child, typeof(GuidDerived));
            Assert.AreEqual(a.Id, ((GuidDerived)newB.Child).Id);
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

    public class GuidDerived : Base
    {
        public Guid Id { get; set; }
    }

    public class DerivedNoConst : Derived
    {
        public DerivedNoConst(Base child, string name)
        {
            Child = child;
            Name = name;
        }
    }

    public class BadDerived : Base
    {
        public BadDerived()
        {
            throw new Exception("BadDerived class constructor should never be called.");
        }
    }

    public class CollectionDerived : Base
    {
        public IEnumerable<Base> Parents { get; set; }
    }

    public class GuidSerializer : ICustomSerializer
    {
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(Guid));
        
        public object Deserialize(string value)
        {
            return Guid.Parse(value);
        }

        public string Serialize(object o)
        {
            return ((Guid)o).ToString("N");
        }
    }
}
