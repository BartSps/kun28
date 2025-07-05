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

        private static readonly CTask tupouTask = new CTask("突破", "D:/kun28image/tansuo_1.png", async () =>
        {

            Point? p;
            p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/tupou.png"));
            Cv2.ImShow("start", new Mat("D:/kun28image/tupou.png"));
            if (p == null) return true;

            //关闭探索界面
            p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/close.png"));
            Cv2.ImShow("start", new Mat("D:/kun28image/close.png"));
            if (p != null)
            {
                await moveAndClick((Point)p);
                await Task.Delay(1000);
            }

            //打开结界突破
            p = null;
            for (int i = 0; i < 3 && p == null; i++)
            {
                p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/tupou1.png"));
                await Task.Delay(1000);
            }

            if (p == null) return false;
            await moveAndClick((Point)p);

            //突破
            while (true)
            {
                await Task.Delay(3000);
                //选择
                MatchResult res = matchByGrayTemplate(BitMapToHImage(ScreenImageUtils.CaptureScreen()), hv_ModelID, 0.83F);
                Cv2.ImShow("start", new Mat("D:/kun28image/weijingong.png"));

                if (res.score > 0)
                {
                    await Task.Delay(1000);
                    await moveAndClick(res.p);

                    //结算1
                    p = null;
                    while (p == null)
                    {
                        await Task.Delay(1000);
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jingong.png"));
                    }
                    await Task.Delay(1000);
                    await moveAndClick((Point)p);

                    await Task.Delay(10000);
                    p = null;
                    //等待结束
                    while (p == null)
                    {
                        //结算1
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));
                        //失败
                        if (p == null)
                        {
                            await Task.Delay(1000);
                            CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/shibai.png"));
                        }
                        await ctr?.checkPause();
                    }
                    if (p != null)
                    {
                        await Task.Delay(1000);
                        await moveAndClick((Point)p);
                        //结算2
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));
                        await Task.Delay(1000);
                        if (p != null) await moveAndClick((Point)p);
                    }
                    continue;
                }
                await ctr?.checkPause();

                break;
            }

            //关闭突破界面
            p = null;
            while (p == null)
            {
                p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));
                await Task.Delay(1000);
            }
            await moveAndClick((Point)p);
            await Task.Delay(1000);
            return true;
        });

        public List<CTask> tasks = new List<CTask>
        {
            new CTask("探索灯笼","D:/kun28image/tansuo_0.png"),


            tupouTask,

            new CTask("第二十八章","D:/kun28image/di28zhang.png",async ()=>{
                Point? p;
                await Task.Delay(2000);

                //第28章
                while (true)
                {
                    await Task.Delay(3000);
                    p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/di28zhang.png"));
                    Cv2.ImShow("start", new Mat("D:/kun28image/di28zhang.png"));

                    if(p == null) break;
                    await Task.Delay(1000);
                    await moveAndClick((Point) p);
                }
                return true;
            }),

            new CTask("探索","D:/kun28image/tansuo_1.png",delayBefore:2000,isLoop:true),


            new CTask("攻击","D:/kun28image/gongji.png",async ()=>{
                while(true){

                    //寻找首领按钮
                    await Task.Delay(1000);
                    Point? p =CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/shouling.png"));

                    Cv2.ImShow("start", new Mat("D:/kun28image/shouling.png"));
                    Cv2.ResizeWindow("start", new Size(5, 50));

                    if(p == null)
                    {
                        //寻找攻击按钮
                        await Task.Delay(1000);
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/gongji.png"));

                        Cv2.ImShow("start", new Mat("D:/kun28image/gongji.png"));

                        if(p == null)
                        {
                            //向右移动
                            p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/right.png"));


                            Cv2.ImShow("start", new Mat("D:/kun28image/right.png"));
                            Cv2.ResizeWindow("start", new Size(5, 50));

                            if(p == null) return false;
                            Point finalP = (Point) p;
                            finalP.Y -= 60;
                            await moveAndClick(finalP);
                            await Task.Delay(2000);
                            continue;
                        }

                        await moveAndClick((Point) p);
                        await Task.Delay(1000);

                        //等待结算
                        while (true)
                        {
                            await Task.Delay(1000);

                             p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));
                            Cv2.ImShow("start", new Mat("D:/kun28image/jieshuang.png"));


                            if(p != null)
                            {
                                await Task.Delay(1000);
                                await moveAndClick((Point) p);
                                await Task.Delay(1000);
                                break;
                            }
                        }
                        continue;
                    }

                    await Task.Delay(1000);
                    await moveAndClick((Point) p);

                    //等待结算
                    while (true)
                    {
                        await Task.Delay(1000);
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));

                        Cv2.ImShow("start", new Mat("D:/kun28image/jieshuang.png"));

                        if(p != null)
                        {
                            await Task.Delay(1500);
                            await moveAndClick((Point) p);
                            break;
                        }
                    }
                    //掉落
                    while (true)
                    {
                        await Task.Delay(4000);
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/diaoluo.png"));
                        Cv2.ImShow("start", new Mat("D:/kun28image/diaoluo.png"));

                        if(p == null) break;
                        await Task.Delay(1000);
                        await moveAndClick((Point) p);

                        //点击上方
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/diaoluo2.png"));

                        Cv2.ImShow("start", new Mat("D:/kun28image/diaoluo2.png"));

                        if(p != null)
                        {
                            Point dp = (Point) p;
                            dp.Y -= 200;
                            await moveAndClick(dp);
                        }

                    }

                    //宝箱
                    await Task.Delay(3500);
                    p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/baoxiang.png"));
                        Cv2.ImShow("start", new Mat("D:/kun28image/baoxiang.png"));

                    if(p != null)
                    {
                        await moveAndClick((Point) p);


                        await Task.Delay(2000);
                        p = CVTools.FindGrayImageOnScreen(new Mat("D:/kun28image/jieshuang.png"));

                        if(p != null)
                        {
                            await moveAndClick((Point) p);
                        }
                    }
                    break;
                }
                return true;
            }),

            tupouTask,
        };

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

        public static void SaveObject(string path, TestClass linkTask)
        {
            string json = JsonSerializer.Serialize(linkTask);
            File.WriteAllText(path, json);
        }

        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.PrimaryScreenWidth - Width - 3;
            DataContext = vm;

            HOperatorSet.ReadShapeModel("D:/kun28image/tupotemplate.shm", out hv_ModelID);

            DiaoLuoJieShuang.TN = DiaoLuo;
            RightMove.TN = ShouLing;
            RightMove.FN = TanSuoDengLong;
            GongjiJieShuang.TN = ShouLing;

            XuanZhe.run = async ()=>{
                await Task.Delay(3000);
                MatchResult res =  matchByGrayTemplate(BitMapToHImage(ScreenImageUtils.CaptureScreen()), hv_ModelID, 0.5F);
                if(res.score > 0)
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

            //string s = JsonConvert.SerializeObject(TanSuoDengLong);
            //MessageBox.Show(s);
            //LinkTask SLT = JsonConvert.DeserializeObject<LinkTask>(s);
            //MessageBox.Show(SLT.fileName);

            SaveObject("D:/test/serializer.txt",new TestClass() { name = "test"});
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            hv_ModelID.Dispose();
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
            //Point? p = CVTools.FindBySqDiffNormed(new Mat("D:/kun28image/tupou.png"));


            //if (p != null)
            //{
            //    Point P = (Point)p;
            //    MouseControlUtils.MoveTo(P.X, P.Y);
            //    MessageBox.Show(P.X + " " + P.Y);
            //}

            ctr = new CustomTaskRunner();

            LinkTask.ctr = ctr;

            while (vm.Times > 0)
            {
                await ctr.checkPause();
                if (ctr.checkCancel()) break;
                LinkTask start = await TanSuoDengLong.run();
                while (!start.fileName.Equals("tansuo_0"))
                {
                    await ctr.checkPause();
                    if (ctr.checkCancel()) break;
                    start = await start.run();
                }
                vm.Times--;
                Cv2.DestroyAllWindows();
                CVTools.init();
                await Task.Delay(3000);
            }

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
            vm.Status = MainViewModel.PAUSE;
            ctr?.Pause();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            vm.Status = MainViewModel.STOP;
            ctr?.Stop();
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
