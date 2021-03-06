using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ECS.UI.Windows
{
    /// <summary>
    /// ProcessExecutingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProcessExecutingWindow : Window
    {
        private string _ExecuteName;
        private bool _FunctionAbort;
        private int _CurrentProgress;
        private readonly BackgroundWorker _BackgroundWorker;

        public PROCESS_RESULT Result { get; set; }
        public ProcessExecutingWindow(string WindowTitle, string message, string executeName)
        {
            InitializeComponent();
            this.gbTitle.Header = WindowTitle;
            tbMessage.Text = message;
            _ExecuteName = executeName;
            _CurrentProgress = 0;
            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.WorkerReportsProgress = true;
            this._BackgroundWorker.DoWork += DoWork;
            this._BackgroundWorker.ProgressChanged += this.ProgressChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_BackgroundWorker.IsBusy)
            {
                this._BackgroundWorker.RunWorkerAsync();
                this._BackgroundWorker.RunWorkerCompleted += _BackgroundWorker_RunWorkerCompleted;
                this.ButtonOk.Visibility = Visibility.Hidden;
                this.ButtonAbort.Visibility = Visibility.Visible;
            }

        }

        private void _BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ButtonOk.Visibility = Visibility.Visible;
            ButtonAbort.Visibility = Visibility.Hidden;

            if(Result == PROCESS_RESULT.SUCCESS)
            {
                this.tbMessage.Text = string.Format("{0} : 실행 완료되었습니다.", this._ExecuteName);
            }
            else if(Result == PROCESS_RESULT.ABORT)
            {
                this.tbMessage.Text = string.Format("{0} : 실행이 중단되었습니다.", this._ExecuteName);
            }
            else
            {
                this.tbMessage.Text = string.Format("{0} : 실행이 실패하였습니다.", this._ExecuteName);
            }

            //if (Result == PRCESS_RESULT.SUCCESS)
            //    MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행 완료되었습니다.", this._ExecuteName));
            //else if (Result == PRCESS_RESULT.ABORT)
            //    MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행이 중단되었습니다.", this._ExecuteName));
            //else
            //    MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행이 실패하였습니다.", this._ExecuteName));
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Abort_Button_Click(object sender, RoutedEventArgs e)
        {
            _FunctionAbort = true;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbProgress.Value = e.ProgressPercentage;
            tbProgressMessage.Text = (string)e.UserState;
            tbProgressPercent.Text = string.Format("{0}%", e.ProgressPercentage);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(_ExecuteName))
            {
                Result = PROCESS_RESULT.FAIL;
                return;
            }

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(_ExecuteName);


            while (true)
            {
                Thread.Sleep(10);

                if (_FunctionAbort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION(_ExecuteName);
                    Result = PROCESS_RESULT.ABORT;
                    return;
                }
                else if (FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(_ExecuteName) == false)
                {
                    _BackgroundWorker.ReportProgress(100);
                    Result = PROCESS_RESULT.SUCCESS;
                    return;
                }
                else
                {
                    _CurrentProgress = FunctionManager.Instance.GET_FUNCTION_PROGRESS(_ExecuteName);
                    string processMessage = FunctionManager.Instance.GET_FUNCTION_PROCESS_MESSAGE(_ExecuteName);
                    _BackgroundWorker.ReportProgress(_CurrentProgress, processMessage);
                }
            }
        }
    }
}
