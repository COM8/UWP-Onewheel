using DataManager.Classes;
using DataManager.Classes.DBManagers;
using DataManager.Classes.DBTables;
using System;
using System.Threading;
using Windows.System.Threading;

namespace BluetoothOnewheelAccess.Classes
{
    class OnewheelSpeedHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly SemaphoreSlim CHANGED_SEMA = new SemaphoreSlim(1);

        private bool isIcreasing;
        private uint lastRpm;
        private DateTime lastTimestamp;
        private ThreadPoolTimer timer;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/03/2018 Created [Fabian Sauter]
        /// </history>
        public OnewheelSpeedHandler()
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Should get called as soon as the boars rpm changes.
        /// Adds the value if necessary to the DB.
        /// </summary>
        /// <param name="rpm">The current RPM.</param>
        /// <param name="timestamp">The timestamp of the RPM value.</param>
        public void onRpmChanged(uint rpm, DateTime timestamp)
        {
            CHANGED_SEMA.Wait();
            if (rpm > lastRpm * 1.1)
            {
                if (!isIcreasing)
                {
                    isIcreasing = true;
                    addToDB(rpm, timestamp);
                }
            }
            else if (rpm < lastRpm * 0.9)
            {
                if (isIcreasing)
                {
                    isIcreasing = false;
                    addToDB(rpm, timestamp);
                }
            }

            lastRpm = rpm;
            lastTimestamp = timestamp;
            CHANGED_SEMA.Release();
        }

        /// <summary>
        /// Resets the OnewheelSpeedHandler.
        /// </summary>
        public void reset()
        {
            stopTimer();

            isIcreasing = false;
            lastRpm = uint.MinValue;
            timer = null;
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Adds a SpeedTable with the given RPM and timestamp to the DB.
        /// </summary>
        /// <param name="rpm">The RPM value that should get used.</param>
        /// <param name="timestamp">The timestamp of the given RPM value.</param>
        private void addToDB(uint rpm, DateTime timestamp)
        {
            MeasurementsDBManager.INSTANCE.setSpeedMeasurement(new SpeedTable()
            {
                dateTime = timestamp,
                rpm = rpm,
                kilometersPerHour = Utils.rpmToKilometersPerHour(rpm)
            });
        }

        /// <summary>
        /// Stops and restarts the timer.
        /// </summary>
        private void resetTimer()
        {
            stopTimer();

            timer = ThreadPoolTimer.CreateTimer((source) => addToDB(lastRpm, lastTimestamp), TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void stopTimer()
        {
            try
            {
                timer?.Cancel();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
