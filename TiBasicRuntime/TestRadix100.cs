using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TiBasicRuntime
{
    [TestFixture]
    public class TestRadix100
    {
        [Test]
        public void StaticFields()
        {
            Assert.IsFalse(Radix100.MaxValue.IsInteger, "max");
            Assert.IsFalse(Radix100.MinValue.IsInteger, "min");
            Assert.IsFalse(Radix100.Epsilon.IsInteger, "epsilon");
            Assert.IsTrue(Radix100.Zero.IsInteger, "zero");
        }

        [Test]
        public void FromInt()
        {
            Radix100 r = 129871;
            Assert.IsTrue(r.IsInteger);
            double d = r;
            Assert.AreEqual((double)129871, d, double.Epsilon);

        }

        [Test]
        public void Add()
        {
            Radix100 r1 = 127394;
            Radix100 r2 = 7892;
            Radix100 r3 = r1 + r2;
            Assert.AreEqual((double)127394 + 7892, r3, Radix100.Epsilon);

            r1 = Radix100.FromDouble(0.000000001);
            Assert.AreEqual(r1, r1 + Radix100.Zero);
            Assert.AreEqual(r1, Radix100.Zero + r1);
        }

        [Test]
        public void IntOrReal()
        {
            double d1 = Math.Exp(2);
            Radix100 r1 = Radix100.Exp((Radix100)2.0);

            double d2 = Math.Log(d1);
            Radix100 r2 = Radix100.Log(r1);

            Assert.AreEqual(d2, r2, Radix100.Epsilon);

        }

        [Test]
        public void Strings()
        {
            Radix100 x = -10;
            Radix100 y = (Radix100)7.1;

            Assert.AreEqual("-10", x.ToString());
            Assert.AreEqual("7.1", y.ToString()); 
            Assert.AreEqual("9.34277E+10", Radix100.FromInteger(93427685127).ToString());
            Assert.AreEqual(".0000000001", Radix100.FromDouble(1e-10).ToString());
            Assert.AreEqual("1.2E-10", Radix100.FromDouble(1.2e-10).ToString());
            Assert.AreEqual("2.46E-10", Radix100.FromDouble(.000000000246).ToString());
            Assert.AreEqual("15", Radix100.FromInteger(15).ToString());
            Assert.AreEqual("-3", Radix100.FromInteger(-3).ToString());
            Assert.AreEqual("3.35", Radix100.FromDouble(3.350).ToString());
            Assert.AreEqual("-46.1", Radix100.FromDouble(-46.1).ToString());
            Assert.AreEqual("791.1234568", Radix100.FromDouble(791.123456789).ToString());
            Assert.AreEqual("7.91123E+10", Radix100.FromDouble(79112345678).ToString());
            Assert.AreEqual("7911234568.", Radix100.FromDouble(7911234567.8).ToString());
            Assert.AreEqual("-7911234568.", Radix100.FromDouble(-7911234567.8).ToString());
            Assert.AreEqual("-.0127", Radix100.FromDouble(-12.7E-3).ToString());
            Assert.AreEqual(".64", Radix100.FromDouble(0.64).ToString());
            Assert.AreEqual("1.97853E-10", Radix100.FromDouble(.0000000001978531).ToString());
            Assert.AreEqual("-9.877E+22", Radix100.FromDouble(-98.77E21).ToString());
            Assert.AreEqual("7.364E+12", Radix100.FromDouble(736.400E10).ToString());
            Assert.AreEqual("1.23659E-14", Radix100.FromDouble(12.36587E-15).ToString());
            Assert.AreEqual("1.25E-09", Radix100.FromDouble(1.25e-9).ToString());
            Assert.AreEqual("-4.36E+13", Radix100.FromDouble(-43.6e12).ToString());
            Assert.AreEqual("7.6E+**", Radix100.FromDouble(.76E126).ToString());
            Assert.AreEqual("8.1E-**", Radix100.FromDouble(81e-115).ToString());
            Assert.AreEqual("-7.6E+**", Radix100.FromDouble(-.76E126).ToString());
            Assert.AreEqual("-8.1E-**", Radix100.FromDouble(-81e-115).ToString());
            Assert.AreEqual("2.E+10", Radix100.FromInteger(19999999999).ToString());
            Assert.AreEqual("8.23498E+12", Radix100.FromDouble(8234983729385).ToString());
            double val = 1.000000000002;
            Radix100 r = Radix100.FromDouble(val);
            Assert.AreEqual("1.", r.ToString());
        }

        [Test]
        public void Round()
        {
            Radix100 original = Radix100.FromInteger(12345678999);
            Radix100 expected = Radix100.FromInteger(12345679000);
            Radix100 actual = Radix100.Round(original, 10);
            Assert.AreEqual(expected, actual);

            original = Radix100.FromInteger(19);
            expected = Radix100.FromInteger(20);
            actual = Radix100.Round(original, 1);
            Assert.AreEqual(expected, actual);

            original = Radix100.FromInteger(199);
            expected = Radix100.FromInteger(200);
            actual = Radix100.Round(original, 2);
            Assert.AreEqual(expected, actual);

            original = Radix100.FromInteger(-199);
            expected = Radix100.FromInteger(-199);
            actual = Radix100.Round(original, 3);
            Assert.AreEqual(expected, actual);

        }
    }
}
