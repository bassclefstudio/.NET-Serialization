using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Serialization;
using BassClefStudio.NET.Serialization.Services;
using BassClefStudio.NET.Serialization.Services.Core;
using BassClefStudio.NET.Serialization.Types;
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

            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly));
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

            ISerializationService serializer = new GraphSerializer(TypeMatch.Type(typeof(Base)));
            Assert.ThrowsException<TypeMatchException>(() => serializer.Serialize(d));
        }

        [TestMethod]
        public void TestCollection()
        {
            Base a = new Base();
            Base b = new Base();
            Base c = new Base();
            Base d = new Base();
            CollectionDerived e = new CollectionDerived() { Child = a, Parents = new List<Base>() { a, b, c, d } };

            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly));
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

            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly));
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

            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly));
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
            ISerializationService serializer = new GraphSerializer(TypeMatch.Type(typeof(Vector2)));
            serializer.Services.Add(new ReflectionGraphField());
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
            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly).Concat(TypeMatch.Type<Guid>()));
            serializer.Services.Add(new GuidSerializer());
            string json = serializer.Serialize(b);
            Console.WriteLine(json);
            Base newB = serializer.Deserialize<Base>(json);
            Assert.IsInstanceOfType(newB.Child, typeof(GuidDerived));
            Assert.AreEqual(a.Id, ((GuidDerived)newB.Child).Id);
        }

        #region ValueTypes

        private void TestValue<T>(T value)
        {
            ISerializationService serializer = new GraphSerializer(TypeMatch.Assembly(typeof(SerializerTests).Assembly).Concat(TypeMatch.Type<Guid>()));
            string json = serializer.Serialize(value);
            Console.WriteLine(json);
            T newVal = serializer.Deserialize<T>(json);
            Assert.AreEqual(value, newVal, $"Value type serialization failed on {typeof(T).Name} native serializer.");
        }

        [TestMethod]
        public void ColorTest()
        {
            Color color = new Color(130, 147, 89, 190);
            TestValue(color);
        }

        [TestMethod]
        public void GuidTest()
        {
            Guid id = Guid.NewGuid();
            TestValue(id);
        }

        [TestMethod]
        public void TimeTest()
        {
            DateTimeOffset offset = new DateTimeOffset(new DateTime(2021, 8, 30));
            TestValue(offset);
            DateTimeSpan span = new DateTimeSpan(offset, new TimeSpan(4, 33, 0));
            TestValue(span);
        }

        [TestMethod]
        public void VectorTest()
        {
            Vector2 vector = new Vector2(470, -132);
            TestValue(vector);
        }

        #endregion
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
        public ITypeMatch SupportedTypes { get; } = TypeMatch.Type<Guid>();

        public IDictionary<string, object> GetProperties(object value)
            => new Dictionary<string, object>()
            {
                { "Value", ((Guid)value).ToString("N") }
            };

        public bool TryConstruct(Type desiredType, IDictionary<string, object> subGraph, out object built, out IEnumerable<string> usedKeys)
        {
            built = Guid.Parse((string)subGraph["Value"]);
            usedKeys = new string[] { "Value" };
            return true;
        }
    }
}
