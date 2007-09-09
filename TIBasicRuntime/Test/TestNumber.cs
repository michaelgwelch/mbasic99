using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace TIBasicRuntime
{
    [TestFixture]
    public class TestNumber
    {
        Number unitialized;

        [Test]
        public void ConstantsAndProperties()
        {
            Assert.That(Number.Epsilon.IsZero, Is.False);
            Assert.That(Number.Epsilon.IsNegative, Is.False);
            Assert.That(Number.Zero.IsZero, Is.True);
            Assert.That(Number.Zero.IsNegative, Is.False);

            Assert.That(unitialized.IsZero, Is.True);
            Assert.That(unitialized, Is.EqualTo(Number.Zero));
            Assert.That(unitialized.IsNegative, Is.False);

            Assert.That(Number.MaxValue.IsZero, Is.False);
            Assert.That(Number.MaxValue.IsNegative, Is.False);

            Assert.That(Number.MinValue.IsZero, Is.False);
            Assert.That(Number.MinValue.IsNegative, Is.True);

            Assert.That(Number.MinusOne.IsZero, Is.False);
            Assert.That(Number.MinusOne.IsNegative, Is.True);

            Assert.That(Number.One.IsZero, Is.False);
            Assert.That(Number.One.IsNegative, Is.False);
        }

        [Test]
        public void ConstantsAndExponent()
        {
            Assert.That(Number.Zero.Exponent, Is.EqualTo(-64), "Zero");
            Assert.That(Number.MaxValue.Exponent, Is.EqualTo(63));
            Assert.That(Number.MinValue.Exponent, Is.EqualTo(-64), "MinValue");
            Assert.That(Number.Epsilon.Exponent, Is.EqualTo(-64), "Epsilon");
            Assert.That(Number.One.Exponent, Is.EqualTo(0), "One");
            Assert.That(unitialized.Exponent, Is.EqualTo(-64), "uninitialized");
        }

        [Test]
        public void ConstantsAndOrdering()
        {
            Assert.That(Number.Zero, Is.LessThan(Number.One));
            Assert.That(Number.Zero, Is.LessThan(Number.Epsilon));
            Assert.That(Number.Zero, Is.LessThan(Number.MaxValue));

            Assert.That(Number.Epsilon, Is.GreaterThan(Number.Zero));
            Assert.That(Number.Epsilon, Is.LessThan(Number.One));
            Assert.That(Number.Epsilon, Is.LessThan(Number.MaxValue));

            Assert.That(Number.One, Is.GreaterThan(Number.Zero));
            Assert.That(Number.One, Is.GreaterThan(Number.Epsilon));
            Assert.That(Number.One, Is.LessThan(Number.MaxValue));

            Assert.That(Number.MaxValue, Is.GreaterThan(Number.Zero));
            Assert.That(Number.MaxValue, Is.GreaterThan(Number.Epsilon));
            Assert.That(Number.MaxValue, Is.GreaterThan(Number.One));

            Assert.That(unitialized, Is.LessThan(Number.Epsilon));
            Assert.That(unitialized, Is.LessThan(Number.One));
            Assert.That(unitialized, Is.LessThan(Number.MaxValue));


            Assert.That(Number.MaxInt32, Is.LessThan(Number.MaxUInt32));
            Assert.That(Number.MaxUInt32, Is.LessThan(Number.MaxInteger));

        }


        [Test]
        public void Equals()
        {
            Assert.That(Number.One.Equals(1));
        }
        [Test]
        public void ConstantsAndConversions()
        {
        }



        [Test]
        public void ToFromUInt32Values()
        {
            uint val1 = 7;
            uint val2 = 23;
            uint val3 = 324;
            uint val4 = 4236;
            uint val5 = 57891;
            uint val6 = 679213;
            uint val7 = 7892135;
            uint val8 = 91283475;
            uint val9 = 127483920;
            uint val10 = 3834729385;

            DoUInt32ConversionTest(val1);
            DoUInt32ConversionTest(val2);
            DoUInt32ConversionTest(val3);
            DoUInt32ConversionTest(val4);
            DoUInt32ConversionTest(val5);
            DoUInt32ConversionTest(val6);
            DoUInt32ConversionTest(val7);
            DoUInt32ConversionTest(val8);
            DoUInt32ConversionTest(val9);
            DoUInt32ConversionTest(val10);

        }

        private void DoUInt32ConversionTest(uint val)
        {
            Number num = val;
            uint val2 = (uint)num;
            Assert.That(val, Is.EqualTo(val2));
        }
    }
}
