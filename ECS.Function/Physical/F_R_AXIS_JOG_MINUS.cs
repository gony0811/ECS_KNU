using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core.Function;
using INNO6.IO;
using ECS.Function;
using System.Diagnostics;
using System.Threading;
using INNO6.Core.Manager;

namespace ECS.Function.Physical
{

    public class F_R_AXIS_JOG_MINUS : AbstractFunction
    {
        private const string IO_R_JOG_MINUS = "oPMAC.iAxisR.JogBwd";
        private const string ALARM_R_AXIS_JOG_MINUS_FAIL = "E2005";


        private const string VIO_R_JOG_SPEED_MODE = "vSet.sAxisR.JogVelMode";
        private const string VIO_R_JOG_SPEED_HIGH = "vSet.dAxisR.JogVelHigh";
        private const string VIO_R_JOG_SPEED_LOW = "vSet.dAxisR.JogVelLow";

        private const string IO_R_JOG_VELOCITY_SET = "oPMAC.dAxisR.JogVel";

        public override bool CanExecute()
        {
            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            if (DataManager.Instance.GET_STRING_DATA(VIO_R_JOG_SPEED_MODE, out bool _) == "HIGH")
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(VIO_R_JOG_SPEED_HIGH, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_R_JOG_VELOCITY_SET, velocity);
            }
            else
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(VIO_R_JOG_SPEED_LOW, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_R_JOG_VELOCITY_SET, velocity);
            }

            if (DataManager.Instance.SET_INT_DATA(IO_R_JOG_MINUS, 1))
            {
                return this.F_RESULT_SUCCESS;
            }
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_R_AXIS_JOG_MINUS_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void PostExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            DataManager.Instance.SET_INT_DATA(IO_R_JOG_MINUS, 0);
        }
    }
}
