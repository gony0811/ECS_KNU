using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using INNO6.Core;
using INNO6.IO.Interface;
using Pcomm32Functions;

namespace DEV.MotionControl
{

    public partial class PMAC : XSequence, IDeviceHandler
    {
        private UInt32 m_dwDevice;
        private Int32 m_bDriverOpen;
        System.Timers.Timer _100msTimer;
        private string _deviceName;
        private eDevMode _deviceMode;
        private int _deviceLog;
        private bool _isWriteLog;
        private float _XAxisUnitPerCounts = 5000;
        private float _YAxisUnitPerCounts = 5000;
        private float _ZAxisUnitPerCounts = 1000;
        private float _TAxisUnitPerCounts = 2400;
        private float _RAxisUnitPerCounts = 1000;

        // mm/s -> counts/msec 
        private float _XAxisVelocityConvert = 5.0F;
        private float _YAxisVelocityConvert = 5.0F;
        private float _ZAxisVelocityConvert = 1.0F;
        private float _TAxisVelocityConvert = 2.4F;
        private float _RAxisVelocityConvert = 1.0F;

        private double _XAxisAbsSetPosition;
        private double _YAxisAbsSetPosition;
        private double _ZAxisAbsSetPosition;
        private double _TAxisAbsSetPosition;
        private double _RAxisAbsSetPosition;

        private double _XAxisRelSetPosition;
        private double _YAxisRelSetPosition;
        private double _ZAxisRelSetPosition;
        private double _TAxisRelSetPosition;
        private double _RAxisRelSetPosition;

        private double _XAxisSetVelocity;
        private double _YAxisSetVelocity;
        private double _ZAxisSetVelocity;
        private double _TAxisSetVelocity;
        private double _RAxisSetVelocity;

        #region Define 
        private const string JOG_FORWARD = "+";
        private const string JOG_BACKWARD = "-";
        private const string JOG_STOP = "/";
        private const string SERVO_STOP = "K";
        private const string MOTION_ABORT = "A";
        private const string MOTION_QUIT = "Q";

        private const string GET_POSITION = "P";
        private const string GET_VELOCITY = "V";
        private const string SET_VELOCITY = "F";

        private const int iAXIS_ALL = 0;
        private const int iAXIS_X = 2;
        private const int iAXIS_Y = 1;
        private const int iAXIS_Z = 5;
        private const int iAXIS_T = 3;
        private const int iAXIS_R = 4;

        private const string sAXIS_X = "2";
        private const string sAXIS_Y = "1";
        private const string sAXIS_Z = "5";
        private const string sAXIS_T = "3";
        private const string sAXIS_R = "4";

        public const string COMM_TIMEOUT = "TIMEOUT";
        public const string COMM_SUCCESS = "SUCCESS";
        public const string COMM_DISCONNECT = "DISCONNECT";
        public const string COMM_ERROR = "ERROR";
        public const string COMM_UNKNOWN = "UNKNOWN";



        // ID Define
        public const string ID_1_INPUT = "1";
        public const string ID_1_OUTPUT = "2";
        public const string ID_1_BOTH = "3";

        public const string ID_2_OBJECT = "0";
        public const string ID_2_DOUBLE = "1";
        public const string ID_2_INT = "2";
        public const string ID_2_STRING = "3";


        private const string AXIS_Y_GET_POSITION = "P421";
        private const string AXIS_Y_GET_VELOCITY = "P422";

        private const string AXIS_X_GET_POSITION = "P411";
        private const string AXIS_X_GET_VELOCITY = "P412";

        private const string AXIS_T_GET_POSITION = "P471";
        private const string AXIS_T_GET_VELOCITY = "P472";

        private const string AXIS_R_GET_POSITION = "P431";
        private const string AXIS_R_GET_VELOCITY = "P432";

        private const string AXIS_Z_GET_POSITION = "P441";
        private const string AXIS_Z_GET_VELOCITY = "P442";

        private const string AXIS_X_HOMMING = "P1100";
        private const string AXIS_Y_HOMMING = "P2100";
        private const string AXIS_Z_HOMMING = "P5100";
        private const string AXIS_T_HOMMING = "P3100";
        private const string AXIS_R_HOMMING = "P4100";

        private const string AXIS_X_HOMMING_STOP = "P1005";
        private const string AXIS_Y_HOMMING_STOP = "P2005";
        private const string AXIS_Z_HOMMING_STOP = "P5005";
        private const string AXIS_T_HOMMING_STOP = "P3005";
        private const string AXIS_R_HOMMING_STOP = "P4005";

        private const string AXIS_X_SET_ABSOLUTE_POSTION = "P1111";
        private const string AXIS_Y_SET_ABSOLUTE_POSTION = "P2111";
        private const string AXIS_Z_SET_ABSOLUTE_POSTION = "P5111";
        private const string AXIS_T_SET_ABSOLUTE_POSTION = "P3111";
        private const string AXIS_R_SET_ABSOLUTE_POSTION = "P4111";

        private const string AXIS_X_SET_VELOCITY = "P1112";
        private const string AXIS_Y_SET_VELOCITY = "P2112";
        private const string AXIS_Z_SET_VELOCITY = "P5112";
        private const string AXIS_T_SET_VELOCITY = "P3112";
        private const string AXIS_R_SET_VELOCITY = "P4112";

        private const string AXIS_X_MOVE_TO_ABSOLUTE_POSTION = "P1110";
        private const string AXIS_Y_MOVE_TO_ABSOLUTE_POSTION = "P2110";
        private const string AXIS_Z_MOVE_TO_ABSOLUTE_POSTION = "P5110";
        private const string AXIS_T_MOVE_TO_ABSOLUTE_POSTION = "P3110";
        private const string AXIS_R_MOVE_TO_ABSOLUTE_POSTION = "P4110";

        private const string AXIS_X_MOVE_STOP = "P357";
        private const string AXIS_Y_MOVE_STOP = "P457";
        private const string AXIS_Z_MOVE_STOP = "P557";
        private const string AXIS_T_MOVE_STOP = "P657";
        private const string AXIS_R_MOVE_STOP = "P757";

        private const string AXIS_X_IS_HOMMING = "P1000";
        private const string AXIS_Y_IS_HOMMING = "P2000";
        private const string AXIS_Z_IS_HOMMING = "P5000";
        private const string AXIS_T_IS_HOMMING = "P3000";
        private const string AXIS_R_IS_HOMMING = "P4000";

