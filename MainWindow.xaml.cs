using HalconDotNet;
using kun28.Models;
using kun28.utils;
using kun28.ViewModel;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using static kun28.utils.HalconUtil;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using Window = System.Windows.Window;

namespace kun28
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm = new MainViewModel();
        private static CustomTaskRunner ctr;

        private static HTuple hv_ModelID;

        static LinkTask TanSuoDengLong = new LinkTask("tansuo_0");

        static LinkTask BaoXiangJieShuang = new LinkTask("jieshuang", 2000, tN: TanSuoDengLong, fN: TanSuoDengLong);
        static LinkTask BaoXiang = new LinkTask("baoxiang", 3500, tN: BaoXiangJieShuang, fN: TanSuoDengLong);
        static LinkTask DiaoLuoJieShuang = new LinkTask("diaoluo2", 3000, fN: BaoXiang, offsetY: -100);
        static LinkTask DiaoLuo = new LinkTask("diaoluo", 6000, tN: DiaoLuoJieShuang, fN: BaoXiang);

        static LinkTask ShouLingJieShuang = new LinkTask("jieshuang", 12000, times: 5, tN: DiaoLuo, fN: TanSuoDengLong);
        static LinkTask GongjiJieShuang = new LinkTask("jieshuang", 12000, times: 5, fN: TanSuoDengLong);
        static LinkTask RightMove = new LinkTask("right", 1000, times: 5, offsetX: 150, offsetY: -150);
        static LinkTask Gongji = new LinkTask("gongji", 2000, tN: GongjiJieShuang, fN: RightMove);
        static LinkTask ShouLing = new LinkTask("shouling", 2000, tN: ShouLingJieShuang, fN: Gongji);


        static LinkTask TanSuo = new LinkTask("tansuo_1", 2000, tN: ShouLing, fN: TanSuoDengLong);
        static LinkTask Di28Zhang = new LinkTask("di28zhang", tN: TanSuo, fN: TanSuo);

        static LinkTask TupoClose = new LinkTask("close", tN: Di28Zhang, fN: Di28Zhang);

        static LinkTask ShuaXinQueDing = new LinkTask("queding", delayBefore: 2000, fN: TupoClose);
        static LinkTask ShuaXin = new LinkTask("shuaxin", fN: TupoClose);

        static LinkTask CiShuJieShuang = new LinkTask("jieshuang", 2000);
        static LinkTask JinGongJieShuang = new LinkTask("jieshuang", 5000, tN: CiShuJieShuang);
        static LinkTask ShiBai = new LinkTask("shibai", 5000, fN: JinGongJieShuang);

        static LinkTask QuXiaoXuanZhe = new LinkTask("quxiaoxuanzhe", times: 3, offsetX: 150);
        static LinkTask JinGongCheck = new LinkTask("jingong", tN: QuXiaoXuanZhe, fN: ShiBai, click: false);
        static LinkTask JinGong = new LinkTask("jingong", times: 3, tN: JinGongCheck, fN: ShuaXin);
        static LinkTask XuanZhe = new LinkTask("xuanzhe", tN: JinGong, fN: ShuaXin);
        static LinkTask TuPo = new LinkTask("tupou1", tN: XuanZhe, fN: Di28Zhang);

        static LinkTask TanSuoClose = new LinkTask("close", tN: TuPo, fN: TuPo);
        static LinkTask TuPoJuan = new LinkTask("tupou", tN: TanSuoClose, fN: Di28Zhang, click: false, matchMode: TemplateMatchModes.SqDiffNormed);

        //static string DATAURL = "run.xml";

        //RunParams runParams;

        //public static bool Debugger = false;

        private LinkTaskCollection data;

        //private LinkTask first;

        private KeyBoardListener _keyboardHook;

        //private int LoadData()
        //{
        //    var runParamsSerialize = new XmlSerializer(typeof(RunParams));
        //    var serialize = new XmlSerializer(typeof(LinkTaskCollection));
        //    int count = 0;
            
        //    using (StreamReader reader = new StreamReader(DATAURL))
        //    {
        //        try
        //        {
        //            runParams = (RunParams)runParamsSerialize.Deserialize(reader);
        //            reader.Close();
        //            reader.Dispose();
        //            Debugger = runParams.DeBugger;

        //            using (StreamReader taskDataReader = new StreamReader(runParams.TaskDataURL)) {
        //                data = (LinkTaskCollection)serialize.Deserialize(taskDataReader);
        //                taskDataReader.Close();
        //                taskDataReader.Dispose();

        //                LinkTask.path = data.ImageURL;
        //                Dictionary<string, LinkTask> linkTaskMap = new Dictionary<string, LinkTask>(data.TaskParamsList.Count);

        //                data.TaskParamsList.ForEach(t =>
        //                {
        //                    if (linkTaskMap.ContainsKey(t.taskName))
        //                    {
        //                        MessageBox.Show("加载数据时检测到taskName重复");
        //                        count = -1;
        //                    }
        //                    linkTaskMap.Add(t.taskName, LinkTask.ConverterByParams(t));
        //                });

        //                data.TaskParamsList.ForEach(t =>
        //                {
        //                    if (linkTaskMap.ContainsKey(t.TN))
        //                        linkTaskMap[t.taskName].TN = linkTaskMap[t.TN];
        //                    if (linkTaskMap.ContainsKey(t.FN))
        //                        linkTaskMap[t.taskName].FN = linkTaskMap[t.FN];
        //                });

        //                first = linkTaskMap[data.TaskParamsList[0].taskName];

        //                return count == 0 ? data.TaskParamsList.Count : -1; 
        //            }
        //        }
        //        catch(InvalidOperationException e)
        //        {
        //            MessageBox.Show("数据加载异常 :" + e.Message);
        //        }
        //        return -1;
        //    }
        //}

        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.PrimaryScreenWidth - Width - 3;
            DataContext = vm;

            //int count = LoadData();

            //if (count < 1)
            //{
            //    if(count == 0)
            //        MessageBox.Show("读取到数据为空");
            //    Close();
            //    return;
            //}

            //if (Debugger) MessageBox.Show("测试模式：请将目标窗口放在 0, 0, 2000, 980 范围内");


            _keyboardHook = new KeyBoardListener();
            _keyboardHook.KeyPressed += (sender, args) =>
            {
                //Ctrl + K + R
                if (args.Key == Key.R && Keyboard.IsKeyDown(Key.K) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (vm.Status == MainViewModel.STOP)
                    {
                        if (vm.Times > 0) run();
                    }
                    else if(vm.Status == MainViewModel.PAUSE)
                        ctr?.Resume();
                    if (vm.Times > 0) vm.Status = MainViewModel.START;
                }

                //Ctrl + K + P
                if (args.Key == Key.P && Keyboard.IsKeyDown(Key.K) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (vm.Status == MainViewModel.START)
                    {
                        ctr?.Pause();
                        vm.Status = MainViewModel.PAUSE;
                    }
                }

                //Ctrl + K + X
                if (args.Key == Key.X && Keyboard.IsKeyDown(Key.K) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (vm.Status == MainViewModel.START || vm.Status == MainViewModel.PAUSE)
                    {
                        vm.Status = MainViewModel.STOP;
                        ctr?.Stop();
                    }
                }

            };

            HOperatorSet.ReadShapeModel("D:/kun28image/tupoTemplate.shm", out hv_ModelID);

            DiaoLuoJieShuang.TN = DiaoLuo;
            RightMove.TN = ShouLing;
            RightMove.FN = TanSuoDengLong;
            GongjiJieShuang.TN = ShouLing;

            XuanZhe.run = async () =>
            {
                await Task.Delay(3000);
                MatchResult res = matchByGrayTemplate(BitMapToHImage(ScreenImageUtils.CaptureScreen()), hv_ModelID, 0.5F);
                if (res.score > 0)
                {
                    await MouseControlUtils.moveAndClick(res.p);
                    return JinGong;
                }
                return ShuaXin;
            };
            ShuaXin.TN = ShuaXinQueDing;
            ShuaXinQueDing.TN = XuanZhe;

            ShiBai.TN = XuanZhe;
            JinGongJieShuang.TN = CiShuJieShuang;
            JinGongJieShuang.FN = ShiBai;

            CiShuJieShuang.TN = XuanZhe;
            CiShuJieShuang.FN = XuanZhe;

            QuXiaoXuanZhe.TN = TupoClose;
            QuXiaoXuanZhe.FN = TanSuoDengLong;

            //开始
            TanSuoDengLong.TN = TuPoJuan;
            TanSuoDengLong.FN = TuPoJuan;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            hv_ModelID?.Dispose();
            base.OnClosing(e);
        }

        public static async Task moveAndClick(Point p)
        {
            await MouseControlUtils.SmoothMoveTo(p);
            await ctr?.checkPause();
            if (ctr != null && ctr.checkCancel()) return;
            await MouseControlUtils.Click();
        }

        private async Task run()
        {
            ctr = new CustomTaskRunner();

            LinkTask.ctr = ctr;

            while (vm.Times > 0)
            {
                LinkTask start = await TanSuoDengLong.run();
                await ctr.checkPause();
                if (ctr.checkCancel()) break;
                while (! (start == TanSuoDengLong))
                {
                    if (start == null) break;
                    await ctr.checkPause();
                    if (ctr.checkCancel()) break;
                    start = await start.run();
                }
                vm.Times--;
                Cv2.DestroyAllWindows();
                CVTools.init();
                await Task.Delay(3000);
            }

            //if (Debugger) Cv2.DestroyAllWindows();

            ctr.Stop();
            ctr.release();

            vm.Times = 0;
            vm.Status = MainViewModel.STOP;

        }

        private void Drag(object sender,MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }

        private void Start(object sender,RoutedEventArgs e)
        {
            if (vm.Status == MainViewModel.STOP)
            {
                if (vm.Times > 0) run();
            }
            else
                ctr?.Resume();
           if(vm.Times > 0) vm.Status = MainViewModel.START;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            if (vm.Status == MainViewModel.START)
            {
                ctr?.Pause();
                vm.Status = MainViewModel.PAUSE;
            }
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            if(vm.Status == MainViewModel.START || vm.Status == MainViewModel.PAUSE)
            {
                //vm.Times = 0;
                vm.Status = MainViewModel.STOP;
                ctr?.Stop();
            }
        }

        private void Exit(object sender,RoutedEventArgs e)
        {
            this.Close();
        }

        private void Discre(object sender,RoutedEventArgs e)
        {
            if(vm.Times > 0) vm.Times--;
        }

        private void Incre(object sender,RoutedEventArgs e)
        {
            vm.Times++;
        }

        private System.Windows.Point _startPoint;
        private bool _isDragging;

        private void DragButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(this);
            _isDragging = true;
            ((UIElement)sender).CaptureMouse();
        }

        private void DragButton_PreviewMouseMove(object sender,MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentPoint = e.GetPosition(this);
                Left += currentPoint.X - _startPoint.X;
                Top += currentPoint.Y - _startPoint.Y;
            }
        }

        private void DragButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }
    }

    public class StatusBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short operate;
            if (value is short v)
            {
                if (short.TryParse(parameter.ToString(), out operate))
                {
                    if (operate == MainViewModel.PAUSE)
                    {
                        return v == MainViewModel.START;
                    }
                    else if (operate == 3) return v == 0 || v == 2;
                    else return operate != v;
                }

            }

            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnabledImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool v)
            {
                if (v)
                {
                    return "/assets/" + parameter.ToString() + ".png";
                }else return "/assets/" + parameter.ToString() + "_band.png";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestClass
    {
        public string name { set; get; }
    }

}
