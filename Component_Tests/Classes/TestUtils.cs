using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnewheelBluetooth.Classes;
using System;

namespace Component_Tests.Classes
{
    [TestClass]
    public class TestUtils
    {
        #region --Carve Ability--
        [TestMethod]
        public void Test_CarveAbilityToDouble_Default_1()
        {
            double result = Utils.CarveAbilityToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_CARVE_ABILITY);
            Assert.AreEqual(result, 0.0d);
        }

        [TestMethod]
        public void Test_CarveAbilityToDouble_Min_1()
        {
            double result = Utils.CarveAbilityToDouble(unchecked((byte)Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY));
            Assert.AreEqual(result, -5.0d);
        }

        [TestMethod]
        public void Test_CarveAbilityToDouble_Max_1()
        {
            double result = Utils.CarveAbilityToDouble((byte)Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY);
            Assert.AreEqual(result, +5.0d);
        }

        [TestMethod]
        public void Test_CarveAbilityToByte_Default_1()
        {
            byte result = Utils.CarveAbilityToByte(0.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_DEFAULT_CARVE_ABILITY);
        }

        [TestMethod]
        public void Test_CarveAbilityToByte_Min_1()
        {
            byte result = Utils.CarveAbilityToByte(-5.0d);
            Assert.AreEqual(result, unchecked((byte)Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY));
        }

        [TestMethod]
        public void Test_CarveAbilityToByte_Min_2()
        {
            byte result = Utils.CarveAbilityToByte(-6.0d);
            Assert.AreEqual(result, unchecked((byte)Consts.CUSTOM_SHAPING_MIN_CARVE_ABILITY));
        }

        [TestMethod]
        public void Test_CarveAbilityToByte_Max_1()
        {
            byte result = Utils.CarveAbilityToByte(+5.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY);
        }

        [TestMethod]
        public void Test_CarveAbilityToByte_Max_2()
        {
            byte result = Utils.CarveAbilityToByte(+6.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_MAX_CARVE_ABILITY);
        }

        #endregion

        #region --Stance Profile--
        [TestMethod]
        public void Test_StanceProfileToDouble_Default_1()
        {
            double result = Utils.StanceProfileToDouble((byte)Consts.CUSTOM_SHAPING_DEFAULT_STANCE_PROFILE);
            Assert.AreEqual(result, 0.0d);
        }

        [TestMethod]
        public void Test_StanceProfileToDouble_Min_1()
        {
            double result = Utils.StanceProfileToDouble(unchecked((byte)Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE));
            Assert.AreEqual(result, -1.0d);
        }

        [TestMethod]
        public void Test_StanceProfileToDouble_Max_1()
        {
            double result = Utils.StanceProfileToDouble((byte)Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE);
            Assert.AreEqual(result, +3.0d);
        }

        [TestMethod]
        public void Test_StanceProfileToByte_Default_1()
        {
            byte result = Utils.StanceProfileToByte(0.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_DEFAULT_STANCE_PROFILE);
        }

        [TestMethod]
        public void Test_StanceProfileToByte_Min_1()
        {
            byte result = Utils.StanceProfileToByte(-1.0d);
            Assert.AreEqual(result, unchecked((byte)Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE));
        }

        [TestMethod]
        public void Test_StanceProfileToByte_Min_2()
        {
            byte result = Utils.StanceProfileToByte(-6.0d);
            Assert.AreEqual(result, unchecked((byte)Consts.CUSTOM_SHAPING_MIN_STANCE_PROFILE));
        }

        [TestMethod]
        public void Test_StanceProfileToByte_Max_1()
        {
            byte result = Utils.StanceProfileToByte(+3.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE);
        }

        [TestMethod]
        public void Test_StanceProfileToByte_Max_2()
        {
            byte result = Utils.StanceProfileToByte(+6.0d);
            Assert.AreEqual(result, (byte)Consts.CUSTOM_SHAPING_MAX_STANCE_PROFILE);
        }

        #endregion

        #region --Aggressiveness--
        [TestMethod]
        public void Test_AggressivenessToUInt_Default_1()
        {
            double result = Utils.AggressivenessToDouble(unchecked((byte)-80.0d));
            Assert.AreEqual(result, 1.0d);
        }

        [TestMethod]
        public void Test_AggressivenessToUInt_Min_1()
        {
            double result = Utils.AggressivenessToDouble(44);
            Assert.AreEqual(result, 7.0d);
        }

        [TestMethod]
        public void Test_AggressivenessToUInt_Max_1()
        {
            double result = Utils.AggressivenessToDouble((byte)Consts.CUSTOM_SHAPING_VALUES_AGGRESSIVENESS[10]);
            Assert.AreEqual(result, 11);
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Default_1()
        {
            byte result = Utils.AggressivenessToByte(7.0d);
            Assert.AreEqual(result, (byte)44.0d);
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Min_1()
        {
            byte result = Utils.AggressivenessToByte(1.0d);
            Assert.AreEqual(result, unchecked((byte)-80.0d));
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Min_2()
        {
            byte result = Utils.AggressivenessToByte(0.0d);
            Assert.AreEqual(result, unchecked((byte)-80.0d));
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Max_1()
        {
            byte result = Utils.AggressivenessToByte(11.0d);
            Assert.AreEqual(result, 127.0d);
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Max_2()
        {
            byte result = Utils.AggressivenessToByte(12.0d);
            Assert.AreEqual(result, 127.0d);
        }

        [TestMethod]
        public void Test_AggressivenessToByte_Reverese_1()
        {
            for (double d = 1; d <= 11; d += 0.1)
            {
                double result = Utils.AggressivenessToDouble(Utils.AggressivenessToByte(d));
                Assert.AreEqual(result, Math.Round(d, 1));
            }
        }

        #endregion
    }
}
