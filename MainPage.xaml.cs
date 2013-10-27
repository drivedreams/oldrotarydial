using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Devices;
using Microsoft.Phone.UserData;
using CN.SmartMad.Ads.WindowsPhone.WPF;
namespace oldrotarydial
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// 转盘位置变化增量
        /// </summary>
        int adddis = 82;
        /// <summary>
        /// 所有联系人列表
        /// </summary>
        List<Contact> contactlist = new List<Contact>();
        /// <summary>
        /// 实例化联系人对象
        /// </summary>
        Contacts contacts = new Contacts();
        /// <summary>
        /// 联系人电话号码对象
        /// </summary>
        ContactPhoneNumber contactphonenumber;
        RotateTransform rt;//偏转对象
        PhoneNumberChooserTask phonenumberchoosertask=new PhoneNumberChooserTask();
        Point iniposition;//触笔按下时的初始位置
        Point currentposition;//当前触笔位置
        double iniangle;//初始位置角度
        double currentangle;//转动当前角度
        double transformangle;//转动过的角度
        Point centerpoint = new Point(240, 470);//圆心
        double stopangle = 360;//设置按键最大偏转角度
        bool ifbacking = false;//防止回转过程中移动引起的转动
        bool ismousedown = false;//防止回转后未抬起时继续移动带来的偏转
        bool ifhadback = false;//防止鼠标抬起时重复回转
        string selectnumber;
        SoundEffect soundeffect;
        SoundEffectInstance soundInstance;
        /// <summary>
        /// 判断是否已播放拨号声
        /// </summary>
        Boolean ifhasplay = false;
        /// <summary>
        /// 震动时长
        /// </summary>
        TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            
        // Constructor
        public MainPage()
        {
            InitializeComponent();
           
            PhoneNumberChooserTask phonenumberchoosertask = new PhoneNumberChooserTask();

            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
            Stream dafeisounstream = Microsoft.Xna.Framework.TitleContainer.OpenStream("sounds/sound.wav");
            soundeffect = SoundEffect.FromStream(dafeisounstream);
            soundInstance =soundeffect.CreateInstance();
            soundInstance.IsLooped = false;

            SMAdBannerView banner = new SMAdBannerView("AdSpaceId");
            grid2.Children.Add(banner);
        }

        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ismousedown && !ifbacking)
            {
                currentposition = e.GetPosition(this);

                double currentdistance = (currentposition.X - centerpoint.X) * (currentposition.X - centerpoint.X) + (currentposition.Y - centerpoint.Y) * (currentposition.Y - centerpoint.Y);
                //if (currentdistance > (130 * 130) && currentdistance < 240 * 240)//限定光标在圆环内有效
                if (currentdistance < 240 * 240 && currentdistance > (125 * 125))//限定光标在圆内有效
                {
                    transformangle = currentangle - iniangle;
                    currentangle = getanglebyposition(currentposition, centerpoint);//获取当前点角度
                    canvas.RenderTransformOrigin = new Point(0, 0);
                    Debug.WriteLine("--------------iniangle--------------------------****" + currentangle);


                    if (transformangle > 5&&transformangle<stopangle)
                    {
                        rt.CenterX = 240;
                        rt.CenterY = 240;
                        rt.Angle = transformangle;
                        image1.RenderTransform = rt;
                    }
                    else
                    {
                        if (!ifhadback && !ifbacking && transformangle>= stopangle)
                        {
                            backrotation();
                        }
                    }

                }
            }
        }

        
        //根据位置获取角度方法
        private double getanglebyposition(Point postion, Point centerpostion)
        {
            double angle = Math.Atan2(postion.Y - centerpoint.Y, postion.X - centerpoint.X) * (180 / Math.PI);
            if (angle < 0)
            {
                angle = angle + 360;
            }
            return angle;
        }
        /// <summary>
        /// 根据点击位置确定停转角度和号码
        /// </summary>
        /// <param name="iniposition"></param>

        private void setstopangelandnumber(Point iniposition)
        {
            double x = iniposition.X, y = iniposition.Y -82;
            if (x > 363 && x < 444 && y > 247 && y < 307)                    //1
            {
                stopangle = 25;
                selectnumber = "1";
            }
            else
            {
                if (x > 307 && x < 385 && y > 186 && y < 264)               //2
                {
                    stopangle = 50;
                    selectnumber = "2";
                }
                else
                {
                    if (x > 225 && x < 303 && y > 152 && y < 233)            //3
                    {
                        stopangle = 75;
                        selectnumber = "3";
                    }
                    else
                    {
                        if (x > 159 && x < 239 && y > 143 && y < 218)       //4
                        {
                            stopangle = 100;
                            selectnumber = "4";
                        }
                        else
                        {
                            if (x > 66 && x < 146 && y > 204 && y < 286)   //5
                            {
                                stopangle = 125;
                                selectnumber = "5";
                            }
                            else
                            {
                                if (x > 23 && x < 103 && y > 276 && y < 352)//6
                                {
                                    stopangle = 150;
                                    selectnumber = "6";
                                }
                                else
                                {
                                    if (x > 13 && x < 93 && y > 360 && y < 440)//7
                                    {
                                        stopangle = 175;
                                        selectnumber = "7";
                                    }
                                    else
                                    {
                                        if (x > 40 && x < 120 && y > 439 && y < 519)//8
                                        {
                                            stopangle = 200;
                                            selectnumber = "8";
                                        }
                                        else
                                        {
                                            if (x > 99 && x < 179 && y > 497 && y < 577)//9
                                            {
                                                stopangle = 225;
                                                selectnumber = "9";
                                            }
                                            else
                                            {
                                                if (x > 179 && x < 259 && y > 526 && y < 606)//0
                                                {
                                                    stopangle = 260;
                                                    selectnumber = "0";
                                                }
                                                else{
                                                    if (x > 263 && x < 343 && y > 513 && y < 593)//*
                                                    {
                                                        stopangle = 290;
                                                        selectnumber = "*";
                                                    }
                                                    else
                                                    {
                                                        if (x > 333 && x < 413 && y > 473 && y < 553)//#
                                                        {
                                                            stopangle = 310;
                                                            selectnumber = "#";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>回转控制
        /// 回转控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ismousedown = true;//重置是否按下
            ifbacking = false;//重置是否正在回转
            ifhadback = false;//重置是否已经回转
            iniposition = e.GetPosition(this);
            currentposition = iniposition;
            iniangle = getanglebyposition(iniposition, centerpoint);
            rt = new RotateTransform();
            stopangle = 0;
            setstopangelandnumber(iniposition);
        }

        PhoneCallTask phonecalltasksearch = new PhoneCallTask();
        //回转动画
        private void backrotation()
        {
            
            ifbacking = true;//重置是否正在回转
            if (transformangle >= stopangle&&stopangle>0)
            {
                numberokstory.Stop();
                numberokstory.Begin();
                VibrateController.Default.Start(duration);
                if (soundInstance.State != SoundState.Playing)
                {
                    soundInstance.Play();
                }
                ifhasplay = false;
                telnumber.Text += selectnumber;
                searchincontacts(telnumber.Text);
                selectnumber = "";
                stopangle = 0;
            }
            DoubleAnimation da = new DoubleAnimation();
            Storyboard.SetTargetProperty(da, new PropertyPath(RotateTransform.AngleProperty));
            Storyboard.SetTarget(da, rt);
            da.From = transformangle;
            da.To = 0;
            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            //da.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard story = new Storyboard();
            story.Children.Add(da);
            story.Begin();
            
            transformangle = 0;//重置转动角度 防止下次转动时回转
            ifhadback = true;
        }
        #region 自动检索联系人
        private void searchincontacts(string telnumber)
        {
            Debug.WriteLine("点击搜索");
            contacts.SearchAsync("", FilterKind.DisplayName, "");
            Debug.WriteLine("搜索完成");
        }
        void contacts_SearchCompleted(object render, ContactsSearchEventArgs e)
        {
            contactlist = e.Results.ToList<Contact>();
            numberlist.Items.Clear();
            
            foreach (Contact contact in contactlist)
            {
                try
                {

                    contactphonenumber = contact.PhoneNumbers.ElementAt<ContactPhoneNumber>(0);


                    if (contactphonenumber.PhoneNumber.Contains(telnumber.Text))
                    {
                        ContactView contactview = new ContactView();
                        
                        contactview.contactname.Text = contact.DisplayName;
                        contactview.contactnumber.Text = contactphonenumber.PhoneNumber;
                        numberlist.Items.Add(contactview);

                    }
                }
                catch
                {

                }
            }
        }
        #endregion
        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ismousedown = false;//重置是否按下
            if (!ifhadback && !ifbacking)
            {
                backrotation();
            }
            transformangle = 0;
        }
        private void image1_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (transformangle >= stopangle)
            {
                ismousedown = false;
                if (!ifhadback && !ifbacking)
                {
                    backrotation();
                }
            }
         }

        private void image1_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (transformangle >= stopangle)
            {
                ismousedown = false;
                if (!ifhadback && !ifbacking)
                {
                    backrotation();
                }
            }
         }

        private void image1_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ismousedown = false;
            if (!ifhadback && !ifbacking)
            {
                backrotation();
            }
            transformangle = 0;
         }

        private void image2_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            if (telnumber.Text != "")
            {
                PhoneCallTask phonecalltask = new PhoneCallTask();
                phonecalltask.PhoneNumber = telnumber.Text;
                phonecalltask.Show();
            }
        }

        private void image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image2.Source = new BitmapImage(new Uri("Images/mid.png", UriKind.Relative));
        }

        private void image2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image2.Source = new BitmapImage(new Uri("Images/mid1.png", UriKind.Relative));
        }

        //单行无法显示
        private void telnumber_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (telnumber.Text.Length > 14)
            {
                telnumber.FontSize = 24;
                
            }
            else 
            {
                telnumber.FontSize = 48;
            }
        }

        private void image4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/about.xaml", UriKind.Relative));//跳转到添加便签页面
        }

        private void image4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image4.Source = new BitmapImage(new Uri("Images/about1.png", UriKind.Relative));
        }

        private void image4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image4.Source = new BitmapImage(new Uri("Images/about.png", UriKind.Relative));
        }

        private void image3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (telnumber.Text.Length > 0)
            {
                telnumber.Text = telnumber.Text.Substring(0, telnumber.Text.Length - 1);
            }
        }

        private void image3_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            telnumber.Text = "";
        }

        private void image3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image3.Source = new BitmapImage(new Uri("Images/delete1.png", UriKind.Relative));
        }

        private void image3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image3.Source = new BitmapImage(new Uri("Images/delete.png", UriKind.Relative));
        }
      
    }
}