        private const string AXIS_X_IS_HOMMING_COMPLETED = "P1001";
        private const string AXIS_Y_IS_HOMMING_COMPLETED = "P2001";
        private const string AXIS_Z_IS_HOMMING_COMPLETED = "P5001";
        private const string AXIS_T_IS_HOMMING_COMPLETED = "P3001";
        private const string AXIS_R_IS_HOMMING_COMPLETED = "P4001";

        private const string AXIS_X_IS_MOVING = "P1010";
        private const string AXIS_Y_IS_MOVING = "P2010";
        private const string AXIS_Z_IS_MOVING = "P5010";
        private const string AXIS_T_IS_MOVING = "P3010";
        private const string AXIS_R_IS_MOVING = "P4010";

        //COMMON DIO-------------------------------------------------------------------------

        private const string EMERGENCY_DOOR_OPEN_STATUS = "M7100";
        private const string EMERGENCY_CPBOX_OPEN_STATUS = "M7101";

        private const string SET_TOWERLAMP_RED = "M7200";
        private const string SET_TOWERLAMP_YELLOW = "M7201";
        private const string SET_TOWERLAMP_GREEN = "M7202";
        private const string SET_BUZZER = "M7203";
        private const string SET_LEDLIGHT_ONOFF = "M7204";

        //KETI DIO------------------------------------------------------------------
        private const string GAS_ALARM_STATUS = "M7103"; //KETI
        private const string LASER_SHUTTER_FORWARD_STATUS = "M7107";
        private const string LASER_SHUTTER_BACKWARD_STATUS = "M7108";
        private const string TABLE_VACCUM_STATUS = "M7109";
        private const string TABLE_VACCUM_PRESSURE_ON_STATUS = "M7110";
        private const string TABLE_VACCUM_DIGITAL_PRESSURE_STATUS = "M7111";



        private const string SET_LASER_SHUTTER_FORWARD = "M7210";
        private const string SET_LASER_SHUTTER_BACKWARD = "M7211";
        private const string SET_TABLE_VACCUM_ONOFF = "M7212";
        private const string SET_TABLE_FURGE_ONOFF = "M7213";
        //----------------------------------------------------------------------------

        //KNU DIO---------------------------------------------------------------------
        private const string GET_DOOR_INPUT_1_FRONT = "M7103"; //KNU
        private const string GET_DOOR_INPUT_2_LEFT = "M7104"; //KNU
        private const string GET_DOOR_INPUT_3_RIGHT = "M7105"; //KNU

        private const string GET_START_SWITCH_INPUT = "M7106"; //KNU
        private const string GET_STOP_SWITCH_INPUT = "M7107"; //KNU
        private const string GET_RESET_SWITCH_INPUT = "M7108"; //KNU

        private const string GET_PROGRAM_COMPLETED = "M7116"; //KNU
        private const string GET_FAULT_LASER = "M7117";
        private const string GET_LASER_READY = "M7118"; //KNU
        private const string GET_PROGRAM_ACTIVE = "M7119"; //KNU
        private const string GET_LIGHT_PATH_NO_BIT_0 = "M7120"; //KNU
        private const string GET_LASER_IS_ON = "M7121"; //KNU
        private const string GET_LASER_ASSIGNED = "M7122"; //KNU
        private const string GET_PILOT_LASER_IS_ON = "M7123"; //KNU

        private const string SET_START_LAMP = "M7206";
        private const string SET_STOP_LAMP = "M7206";
        private const string SET_RESET_LAMP = "M7208";
        private const string SET_SCANNER_BLOW = "M7210";
        private const string SET_CLEARNING = "M7211";

        private const string SET_EXT_ACTIVATION = "M7216";
        private const string SET_LASER_RESET = "M7217";
        private const string SET_LASER_ON = "M7218";
        private const string SET_PSTART_DYN = "M7219";
        private const string SET_PROGRAM_NO_BIT_0 = "M7220";
        private const string SET_LASER_STANDY = "M7221";
        private const string SET_PSTART_STATICAL = "M7222";
        private const string SET_PILOT_LASER_ON = "M7223";
        private const string SET_REQUEST_LASER = "M7224";

        //-----------------------------------------------------------------------------



        #endregion

        private static object _key = new object();

        public PMAC()
        {
            _100msTimer = null;
            _deviceMode = eDevMode.UNKNOWN;
            _isWriteLog = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg0">DEVICE NAME</param>
        /// <param name="arg1">DEVICE NUMBER</param>
        /// <param name="arg2">DEVICE LOG</param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <param name="arg9"></param>
        /// <returns></returns>
        public bool DeviceAttach(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            _deviceName = arg0;
            uint deviceNumber = uint.Parse(arg1);
            m_dwDevice = PCOMM32.PmacSelect(deviceNumber);
            m_bDriverOpen = PCOMM32.OpenPmacDevice(m_dwDevice);
            _deviceLog = int.Parse(arg2);

            if (_deviceLog > 0) _isWriteLog = true;
            else _isWriteLog = false;

            if (m_bDriverOpen == 0)
            {
                _deviceMode = eDevMode.DISCONNECT;

                if (_deviceLog > 0)
                {                   
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Device name : {0} is not open!", _deviceName);
                }

                return false;
            }

            //_100msTimer = new System.Timers.Timer(500);          
            //_100msTimer.Elapsed += _100msTimer_Elapsed;
            //_100msTimer.Start();
            _deviceMode = eDevMode.CONNECT;

            LaserExternalActivationKill();

            return true;
        }

        private void LaserExternalActivationKill()
        {
            // Laser safty EXT_ACTIVATION signal off
            SetDigitalValue(SET_PSTART_STATICAL, 0);
            SetDigitalValue(SET_LASER_STANDY, 0);
            SetDigitalValue(SET_REQUEST_LASER, 0);
            SetDigitalValue(SET_LASER_ON, 0);                    
            SetDigitalValue(SET_EXT_ACTIVATION, 0);
            
        }

        private void _100msTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //QueryMotionStatus(1, out bool[] status);
        }

        public bool DeviceDettach()
        {
            if (m_bDriverOpen == 0)
            {
                return true;
            }

            LaserExternalActivationKill();
            PCOMM32.ClosePmacDevice(m_dwDevice);
            _deviceMode = eDevMode.DISCONNECT;
            return true;
        }

        public bool DeviceInit()
        {
            throw new NotImplementedException();
        }

        public bool DeviceReset()
        {
            throw new NotImplementedException();
        }


