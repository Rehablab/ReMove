using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace Tests
{
    public class ParamterTestScript
    {
        [SetUp]
        public void SetUp()
        {
            XCoreParameter.Init();
        }
        [Test]
        public void TestXCoreParameterInited()
        {
            Assert.AreEqual(Constant.FreeTrial.CircleTime, XCoreParameter.cdDouble[CDDoubleKeys.CircleTime]);
            Assert.AreEqual(Constant.FreeTrial.TakeBreakTime, XCoreParameter.cdDouble[CDDoubleKeys.TakeBreakTime]);
        }
    }
}
