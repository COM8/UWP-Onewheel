using DataManager.Classes.DBTables;
using System.Collections.Generic;

namespace DataManager.Classes.DBManagers
{
    public class MeasurementsDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MeasurementsDBManager INSTANCE = new MeasurementsDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/03/2018 Created [Fabian Sauter]
        /// </history>
        public MeasurementsDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setBatteryMeasurement(BatteryTable battery)
        {
            update(battery);
        }

        public void setSpeedMeasurement(SpeedTable speed)
        {
            update(speed);
        }

        public List<BatteryTable> getAllBatteryMeasurement()
        {
            return dB.Query<BatteryTable>("SELECT * FROM " + DBTableConsts.BATTERY_TABLE + ";");
        }

        public List<SpeedTable> getAllSpeedMeasurement()
        {
            return dB.Query<SpeedTable>("SELECT * FROM " + DBTableConsts.SPEED_TABLE + " ORDER BY dateTime;");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<BatteryTable>();
            dB.CreateTable<SpeedTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<BatteryTable>();
            dB.DropTable<SpeedTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
