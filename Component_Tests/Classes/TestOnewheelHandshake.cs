using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnewheelBluetooth.Classes;
using System.Linq;

namespace Component_Tests.Classes
{
    [TestClass]
    // Test data from: https://github.com/ponewheel/android-ponewheel/issues/86#issuecomment-440620036
    public class TestOnewheelHandshake
    {
        [TestMethod]
        public void Test_Handshake1()
        {
            OnewheelUnlockHelper unlockHelper = new OnewheelUnlockHelper(null);
            byte[] inData = Utils.HexStringToByteArray("43:52:58:7f:9e:5c:14:df:42:e2:62:82:62:62:62:62:62:77:f6:9c".Replace(":", ""));
            byte[] outRefData = Utils.HexStringToByteArray("43:52:58:d8:82:11:d1:26:96:5f:9f:aa:72:fc:de:92:f3:25:3d:20".Replace(":", ""));
            byte[] outData = unlockHelper.CalcResponse(inData);

            Assert.IsTrue(outData.Length == 20);
            Assert.IsTrue(outData.SequenceEqual(outRefData));
        }

        [TestMethod]
        public void Test_Handshake2()
        {
            OnewheelUnlockHelper unlockHelper = new OnewheelUnlockHelper(null);
            byte[] inData = Utils.HexStringToByteArray("43:52:58:7f:8e:0c:4c:17:7a:22:a2:b2:e2:e2:e2:e2:e2:f8:77:ca".Replace(":", ""));
            byte[] outRefData = Utils.HexStringToByteArray("43:52:58:4a:8d:4c:93:ca:9c:75:bc:ba:73:87:53:e9:10:4b:49:28".Replace(":", ""));
            byte[] outData = unlockHelper.CalcResponse(inData);

            Assert.IsTrue(outData.Length == 20);
            Assert.IsTrue(outData.SequenceEqual(outRefData));
        }

        [TestMethod]
        public void Test_Handshake3()
        {
            OnewheelUnlockHelper unlockHelper = new OnewheelUnlockHelper(null);
            byte[] inData = Utils.HexStringToByteArray("43:52:58:be:3c:45:5d:2d:90:38:f8:78:38:38:38:38:38:4e:0c:ac".Replace(":", ""));
            byte[] outRefData = Utils.HexStringToByteArray("43:52:58:c8:4b:77:d2:1d:fa:5c:a1:ab:7e:ee:1f:c8:2f:fa:19:55".Replace(":", ""));
            byte[] outData = unlockHelper.CalcResponse(inData);

            Assert.IsTrue(outData.Length == 20);
            Assert.IsTrue(outData.SequenceEqual(outRefData));
        }

        [TestMethod]
        public void Test_Handshake4()
        {
            OnewheelUnlockHelper unlockHelper = new OnewheelUnlockHelper(null);
            byte[] inData = Utils.HexStringToByteArray("43:52:58:ff:fe:cb:12:dd:3f:b7:b7:b7:57:57:57:57:57:6c:6b:94".Replace(":", ""));
            byte[] outRefData = Utils.HexStringToByteArray("43:52:58:bd:26:ed:86:75:c3:be:b7:ab:7f:78:8c:0b:b6:3c:85:22".Replace(":", ""));
            byte[] outData = unlockHelper.CalcResponse(inData);

            Assert.IsTrue(outData.Length == 20);
            Assert.IsTrue(outData.SequenceEqual(outRefData));
        }
    }
}
