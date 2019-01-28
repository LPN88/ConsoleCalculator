using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleCalculator;

namespace UnitTestCalculator
{
    [TestClass]
    public class UnitTest1
    {
        //Правильность расчета
        [TestMethod]
        public void TestMethod1()
        { 
            Assert.AreEqual<decimal>(Program.ComputeEquation("(14-2)*13"),156);
            Assert.AreEqual<decimal>(Program.ComputeEquation("(14-2^4)*13"), -25);
        }

        //Ожидание ошибки при делении на 0
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestMethod2()
        {
            Assert.AreEqual(Program.ComputeEquation("14-(2-(2132-45,6)/(35-7*5)"), -12);
        }

        //Корректность выражения
        [TestMethod]
        public void TestMethod3()
        {            
            string error;
            Assert.IsTrue(!Program.ValidateInput("2+-5)",out error));
            Assert.IsTrue(error != null);
        }
      
    }
}