        public object GET_DATA_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// X AXIS POSITION ? : id1 = '1', id2 = '1', id3 = '1', id4 = '1'
        /// Y AXIS POSITION ? : id1 = '1', id2 = '1', id3 = '2', id4 = '1'
        /// Z AXIS POSITION ? : id1 = '1', id2 = '1', id3 = '3', id4 = '1'
        /// T AXIS POSITION ? : id1 = '1', id2 = '1', id3 = '4', id4 = '1'
        /// R AXIS POSITION ? : id1 = '1', id2 = '1', id3 = '5', id4 = '1'
        /// X AXIS VELOCITY ? : id1 = '1', id2 = '1', id3 = '1', id4 = '2'
        /// Y AXIS VELOCITY ? : id1 = '1', id2 = '1', id3 = '2', id4 = '2'
        /// Z AXIS VELOCITY ? : id1 = '1', id2 = '1', id3 = '3', id4 = '2'
        /// T AXIS VELOCITY ? : id1 = '1', id2 = '1', id3 = '4', id4 = '2'
        /// R AXIS VELOCITY ? : id1 = '1', id2 = '1', id3 = '5', id4 = '2'
        /// </summary>
        /// <param name="id_1"></param>
        /// <param name="id_2"></param>
        /// <param name="id_3"></param>
        /// <param name="id_4"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public double GET_DOUBLE_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            double retValue = 0;
            result = false;

