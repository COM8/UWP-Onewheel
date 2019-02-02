using Logging;
using Onewheel_UI_Context.Classes.DataContexts.Dialogs;
using Onewheel_UI_Context.Classes.DataTemplates.Controls;
using OnewheelBluetooth.Classes;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Onewheel_UI_Context.Classes.DataContexts.Controls
{
    public sealed class InfoTitleBarControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly InfoTitleBarControlDataTemplate MODEL = new InfoTitleBarControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<GattWriteResult> ChangeBoardNameAsync(ChangeBoardNameDialogContext context)
        {
            if (context.MODEL.Accepted)
            {
                GattWriteResult result = await OnewheelConnectionHelper.INSTANCE.GetOnewheel().WriteStringAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_CUSTOM_NAME, context.MODEL.CustomName);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    Logger.Info("Successfully update the custom name to: " + context.MODEL.CustomName);
                }
                else
                {
                    Logger.Error("Failed with " + result.Status + " to update the custom name to: " + context.MODEL.CustomName);
                }

                return result;
            }
            return null;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
