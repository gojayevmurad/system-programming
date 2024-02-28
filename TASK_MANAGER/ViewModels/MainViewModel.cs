using _1._task_manager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace _1._task_manager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Process selectedProcess;
        public Process SelectedProcess
        {
            get { return selectedProcess; }
            set { selectedProcess = value; OnPropertyChanged(); }
        }


        private ObservableCollection<Process> allProcesses;
        public ObservableCollection<Process> AllProcesses
        {
            get { return allProcesses; }
            set { allProcesses = value; OnPropertyChanged(); }
        }


        private string processName;
        public string ProcessName
        {
            get { return processName; }
            set { processName = value; OnPropertyChanged(); }
        }

        private string blackListItemName;
        public string BlackListItemName
        {
            get { return blackListItemName; }
            set { blackListItemName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<String> blackList;
        public ObservableCollection<String> BlackList
        {
            get { return blackList; }
            set { blackList = value; OnPropertyChanged(); }
        }


        public RelayCommand KillProcess { get; set; }
        public RelayCommand AddProcess { get; set; }
        public RelayCommand AddProcessToBlackList { get; set; }

        public MainViewModel()
        {
            BlackList = new ObservableCollection<string>();

            DispatcherTimer timer = new DispatcherTimer();
            AllProcesses = new ObservableCollection<Process>(Process.GetProcesses());

            Thread setProcessThread = new Thread(() =>
            {
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (s, e) =>
                {
                    LoadProcesses();
                    CheckBlackList();
                };
                timer.Start();
            });

            setProcessThread.Start();


            KillProcess = new RelayCommand((param) =>
            {
                if (SelectedProcess != null)
                {
                    SelectedProcess.Kill();
                }
            });

            AddProcess = new RelayCommand((param) =>
            {
                Process.Start(ProcessName);
                ProcessName = "";
            });


            AddProcessToBlackList = new RelayCommand((param) =>
            {
                if (BlackListItemName != null)
                {
                    BlackList.Add(BlackListItemName);
                    BlackListItemName = "";
                }
            });



        }

        public void LoadProcesses()
        {

            AllProcesses = new ObservableCollection<Process>(Process.GetProcesses());
        }

        public void CheckBlackList()
        {
            for (int i = 0; i < AllProcesses.Count; i++)
            {
                for (int k = 0; k < BlackList.Count; k++)
                {
                    if (AllProcesses[i].ProcessName.ToLower().Trim() == BlackList[k].ToLower().Trim())
                    {
                        AllProcesses[i].Kill();
                    }
                }
            }
        }



    }
}