            if (id_1.Equals(ID_1_INPUT) && id_2.Equals(ID_2_DOUBLE))
            {
                if (id_3.Equals(sAXIS_X)) // AXIS X
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosisionByAxis(sAXIS_X, ref retValue);
                        retValue /= _XAxisUnitPerCounts;
                        //result = QueryPosition(AXIS_X_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocityByAxis(sAXIS_X, ref retValue);
                        retValue *= _XAxisVelocityConvert;
                        //result = QueryVelocity(AXIS_X_GET_VELOCITY, ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_Y)) // AXIS Y
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosisionByAxis(sAXIS_Y, ref retValue);
                        retValue /= _YAxisUnitPerCounts;
                        //result = QueryPosition(AXIS_Y_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocityByAxis(sAXIS_Y, ref retValue);
                        retValue *= _YAxisVelocityConvert;
                        //result = QueryVelocity(AXIS_Y_GET_VELOCITY, ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_Z)) // AXIS Z
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosisionByAxis(sAXIS_Z, ref retValue);
                        retValue /= _ZAxisUnitPerCounts;
                        //result = QueryPosition(AXIS_Z_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocityByAxis(sAXIS_Z, ref retValue);
                        retValue *= _ZAxisVelocityConvert;
                        //result = QueryVelocity(AXIS_Z_GET_VELOCITY, ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_T)) // AXIS T
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosisionByAxis(sAXIS_T, ref retValue);
                        retValue /= _TAxisUnitPerCounts;
                        //result = QueryPosition(AXIS_T_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocityByAxis(sAXIS_T, ref retValue);
                        retValue *= _TAxisVelocityConvert;
                        //result = QueryVelocity(AXIS_T_GET_VELOCITY, ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_R)) // AXIS R
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosisionByAxis(sAXIS_R, ref retValue);
                        retValue /= _RAxisUnitPerCounts;
                        //result = QueryPosition(AXIS_R_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocityByAxis(sAXIS_R, ref retValue);
                        retValue *= _RAxisVelocityConvert;
                        //result = QueryVelocity(AXIS_R_GET_VELOCITY, ref retValue);
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// X AXIS IS HOMMING? : id1 = '1', id2 = '2', id3 = '1', id4 = '1'
        /// Y AXIS IS HOMMING? : id1 = '1', id2 = '2', id3 = '2', id4 = '1'
        /// X AXIS HOMMING COMPLETED : id1 = '1', id2 = '2', id3 = '1', id4 = '2'
        /// Y AXIS HOMMING COMPLETED : id1 = '1', id2 = '2', id3 = '2', id4 = '2'
        /// X AXIS IS MOVING : id1 = '1', id2 = '2', id3 = '1', id4 = '3'
        /// Y AXIS IS MOVING : id1 = '1', id2 = '2', id3 = '2', id4 = '3'
        /// 
        /// *INPUT I/0 LIST
        /// M7100 Emergency input (Door)  : id1 = '1', id2 = '2', id3 = '3', id4 = '1'
        /// M7101 Emergency input (CpBox) : id1 = '1', id2 = '2', id3 = '3', id4 = '2'
        /// M7102 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '3'
        /// M7103 GAS ALARM : id1 = '1', id2 = '2', id3 = '3', id4 = '4'
        /// M7104 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '5'
        /// M7105 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '6'
        /// M7106 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '7'
        /// M7107 Laser Shutter Fwd : id1 = '1', id2 = '2', id3 = '3', id4 = '8'
        /// M7108 Laser Shutter Bwd : id1 = '1', id2 = '2', id3 = '3', id4 = '9'
        /// M7109 Table Vaccum : id1 = '1', id2 = '2', id3 = '3', id4 = '10'
        /// M7110 Table Vaccum Pressure On : id1 = '1', id2 = '2', id3 = '3', id4 = '11'
        /// M7111 Table Vaccum Digital Pressure : id1 = '1', id2 = '2', id3 = '3', id4 = '12'      
        /// 
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value">DOUBLE VALUE</param>
        /// <param name="result">SET RESULT</param>
        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            int retValue = 0;
            result = false;

            if (id_1.Equals(ID_1_INPUT) && id_2.Equals(ID_2_INT))
            {
                if (id_3.Equals(sAXIS_X)) // AXIS X
                {
                    if (id_4.Equals("1")) // X AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisX(ref retValue);
                    }
                    else if (id_4.Equals("2")) // X AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisX(ref retValue);
                    }
                    else if (id_4.Equals("3")) // X AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisX(ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_Y)) // AXIS Y
                {
                    if (id_4.Equals("1")) // Y AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisY(ref retValue);
                    }
                    else if (id_4.Equals("2")) // Y AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisY(ref retValue);
                    }
                    else if (id_4.Equals("3")) // Y AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisY(ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_Z)) // AXIS Z
                {
                    if (id_4.Equals("1")) // Z AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisZ(ref retValue);
                    }
                    else if (id_4.Equals("2")) // Z AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisZ(ref retValue);
                    }
                    else if (id_4.Equals("3")) // Z AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisZ(ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_T)) // AXIS T
                {
                    if (id_4.Equals("1")) // T AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisT(ref retValue);
                    }
                    else if (id_4.Equals("2")) // T AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisT(ref retValue);
                    }
                    else if (id_4.Equals("3")) // T AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisT(ref retValue);
                    }
                }
                else if (id_3.Equals(sAXIS_R)) // AXIS R
                {
                    if (id_4.Equals("1")) // R AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisR(ref retValue);
                    }
                    else if (id_4.Equals("2")) // R AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisR(ref retValue);
                    }
                    else if (id_4.Equals("3")) // R AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisR(ref retValue);
                    }
                }
                else if (id_3.Equals("9")) // INPUT IO
                {
                    if (id_4.Equals("0")) //M7100 Emergency input (Door)  : id1 = '1', id2 = '2', id3 = '9', id4 = '0'
                    {
                        result = GetDigitalValue(EMERGENCY_DOOR_OPEN_STATUS, ref retValue);
                    }
                    else if (id_4.Equals("1")) //M7101 Emergency input (CpBox) : id1 = '1', id2 = '2', id3 = '9', id4 = '1'
                    {
                        result = GetDigitalValue(EMERGENCY_CPBOX_OPEN_STATUS, ref retValue);
                    }
                    else if (id_4.Equals("3")) //M7103 DOOR INPUT 1 FRONT: id1 = '1', id2 = '2', id3 = '9', id4 = '3'
                    {
                        result = GetDigitalValue(GET_DOOR_INPUT_1_FRONT, ref retValue);
                    }
                    else if (id_4.Equals("4")) //M7104 DOOR INPUT 2 LEFT: id1 = '1', id2 = '2', id3 = '9', id4 = '4'
                    {
                        result = GetDigitalValue(GET_DOOR_INPUT_2_LEFT, ref retValue);
                    }
                    else if (id_4.Equals("5")) //M7105 DOOR INPUT 3 RIGHT: iid1 = '1', id2 = '2', id3 = '9', id4 = '5'
                    {
                        result = GetDigitalValue(GET_DOOR_INPUT_3_RIGHT, ref retValue);
                    }
                    else if (id_4.Equals("6")) //M7106 START SWITCH INPUT : iid1 = '1', id2 = '2', id3 = '9', id4 = '6'
                    {
                        result = GetDigitalValue(GET_START_SWITCH_INPUT, ref retValue);
                    }
                    else if (id_4.Equals("7")) //M7107 STOP SWITCH INPUT : iid1 = '1', id2 = '2', id3 = '9', id4 = '7'
                    {
                        result = GetDigitalValue(GET_STOP_SWITCH_INPUT, ref retValue);
                    }
                    else if (id_4.Equals("8")) //M7108 RESET SWITCH INPUT : iid1 = '1', id2 = '2', id3 = '9', id4 = '8'
                    {
                        result = GetDigitalValue(GET_RESET_SWITCH_INPUT, ref retValue);
                    }
                    else if (id_4.Equals("16")) //M7116 PROGRAM COMPLETED : iid1 = '1', id2 = '2', id3 = '9', id4 = '16'
                    {
                        result = GetDigitalValue(GET_PROGRAM_COMPLETED, ref retValue);
                    }
                    else if (id_4.Equals("17")) //M7117 FAULT LASER : iid1 = '1', id2 = '2', id3 = '9', id4 = '17'
                    {
                        result = GetDigitalValue(GET_FAULT_LASER, ref retValue);
                    }
                    else if (id_4.Equals("18")) //M7118 LASER READY : iid1 = '1', id2 = '2', id3 = '9', id4 = '18'
                    {
                        result = GetDigitalValue(GET_LASER_READY, ref retValue);
                    }
                    else if (id_4.Equals("19")) //M7119 PROGRAM ACTIVE : iid1 = '1', id2 = '2', id3 = '9', id4 = '19'
                    {
                        result = GetDigitalValue(GET_PROGRAM_ACTIVE, ref retValue);
                    }
                    else if (id_4.Equals("20")) //M7120 LIGHT PATH NO : iid1 = '1', id2 = '2', id3 = '9', id4 = '20'
                    {
                        result = GetDigitalValue(GET_LIGHT_PATH_NO_BIT_0, ref retValue);
                    }
                    else if (id_4.Equals("21")) //M7121 LASER IS ON : iid1 = '1', id2 = '2', id3 = '9', id4 = '21'
                    {
                        result = GetDigitalValue(GET_LASER_IS_ON, ref retValue);
                    }
                    else if (id_4.Equals("22")) //M7122 LASER ASSIGN : iid1 = '1', id2 = '2', id3 = '9', id4 = '22'
                    {
                        result = GetDigitalValue(GET_LASER_ASSIGNED, ref retValue);
                    }
                    else if (id_4.Equals("23")) //M7123 LASER ASSIGN : iid1 = '1', id2 = '2', id3 = '9', id4 = '23'
                    {
                        result = GetDigitalValue(GET_PILOT_LASER_IS_ON, ref retValue);
                    }
                }
            }

            return retValue;
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public eDevMode IsDevMode()
        {
            return _deviceMode;
        }

        public void SET_DATA_OUT(string id_1, string id_2, string id_3, string id_4, object value, ref bool result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// X AXIS SET POSTION(mm) : id1 = '2', id2 = '1', id3 = '2', id4 = '1'
        /// Y AXIS SET POSTION(mm) : id1 = '2', id2 = '1', id3 = '1', id4 = '1'
        /// X AXIS SET VELOCITY(mm/s) : id1 = '2', id2 = '1', id3 = '2', id4 = '2'
        /// Y AXIS SET VELOCITY(mm/s) : id1 = '2', id2 = '1', id3 = '1', id4 = '2'
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value">DOUBLE VALUE</param>
        /// <param name="result">SET RESULT</param>
        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {
            if (id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_DOUBLE))
            {
                if (id_3.Equals(sAXIS_X)) // AXIS X
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_X, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_X, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_X, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_X, value);
                    }
                }
                else if (id_3.Equals(sAXIS_Y)) // AXIS Y
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_Y, value);
                    }
                }
                else if (id_3.Equals(sAXIS_Z)) // AXIS Z
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_Z, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_Z, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_Z, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_Z, value);
                    }
                }
                else if (id_3.Equals(sAXIS_T)) // AXIS T
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_T, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_T, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_T, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_T, value);
                    }
                }
                else if (id_3.Equals(sAXIS_R)) // AXIS R
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_R, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_R, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_R, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_R, value);
                    }
                }
            }
        }

        /// <summary>
        /// JOG X FORWARD : id1 = '2', id2 = '2', id3 = '1', id4 = '1' value = '1';
        /// JOG X BACKWARD : id1 = '2', id2 = '2', id3 = '1', id4 = '2' value = '1';
        /// JOG X STOP : id1 = '2', id2 = '2', id3 = '1', id4 = '7' value = '1';
        /// JOG Y FORWARD : id1 = '2', id2 = '2', id3 = '2', id4 = '1' value = '1';
        /// JOG Y BACKWARD : id1 = '2', id2 = '2', id3 = '2', id4 = '2' value = '1';
        /// JOG Y STOP : id1 = '2', id2 = '2', id3 = '2', id4 = '7' value = '1';
        /// 
        /// HOMMING X : id1 = '2', id2 = '2', id3 = '1', id4 = '3' value = '1';
        /// HOMMING Y : id1 = '2', id2 = '2', id3 = '2', id4 = '3' value = '1';
        /// HOMMING STOP X : id1 = '2', id2 = '2', id3 = '1', id4 = '4' value = '1';
        /// HOMMING STOP Y : id1 = '2', id2 = '2', id3 = '2', id4 = '4' value = '1';
        /// ABSOLUTE MOVE AXIS Y : id1 = '2', id2 = '2', id3 = '1', id4 = '5' value = '1';
        /// ABSOULTE MOVE AXIS X : id1 = '2', id2 = '2', id3 = '2', id4 = '5' value = '1';
        /// ABSOLUTE STOP AXIS Y : id1 = '2', id2 = '2', id3 = '1', id4 = '6' value = '1';
        /// ABSOULTE STOP AXIS X : id1 = '2', id2 = '2', id3 = '2', id4 = '6' value = '1';
        /// 
        /// *OUTPUT I/0 LIST
        /// M7200 Tower Lamp Red  : id1 = '2', id2 = '2', id3 = '3', id4 = '1'
        /// M7201 Tower Lamp Yellow : id1 = '2', id2 = '2', id3 = '3', id4 = '2'
        /// M7202 Tower Lamp Green : id1 = '2', id2 = '2', id3 = '3', id4 = '3'
        /// M7203 Buzzer  : id1 = '2', id2 = '2', id3 = '3', id4 = '4'
        /// M7204 Led Light On/Off  : id1 = '2', id2 = '2', id3 = '3', id4 = '5'
        /// M7210 Laser Shutter Forward : id1 = '2', id2 = '2', id3 = '3', id4 = '11'
        /// M7211 Laser Shutter Backward : id1 = '2', id2 = '2', id3 = '3', id4 = '12'
        /// M7212 Table Vaccum On : id1 = '2', id2 = '2', id3 = '3', id4 = '13'
        /// M7213 Table Furge : id1 = '2', id2 = '2', id3 = '3', id4 = '14'
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            if (id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_INT))
            {
                if (id_3 == "0")
                {
                    if (id_4.Equals("1"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoKillAll();
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2"))
                    {
                        if(value == 1)
                        {
                            result = CommandMotonAbort(iAXIS_ALL);
                        }
                    }
                }
                else if (id_3.Equals(sAXIS_X)) // AXIS X
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogFoward(iAXIS_X);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_X);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING X AXIS
                    {
                        if (value == 1)
                        {
                            result = CommandAxisXHomming(value);
                        }
                        else
                        {
                            result = CommandAxisXHomming(0);
                        }
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP X AXIS
                    {
                        if (value == 1)
                        {
                            result = CommandAxisXHommingStop(value);
                        }
                        else
                        {
                            result = CommandAxisXHommingStop(0);
                        }
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE X AXIS
                    {
                        if (value == 1)
                        {
                            result = CommnadMoveToSetPosition(iAXIS_X, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE X AXIS
                    {
                        if (value == 1)
                        {
                            result = CommandJogStop(iAXIS_X);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("7"))
                    {
                        if (value == 1)
                        {
                            result = CommandJogStop(iAXIS_X);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_X, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_X);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else if (id_3.Equals(sAXIS_Y)) // AXIS Y
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1)
                        {
                            result = CommandJogFoward(iAXIS_Y);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_Y);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisYHomming(value);
                        else
                            result = CommandAxisYHomming(0);
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisYHommingStop(value);
                        else
                            result = CommandAxisYHommingStop(0);
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if (value == 1)
                            result = CommnadMoveToSetPosition(iAXIS_Y, value);
                        else
                            result = true;
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        result = CommandJogStop(iAXIS_Y);
                    }
                    else if (id_4.Equals("7")) // JOG STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandJogStop(iAXIS_Y);
                        else
                            result = true;
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_Y, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_Y);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else if (id_3.Equals(sAXIS_Z))
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1)
                        {
                            result = CommandJogFoward(iAXIS_Z);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_Z);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisZHomming(value);
                        else
                            result = CommandAxisZHomming(0);
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisZHommingStop(value);
                        else
                            result = CommandAxisZHommingStop(0);
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if (value == 1)
                            result = CommnadMoveToSetPosition(iAXIS_Z, value);
                        else
                            result = result;
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        result = CommandJogStop(iAXIS_Z);
                    }
                    else if (id_4.Equals("7")) // JOG STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandJogStop(iAXIS_Z);
                        else
                            result = true;
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_Z, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_Z);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else if (id_3.Equals(sAXIS_T))
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1)
                        {
                            result = CommandJogFoward(iAXIS_T);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_T);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisTHomming(value);
                        else
                            result = CommandAxisTHomming(0);
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisTHommingStop(value);
                        else
                            result = CommandAxisTHommingStop(0);
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if (value == 1)
                            result = CommnadMoveToSetPosition(iAXIS_T, value);
                        else
                            result = result;
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        result = CommandJogStop(iAXIS_T);
                    }
                    else if (id_4.Equals("7")) // JOG STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandJogStop(iAXIS_T);
                        else
                            result = true;
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_T, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_T);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else if (id_3.Equals(sAXIS_R))
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1)
                        {
                            result = CommandJogFoward(iAXIS_R);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_R);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisRHomming(value);
                        else
                            result = CommandAxisRHomming(0);
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisRHommingStop(value);
                        else
                            result = CommandAxisRHommingStop(0);
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if (value == 1)
                            result = CommnadMoveToSetPosition(iAXIS_R, value);
                        else
                            result = result;
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE R AXIS
                    {
                        result = CommandJogStop(iAXIS_R);
                    }
                    else if (id_4.Equals("7")) // JOG STOP R AXIS
                    {
                        if (value == 1)
                            result = CommandJogStop(iAXIS_R);
                        else
                            result = true;
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_R, value);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_R);
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                else if (id_3.Equals("9")) // OUTPUT I/O 
                {
                    if (id_4.Equals("0")) //M7200 Tower Lamp Red  : id1 = '2', id2 = '2', id3 = '9', id4 = '0'
                    {
                        result = SetDigitalValue(SET_TOWERLAMP_RED, value);
                    }
                    else if (id_4.Equals("1")) //M7201 Tower Lamp Yellow : id1 = '2', id2 = '2', id3 = '9', id4 = '1'
                    {
                        result = SetDigitalValue(SET_TOWERLAMP_YELLOW, value);
                    }
                    else if (id_4.Equals("2")) //M7202 Tower Lamp Green : id1 = '2', id2 = '2', id3 = '9', id4 = '2'
                    {
                        result = SetDigitalValue(SET_TOWERLAMP_GREEN, value);
                    }
                    else if (id_4.Equals("3")) //M7203 Buzzer  : id1 = '2', id2 = '2', id3 = '9', id4 = '3'
                    {
                        result = SetDigitalValue(SET_BUZZER, value);
                    }
                    else if (id_4.Equals("4")) //M7204 Led Light On/Off  : id1 = '2', id2 = '2', id3 = '9', id4 = '4'
                    {
                        result = SetDigitalValue(SET_LEDLIGHT_ONOFF, value);
                    }
                    else if (id_4.Equals("6")) // M7206 Start Lamp On/Off : id1 = '2', id2 = '2', id3 = '9', id4 = '6'
                    {
                        result = SetDigitalValue(SET_START_LAMP, value);
                    }
                    else if (id_4.Equals("7")) // M7207 Stop Lamp On/Off : id1 = '2', id2 = '2', id3 = '9', id4 = '7'
                    {
                        result = SetDigitalValue(SET_STOP_LAMP, value);
                    }
                    else if (id_4.Equals("8")) // M7208 Reset Lamp On/Off : id1 = '2', id2 = '2', id3 = '9', id4 = '8'
                    {
                        result = SetDigitalValue(SET_RESET_LAMP, value);
                    }
                    else if (id_4.Equals("10")) // M7210 Scanner Blow On/Off : id1 = '2', id2 = '2', id3 = '9', id4 = '10'
                    {
                        result = SetDigitalValue(SET_SCANNER_BLOW, value);
                    }
                    else if (id_4.Equals("11")) //M7211 AIR CLEARNING On/Off  : id1 = '2', id2 = '2', id3 = '9', id4 = '11'
                    {
                        result = SetDigitalValue(SET_CLEARNING, value);
                    }
                    else if (id_4.Equals("16")) //M7216 EXT ACTIVATION : id1 = '2', id2 = '2', id3 = '9', id4 = '16'
                    {
                        result = SetDigitalValue(SET_EXT_ACTIVATION, value);
                    }
                    else if (id_4.Equals("17")) //M7217 LASER RESET : id1 = '2', id2 = '2', id3 = '9', id4 = '17'
                    {
                        result = SetDigitalValue(SET_LASER_RESET, value);
                    }
                    else if (id_4.Equals("18")) //M7218 LASER ON : id1 = '2', id2 = '2', id3 = '9', id4 = '18'
                    {
                        result = SetDigitalValue(SET_LASER_ON, value);
                    }
                    else if (id_4.Equals("19")) //M7219 PROGRAM START DYNAMIC ON : id1 = '2', id2 = '2', id3 = '9', id4 = '19'
                    {
                        result = SetDigitalValue(SET_PSTART_DYN, value);
                    }
                    else if (id_4.Equals("20")) //M7220 PROGRAM NO SET : id1 = '2', id2 = '2', id3 = '9', id4 = '20'
                    {
                        result = SetDigitalValue(SET_PROGRAM_NO_BIT_0, value);
                    }
                    else if (id_4.Equals("21")) //M7221 LASER STANDBY : id1 = '2', id2 = '2', id3 = '9', id4 = '21'
                    {
                        result = SetDigitalValue(SET_LASER_STANDY, value);
                    }
                    else if (id_4.Equals("22")) //M7222 LASER PROGRAM START STATIC : id1 = '2', id2 = '2', id3 = '9', id4 = '22'
                    {
                        result = SetDigitalValue(SET_PSTART_STATICAL, value);
                    }
                    else if (id_4.Equals("23")) //M7223 PILOT LASER ON : id1 = '2', id2 = '2', id3 = '9', id4 = '23'
                    {
                        result = SetDigitalValue(SET_PILOT_LASER_ON, value);
                    }
                    else if (id_4.Equals("24")) //M7224 REQUEST LASER : id1 = '2', id2 = '2', id3 = '9', id4 = '24'
                    {
                        result = SetDigitalValue(SET_REQUEST_LASER, value);
                    }
                }
            }
            else
            {
                result = false;
            }

        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            throw new NotImplementedException();
        }

        private bool CommandTowerLampRedOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_RED, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if(_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandTowerLampRedOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandTowerLampRedOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTowerLampYellowOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_YELLOW, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandTowerLampYellowOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandTowerLampYellowOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTowerLampGreenOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_GREEN, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandTowerLampGreenOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandTowerLampGreenOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandBuzzerOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_BUZZER, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandBuzzerOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandBuzzerOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }


        private bool CommandLedLightOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_LEDLIGHT_ONOFF, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandLedLightOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandLedLightOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandLaserShutterFwd(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_LASER_SHUTTER_FORWARD, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandLaserShutterFwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandLaserShutterFwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandLaserShutterBwd(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_LASER_SHUTTER_BACKWARD, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandLaserShutterBwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandLaserShutterBwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTableVaccumOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_TABLE_VACCUM_ONOFF, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandTableVaccumOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandTableVaccumOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTableFurgeOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_TABLE_FURGE_ONOFF, setOnOff);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandTableFurgeOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandTableFurgeOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetRelPosition(int axis, double distance)
        {
            switch (axis)
            {
                case iAXIS_X:
                    {
                        _XAxisRelSetPosition = distance;
                    }
                    break;
                case iAXIS_Y:
                    {
                        _YAxisRelSetPosition = distance;
                    }
                    break;
                case iAXIS_Z:
                    {
                        _ZAxisRelSetPosition = distance;
                    }
                    break;
                case iAXIS_T:
                    {
                        _TAxisRelSetPosition = distance;
                    }
                    break;
                case iAXIS_R:
                    {
                        _RAxisRelSetPosition = distance;
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            return true;
        }

        private bool SetAbsPosition(int axis, double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strPosition = pos.ToString("F3");

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_X_SET_ABSOLUTE_POSTION, strPosition);
                        _XAxisAbsSetPosition = pos;
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_Y_SET_ABSOLUTE_POSTION, strPosition);
                        _YAxisAbsSetPosition = pos;
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_Z_SET_ABSOLUTE_POSTION, strPosition);
                        _ZAxisAbsSetPosition = pos;
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_T_SET_ABSOLUTE_POSTION, strPosition);
                        _TAxisAbsSetPosition = pos;
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_R_SET_ABSOLUTE_POSTION, strPosition);
                        _RAxisAbsSetPosition = pos;
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }



            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] SetPostion() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SetPostion() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetVelocity(int axis, double velocity)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            switch (axis)
            {
                case iAXIS_X:
                    {
                        _XAxisSetVelocity = velocity * _XAxisVelocityConvert;
                        string strVelocity = velocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I222", _XAxisSetVelocity);

                    }
                    break;
                case iAXIS_Y:
                    {
                        _YAxisSetVelocity = velocity * _YAxisVelocityConvert;
                        string strVelocity = _YAxisSetVelocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I122", strVelocity);

                    }
                    break;
                case iAXIS_Z:
                    {
                        _ZAxisSetVelocity = velocity * _ZAxisVelocityConvert;
                        string strVelocity = _ZAxisSetVelocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I522", strVelocity);

                    }
                    break;
                case iAXIS_T:
                    {
                        _TAxisSetVelocity = velocity * _TAxisVelocityConvert;
                        string strVelocity = _TAxisSetVelocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I322", strVelocity);

                    }
                    break;
                case iAXIS_R:
                    {
                        _RAxisSetVelocity = velocity * _RAxisVelocityConvert;
                        string strVelocity = _RAxisSetVelocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I422", strVelocity);

                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] SetVelocity() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SetVelocity() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAbsPositionMoveStop(int axis, int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_X_MOVE_STOP, setValue);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_Y_MOVE_STOP, setValue);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_Z_MOVE_STOP, setValue);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_T_MOVE_STOP, setValue);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_R_MOVE_STOP, setValue);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAbsPostionMoveStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAbsPostionMoveStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }


        private bool CommandMoveToRelPosition(int axis, int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#2J", _XAxisRelSetPosition * _XAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#1J", _YAxisRelSetPosition * _YAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#5J", _ZAxisRelSetPosition * _ZAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#3J", _TAxisRelSetPosition * _TAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#4J", _RAxisRelSetPosition * _RAxisUnitPerCounts);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }

        private bool CommandMotonAbort(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_ALL:
                    strRequest.AppendFormat("#1A #2A #3A #4A #5A");
                    break;
                case iAXIS_X:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_X,  MOTION_ABORT);
                    break;
                case iAXIS_Y:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Y, MOTION_ABORT);
                    break;
                case iAXIS_Z:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Z, MOTION_ABORT);
                    break;
                case iAXIS_T:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_T, MOTION_ABORT);
                    break;
                case iAXIS_R:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_R, MOTION_ABORT);
                    break;
                default:
                    LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandMotonAbort() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                    return false;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandMotonAbort() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandMotonAbort() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandMotonQuit(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_X, MOTION_QUIT);
                    break;
                case iAXIS_Y:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Y, MOTION_QUIT);
                    break;
                case iAXIS_Z:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Z, MOTION_QUIT);
                    break;
                case iAXIS_T:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_T, MOTION_QUIT);
                    break;
                case iAXIS_R:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_R, MOTION_QUIT);
                    break;
                default:
                    LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandMotonQuit() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                    return false;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandMotonQuit() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandMotonQuit() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandServoStop(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_X, SERVO_STOP);
                    break;
                case iAXIS_Y:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Y, SERVO_STOP);
                    break;
                case iAXIS_Z:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_Z, SERVO_STOP);
                    break;
                case iAXIS_T:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_T, SERVO_STOP);
                    break;
                case iAXIS_R:
                    strRequest.AppendFormat("#{0}{1}", sAXIS_R, SERVO_STOP);
                    break;
                default:
                    LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                    return false;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }
        private bool CommnadMoveToSetPosition(int axis, int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}={1}", "#2J", _XAxisAbsSetPosition*_XAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}={1}", "#1J", _YAxisAbsSetPosition*_YAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("{0}={1}", "#5J", _ZAxisAbsSetPosition*_ZAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("{0}={1}", "#3J", _TAxisAbsSetPosition*_TAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("{0}={1}", "#4J", _RAxisAbsSetPosition*_RAxisUnitPerCounts);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }        
        }

        private bool CommandJogFoward(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch(axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_X, JOG_FORWARD);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Y, JOG_FORWARD);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Z, JOG_FORWARD);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_T, JOG_FORWARD);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_R, JOG_FORWARD);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;

                    if (string.IsNullOrEmpty(strResponse))
                    {
                        if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandJogFoward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return true;
                    }
                    else
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandJogFoward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return false;
                    }
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            return true;
        }

        private bool CommandJogBackward(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_X, JOG_BACKWARD);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Y, JOG_BACKWARD);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Z, JOG_BACKWARD);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_T, JOG_BACKWARD);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_R, JOG_BACKWARD);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;

                    if (string.IsNullOrEmpty(strResponse))
                    {
                        if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandJogBackward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return true;
                    }
                    else
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandJogBackward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return false;
                    }
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            return true;
        }

        private bool CommandJogStop(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_X, JOG_STOP);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Y, JOG_STOP);
                    }
                    break;
                case iAXIS_Z:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_Z, JOG_STOP);
                    }
                    break;
                case iAXIS_T:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_T, JOG_STOP);
                    }
                    break;
                case iAXIS_R:
                    {
                        strRequest.AppendFormat("#{0} J{1}", sAXIS_R, JOG_STOP);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandJogStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandJogStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }





        private bool CommandAxisXHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_X_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisXHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisXHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisXHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_X_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisXHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisXHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisYHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Y_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisYHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisYHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisYHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Y_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisYHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisYHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisZHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Z_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisZHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisZHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisZHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Z_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisZHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisZHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisTHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_T_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisTHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisTHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisTHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_T_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisTHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisTHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisRHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_R_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisRHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisRHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisRHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_R_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisRHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisRHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandServoKillAll()
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            //strRequest.Append(0x1D);
            strRequest.Append("#1K #2K #3K #4K #5K");

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommandAxisRHommingStop() : SendMessage={0}, ResponseMessage={1}", "^k", strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommandAxisRHommingStop() : SendMessage={0}, ResponseMessage={1}", "^k", strResponse);
                return false;
            }
        }

        private bool QueryPosisionByAxis(string axis, ref double position)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;
            double retVal = 0.0;

            strRequest.AppendFormat("#{0} {1}", axis, GET_POSITION);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (double.TryParse(strResponse, out position))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPosVelByAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPosVelByAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryVelocityByAxis(string axis, ref double velocity)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;
            double retVal = 0.0;

            strRequest.AppendFormat("#{0} {1}", axis, GET_VELOCITY);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (double.TryParse(strResponse, out velocity))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPosVelByAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPosVelByAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }


        private bool QueryPosition(string address, ref double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", address);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if(double.TryParse(strResponse, out pos))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryVelocity(string address, ref double vel)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", address);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (double.TryParse(strResponse, out vel))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisX(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisY(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsHommingAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsHommingAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisZ(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Z_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsHommingAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsHommingAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisT(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_T_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsHommingAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsHommingAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisR(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_R_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsHommingAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsHommingAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisX(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisY(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryHommingCompletedAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryHommingCompletedAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisZ(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Z_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryHommingCompletedAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryHommingCompletedAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }


        private bool QueryHommingCompletedAxisT(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_T_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryHommingCompletedAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryHommingCompletedAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisR(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_R_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryHommingCompletedAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryHommingCompletedAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisX(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisY(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsMovingAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsMovingAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisZ(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Z_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsMovingAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsMovingAxisZ() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisT(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_T_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsMovingAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsMovingAxisT() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisR(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_R_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryIsMovingAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryIsMovingAxisR() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryEmergencyDoorOpen(ref int isOpen)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", EMERGENCY_DOOR_OPEN_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isOpen))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryEmergencyDoorOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryEmergencyDoorOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryEmergencyCpBoxOpen(ref int isOpen)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", EMERGENCY_CPBOX_OPEN_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isOpen))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryEmergencyCpBoxOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryEmergencyCpBoxOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryGasAlarmStatus(ref int isAlarm)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", GAS_ALARM_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isAlarm))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryGasAlarmStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryGasAlarmStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryLaserShutterFwdStatus(ref int isForward)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", LASER_SHUTTER_FORWARD_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isForward))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryLaserShutterFwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryLaserShutterFwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryLaserShutterBwdStatus(ref int isBackward)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", LASER_SHUTTER_BACKWARD_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isBackward))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryLaserShutterBwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryLaserShutterBwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumStatus(ref int isVaccumOn)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isVaccumOn))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryTableVaccumStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryTableVaccumStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumPressureOnStatus(ref int vaccumPressure)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_PRESSURE_ON_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out vaccumPressure))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryTableVaccumPressureOnStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryTableVaccumPressureOnStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumDigitalPressure(ref int vaccumPressure)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_DIGITAL_PRESSURE_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out vaccumPressure))
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryTableVaccumDigitalPressure() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryTableVaccumDigitalPressure() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryMotionStatus(int iAxis, out bool[] status)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            bool result = false;
            status = null;
            strRequest.AppendFormat("#{0}{1}", iAxis, "?");

            CommandOrQuery(strRequest.ToString(), out strResponse);

            byte[] arrByte = ASCIIEncoding.ASCII.GetBytes(strResponse);

            if(arrByte != null && arrByte.Length > 0)
            {
                List<bool> bitList = new List<bool>();

                foreach(byte c in arrByte)
                {
                    bool[] bitArr = AppUtil.ConvertByteToBoolArray(c);
                    bitList.AddRange(bitArr);
                }

                status = bitList.ToArray();

                result = true;
            }
            else
            {
                result = false;
            }

            if (result)
            {
                if (_isWriteLog) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryMotionStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryMotionStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private void CommandOrQuery(string strRequest, out string strResponse)
        {

            strResponse = "";

            if (m_bDriverOpen == 0)
                return;

            //C# :  public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, StringBuilder command);
            //C : long PmacGetResponseExA(DWORD dwDevice,PCHAR response,UINT maxchar,PCHAR command);
            IntPtr pResponse;
            long nRetVal = 0;
            long nRetCommCharLength = 0;
            uint dwMaxchar = 10240;
            byte[] bBuffer;


            //Unmanaged 동적영역 할당
            pResponse = System.Runtime.InteropServices.Marshal.AllocHGlobal((int)dwMaxchar);

            //PMAC에 값 쿼리
            nRetVal = PCOMM32.PmacGetResponseExA(m_dwDevice, pResponse, dwMaxchar, new StringBuilder(strRequest));

            //리턴값 검사 1
            if (pResponse != IntPtr.Zero)
            {
                //반환된 ASCII Char수 확인
                nRetCommCharLength = PCOMM32.getCOMM_CHARS(nRetVal);

                //Managed 동적영역 할당
                bBuffer = new byte[nRetCommCharLength];

                Marshal.Copy(pResponse, bBuffer, 0, (int)nRetCommCharLength);

                strResponse = System.Text.Encoding.ASCII.GetString(bBuffer);
            }
            else
            {
                //리턴값 검사 2
                strResponse = ErrorHandlingASCIICommunication(nRetVal);
            }
            //unmanaged 동적영역 해제
            System.Runtime.InteropServices.Marshal.FreeHGlobal(pResponse);


        }//end function - private void CommandOrQuery()

        private string ErrorHandlingASCIICommunication(long c)
        {
            long nRetVal;
            bool isErrorOccur = false;
            StringBuilder sbResult = new StringBuilder();
            //To check for individual error codes the MACROs below are very useful:
            //if (PMAC.IS_COMM_MORE(c)){ textBox_CmdResponse.AppendText("COMM_MORE" + "\n"); }
            //else if(PMAC.IS_COMM_EOT( c)){ textBox_CmdResponse.AppendText(Constants.STR_COMM_EOT); }
            //else 
            if (PCOMM32.IS_COMM_TIMEOUT(c)) { sbResult.Append(Constants.STR_COMM_TIMEOUT); }
            else if (PCOMM32.IS_COMM_BADCKSUM(c)) { sbResult.Append(Constants.STR_COMM_BADCKSUM); }
            else if (PCOMM32.IS_COMM_ERROR(c)) { sbResult.Append(Constants.STR_COMM_ERROR); }
            else if (PCOMM32.IS_COMM_FAIL(c)) { sbResult.Append(Constants.STR_COMM_FAIL); }
            else if (PCOMM32.IS_COMM_ANYERROR(c))
            {
                isErrorOccur = true;
            }
            else if (PCOMM32.IS_COMM_UNSOLICITED(c)) { sbResult.Append(Constants.STR_COMM_UNSOLICITED); }


            if (isErrorOccur)
            {
                StringBuilder strResponse = new StringBuilder();
                nRetVal = PCOMM32.PmacGetErrorStrA(m_dwDevice, strResponse, 512);
                if (nRetVal > 0)
                {
                    sbResult.Append(Convert.ToString(strResponse));
                }
            }

            return sbResult.ToString();
        }
    }
}
