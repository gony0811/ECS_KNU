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
    public class F_Z_AXIS_JOG_STOP : AbstractFunction
    {
        private readonly string IO_Z_JOG_STOP = "oPMAC.iAxisZ.JogStop";
        private const string ALARM_Z_AXIS_JOG_STOP_FAIL = "E2047";

        public override bool CanExecute()
        {
            return true;
        }

        public override string Execute()
        {
            if (DataManager.Instance.SET_INT_DATA(IO_Z_JOG_STOP, 1))
            {
                
                return this.F_RESULT_SUCCESS;
            }
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_Z_AXIS_JOG_STOP_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IO_Z_JOG_STOP, 0);
        }
    }
}
